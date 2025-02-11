﻿using BSFoodFramework.DataAccess;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.Apoio;
using BSFoodFramework.BusinessLogic.Interfaces;
using BSFoodFramework.Models;
using System;
using System.Linq;
using System.Data.Entity;

namespace BSFoodFramework.BusinessLogic
{
    public class Cidades : ICidades, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public Cidades()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public Cidades(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        public Retorno RetornaCidade(int intCodigo, enNavegacao? enDirecao)
        {
            var objRetorno = new Retorno();
            try
            {
                tbCidade objCidade = null;
                if (enDirecao == null)
                    objCidade = _objCtx.tbCidade.AsNoTracking()
                                                    .FirstOrDefault(cid => cid.cid_codigo == intCodigo);
                if (enDirecao == enNavegacao.Proximo)
                    objCidade = _objCtx.tbCidade.AsNoTracking()
                                                    .Where(cid => cid.cid_codigo > intCodigo)
                                                    .OrderBy(cid => cid.cid_codigo).FirstOrDefault();
                if (enDirecao == enNavegacao.Anterior)
                    objCidade = _objCtx.tbCidade.AsNoTracking()
                                                    .Where(cid => cid.cid_codigo < intCodigo)
                                                    .OrderByDescending(cid => cid.cid_codigo).FirstOrDefault();
                if (objCidade != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objCidade;
                }
                else
                {
                    objRetorno.intCodigoErro = 48;
                    objRetorno.strMsgErro = "Registro não encontrado";
                }
            }
            catch (Exception ex)
            {
                Util.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno RetornaListaCidade(string strCodigo, string strNome, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                var arrCidade = _objCtx.tbCidade.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(strCodigo))
                {
                    int intCodigo = Convert.ToInt32(strCodigo);
                    arrCidade = arrCidade.Where(cid => cid.cid_codigo == intCodigo).AsQueryable();
                }
                if (!string.IsNullOrWhiteSpace(strNome))
                    arrCidade = arrCidade.Where(cid => cid.cid_nome.Contains(strNome)).AsQueryable();
                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrCidade.Count();
                objRetorno.objRetorno = arrCidade.OrderBy(cid => cid.cid_codigo).Skip(intSkip).Take(intTake).ToList();
            }
            catch (Exception ex)
            {
                Util.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno RetornaListaEstado()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrEstado = _objCtx.tbEstado.Include(cid => cid.tbCidade).AsNoTracking()
                                                      .OrderBy(est => est.est_nome)
                                                      .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrEstado;
            }
            catch (Exception ex)
            {
                Util.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno SalvarCidade(tbCidade objCidade, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaCidade(objCidade);
            try
            {
                if (strValidacao == string.Empty)
                {
                    enOperacao enTipoOperacao;
                    if (objCidade.cid_codigo == 0)
                    {
                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbCidade.Add(objCidade);
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objCidadeContexto = _objCtx.tbCidade.FirstOrDefault(cid => cid.cid_codigo == objCidade.cid_codigo);
                        _objCtx.Entry(objCidadeContexto).CurrentValues.SetValues(objCidade);
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objCidade.cid_codigo, enTipoOperacao, objCidade, intFunCodigo);
                    objRetorno.intCodigoErro = 0;
                }
                else
                {
                    objRetorno.intCodigoErro = 48;
                    objRetorno.strMsgErro = strValidacao;
                }
            }
            catch (Exception ex)
            {
                Util.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno ExcluirCidade(int intCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                //Cria um contexto isolado para a trasacao de exclusao
                using (var objContexto = new EFContexto())
                {
                    //Inicia uma transacao no contexto isolado
                    using (var transacao = objContexto.Database.BeginTransaction())
                    {
                        try
                        {
                            var objCidade = objContexto.tbCidade.FirstOrDefault(cid => cid.cid_codigo == intCodigo);
                            if (objCidade != null)
                            {
                                //Tenta excluir o perfil no contexto isolado
                                objContexto.tbCidade.Remove(objCidade);
                                objContexto.SaveChanges();
                                transacao.Commit();

                                objRetorno.intCodigoErro = 0;
                                objRetorno.objRetorno = true;
                            }
                            else
                            {
                                objRetorno.intCodigoErro = 48;
                                objRetorno.strMsgErro = "Cidade não encontrada para exclusão";
                            }
                        }
                        catch (Exception)
                        {
                            //Se deu erro é porque o perfil tem  registros relacionado
                            transacao.Rollback();
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = "Cidade não pode ser excluida pois há registros relacionados à mesma";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        private string ValidaCidade(tbCidade objCidade)
        {
            if (string.IsNullOrEmpty(objCidade.cid_nome) || string.IsNullOrWhiteSpace(objCidade.cid_nome))
                return "O nome deve ser informado.";

            return _objCtx.tbCidade
                .AsNoTracking().Any(cid => (cid.cid_nome.Equals(objCidade.cid_nome)) && cid.cid_codigo != objCidade.cid_codigo) ? "Já existe cidade com esse nome." : string.Empty;
        }

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
