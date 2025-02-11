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
    public class FormaPagamento : IFormaPagamento, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;
        
        public FormaPagamento()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public FormaPagamento(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        public Retorno RetornaFormaPagamento(int intCodigo, enNavegacao? enDirecao)
        {
            var objRetorno = new Retorno();
            try
            {
                tbFormaPagamento objFormaPagamento = null;
                if (enDirecao == null)
                    objFormaPagamento = _objCtx.tbFormaPagamento.AsNoTracking().Include(fpt => fpt.tbFormaPagamentoTipo)
                                                    .FirstOrDefault(fpg => fpg.fpg_codigo == intCodigo);
                if (enDirecao == enNavegacao.Proximo)
                    objFormaPagamento = _objCtx.tbFormaPagamento.AsNoTracking().Include(fpt => fpt.tbFormaPagamentoTipo)
                                                    .Where(fpg => fpg.fpg_codigo > intCodigo)
                                                    .OrderBy(fpg => fpg.fpg_codigo).FirstOrDefault();
                if (enDirecao == enNavegacao.Anterior)
                    objFormaPagamento = _objCtx.tbFormaPagamento.AsNoTracking().Include(fpt => fpt.tbFormaPagamentoTipo)
                                                    .Where(fpg => fpg.fpg_codigo < intCodigo)
                                                    .OrderByDescending(fpg => fpg.fpg_codigo).FirstOrDefault();
                if (objFormaPagamento != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objFormaPagamento;
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

        public Retorno RetornaListaFormaPagamento()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrFormaPagamento = _objCtx.tbFormaPagamento.AsNoTracking()
                                                      .OrderBy(fpg => fpg.fpg_descricao)
                                                      .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrFormaPagamento;
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

        public Retorno RetornaListaFormaPagamentoTipo()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrFormaPagamentoTipo = _objCtx.tbFormaPagamentoTipo.AsNoTracking()
                                                      .OrderBy(fpt => fpt.fpt_codigo)
                                                      .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrFormaPagamentoTipo;
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

        public Retorno RetornaListaFormaPagamento(string strCodigo, string strNome, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                var arrFormaPagamento = _objCtx.tbFormaPagamento.AsNoTracking().Include(fpt => fpt.tbFormaPagamentoTipo).AsQueryable();
                if (!string.IsNullOrWhiteSpace(strCodigo))
                {
                    int intCodigo = Convert.ToInt32(strCodigo);
                    arrFormaPagamento = arrFormaPagamento.Where(fpg => fpg.fpg_codigo == intCodigo).AsQueryable();
                }
                if (!string.IsNullOrWhiteSpace(strNome))
                    arrFormaPagamento = arrFormaPagamento.Where(fpg => fpg.fpg_descricao.Contains(strNome)).AsQueryable();
                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrFormaPagamento.Count();
                objRetorno.objRetorno = arrFormaPagamento.OrderByDescending(fpg => fpg.fpg_codigo).Skip(intSkip).Take(intTake).ToList();
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

        public Retorno SalvarFormaPagamento(tbFormaPagamento objFormaPagamento, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaFormaPagamento(objFormaPagamento);
            try
            {
                if (strValidacao == string.Empty)
                {
                    enOperacao enTipoOperacao;
                    if (objFormaPagamento.fpg_codigo == 0)
                    {
                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbFormaPagamento.Add(objFormaPagamento);
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objFormaPagamentoContexto = _objCtx.tbFormaPagamento.FirstOrDefault(fpg => fpg.fpg_codigo == objFormaPagamento.fpg_codigo);
                        _objCtx.Entry(objFormaPagamentoContexto).CurrentValues.SetValues(objFormaPagamento);
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objFormaPagamento.fpg_codigo, enTipoOperacao, objFormaPagamento, intFunCodigo);
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

        public Retorno ExcluirFormaPagamento(int intCodigo)
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
                            var objFormaPagamento = objContexto.tbFormaPagamento.FirstOrDefault(fpg => fpg.fpg_codigo == intCodigo);
                            if (objFormaPagamento != null)
                            {
                                objContexto.tbFormaPagamento.Remove(objFormaPagamento);
                                objContexto.SaveChanges();
                                transacao.Commit();

                                objRetorno.intCodigoErro = 0;
                                objRetorno.objRetorno = true;
                            }
                            else
                            {
                                objRetorno.intCodigoErro = 48;
                                objRetorno.strMsgErro = "FormaPagamento não encontrado para exclusão";
                            }
                        }
                        catch (Exception)
                        {
                            transacao.Rollback();
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = "FormaPagamento não pode ser excluido pois há registros relacionados ao mesmo.";
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

        private string ValidaFormaPagamento(tbFormaPagamento objFormaPagamento)
        {
            if (string.IsNullOrEmpty(objFormaPagamento.fpg_descricao) || string.IsNullOrWhiteSpace(objFormaPagamento.fpg_descricao))
                return "O nome deve ser informado.";

            return _objCtx.tbFormaPagamento.AsNoTracking().Any(fpg => (fpg.fpg_descricao.Equals(objFormaPagamento.fpg_descricao)) && fpg.fpg_codigo != objFormaPagamento.fpg_codigo) ? "Já existe FormaPagamento com esse nome." : string.Empty;
        }

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
