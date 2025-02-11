﻿using BSFoodFramework.DataAccess;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.Apoio;
using BSFoodFramework.BusinessLogic.Interfaces;
using BSFoodFramework.Models;
using System;
using System.Linq;

namespace BSFoodFramework.BusinessLogic
{
    public class Produtos : IProdutos, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public Produtos()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public Produtos(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        public Retorno RetornaProduto(int intCodigo, enNavegacao? enDirecao)
        {
            var objRetorno = new Retorno();
            try
            {
                tbProduto objProduto = null;
                if (enDirecao == null)
                    objProduto = _objCtx.tbProduto.AsNoTracking()
                                                .FirstOrDefault(pro => pro.pro_codigo == intCodigo);
                if (enDirecao == enNavegacao.Proximo)
                    objProduto = _objCtx.tbProduto.AsNoTracking()
                                                .Where(pro => pro.pro_codigo > intCodigo)
                                                .OrderBy(pro => pro.pro_codigo).FirstOrDefault();
                if (enDirecao == enNavegacao.Anterior)
                    objProduto = _objCtx.tbProduto.AsNoTracking()
                                                .Where(pro => pro.pro_codigo < intCodigo)
                                                .OrderByDescending(pro => pro.pro_codigo).FirstOrDefault();
                if (objProduto != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objProduto;
                }
                else
                {
                    objRetorno.intCodigoErro = 48;
                    objRetorno.strMsgErro = "Registro não encontrado";
                }
            }
            catch (Exception ex)
            {
                FrameworkUtil.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno RetornaListaProduto(string strCodigo, string strNome, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                var arrProduto = _objCtx.tbProduto.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(strCodigo))
                {
                    int intCodigo = Convert.ToInt32(strCodigo);
                    arrProduto = arrProduto.Where(pro => pro.pro_codigo == intCodigo).AsQueryable();
                }
                if (!string.IsNullOrWhiteSpace(strNome))
                    arrProduto = arrProduto.Where(pro => pro.pro_nome.Contains(strNome)).AsQueryable();
                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrProduto.Count();
                objRetorno.objRetorno = arrProduto.OrderByDescending(pro => pro.pro_codigo).Skip(intSkip).Take(intTake).ToList();
            }
            catch (Exception ex)
            {
                FrameworkUtil.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno SalvarProduto(tbProduto objProduto, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaProduto(objProduto);
            try
            {
                if (strValidacao == string.Empty)
                {
                    enOperacao enTipoOperacao;
                    if (objProduto.pro_codigo == 0)
                    {
                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbProduto.Add(objProduto);
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objProdutoContexto = _objCtx.tbProduto.FirstOrDefault(pro => pro.pro_codigo == objProduto.pro_codigo);
                        _objCtx.Entry(objProdutoContexto).CurrentValues.SetValues(objProduto);
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objProduto.pro_codigo, enTipoOperacao, objProduto, intFunCodigo);
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
                FrameworkUtil.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        public Retorno ExcluirProduto(int intCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                using (var objContexto = new EFContexto())
                {
                    using (var transacao = objContexto.Database.BeginTransaction())
                    {
                        try
                        {
                            var objProduto = objContexto.tbProduto.FirstOrDefault(pro => pro.pro_codigo == intCodigo);
                            if (objProduto != null)
                            {
                                objContexto.tbProduto.Remove(objProduto);
                                objContexto.SaveChanges();
                                transacao.Commit();

                                objRetorno.intCodigoErro = 0;
                                objRetorno.objRetorno = true;
                            }
                            else
                            {
                                objRetorno.intCodigoErro = 48;
                                objRetorno.strMsgErro = "procionário não encontrado para exclusão";
                            }
                        }
                        catch (Exception)
                        {
                            transacao.Rollback();
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = "procionário não pode ser excluido pois há registros relacionados ao mesmo";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FrameworkUtil.LogErro(ex);
                objRetorno.intCodigoErro = 16;
                objRetorno.strMsgErro = ex.Message;
                objRetorno.strExceptionToString = ex.ToString();
            }
            return objRetorno;
        }

        private string ValidaProduto(tbProduto objProduto)
        {
            if (string.IsNullOrEmpty(objProduto.pro_nome) || string.IsNullOrWhiteSpace(objProduto.pro_nome))
                return "O nome deve ser informado.";

            return _objCtx.tbProduto.AsNoTracking().Any(pro => (pro.pro_nome.Equals(objProduto.pro_nome)) && pro.pro_codigo != objProduto.pro_codigo) ? "Já existe produto com esse nome." : string.Empty;
        }

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
