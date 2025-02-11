﻿using BSFoodFramework.DataAccess;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.Apoio;
using BSFoodFramework.BusinessLogic.Interfaces;
using BSFoodFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace BSFoodFramework.BusinessLogic
{
    public class PerfilAcesso : IPerfilAcesso, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public PerfilAcesso()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public PerfilAcesso(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        public Retorno RetornaPerfilAcesso(int intCodigo, enNavegacao? enDirecao)
        {
            var objRetorno = new Retorno();
            try
            {
                tbPerfilAcesso objPerfilAcesso = null;
                if (enDirecao == null)
                    objPerfilAcesso = _objCtx.tbPerfilAcesso.Include(pam => pam.tbPerfilAcessoMenu.Select(men => men.tbMenu))
                                                      .AsNoTracking()
                                                      .FirstOrDefault(pac => pac.pac_codigo == intCodigo);
                if (enDirecao == enNavegacao.Proximo)
                    objPerfilAcesso = _objCtx.tbPerfilAcesso.Include(pam => pam.tbPerfilAcessoMenu.Select(men => men.tbMenu)).AsNoTracking()
                                                                          .Where(pac => pac.pac_codigo > intCodigo)
                                                                          .OrderBy(pac => pac.pac_codigo).FirstOrDefault();
                if (enDirecao == enNavegacao.Anterior)
                    objPerfilAcesso = _objCtx.tbPerfilAcesso.Include(pam => pam.tbPerfilAcessoMenu.Select(men => men.tbMenu)).AsNoTracking()
                                                                          .Where(pac => pac.pac_codigo < intCodigo)
                                                                          .OrderByDescending(pac => pac.pac_codigo).FirstOrDefault();
                if (objPerfilAcesso != null)
                {
                    objRetorno = RetornaListaMenu();
                    if (objRetorno.intCodigoErro == 0)
                    {
                        foreach (var objMenu in (List<tbMenu>)objRetorno.objRetorno)
                        {
                            if (objPerfilAcesso.tbPerfilAcessoMenu.FirstOrDefault(pam => pam.men_codigo == objMenu.men_codigo) == null)
                            {
                                objPerfilAcesso.tbPerfilAcessoMenu.Add(new tbPerfilAcessoMenu
                                {
                                    pac_codigo = 0,
                                    men_codigo = objMenu.men_codigo,
                                    pam_permiteAlteracao = false,
                                    pam_permiteExclusao = false,
                                    pam_permiteInclusao = false,
                                    pam_permiteVisualizacao = false,
                                    pam_toolBar = false,
                                    tbMenu = objMenu
                                });
                            }
                        }
                        objPerfilAcesso.tbPerfilAcessoMenu = objPerfilAcesso.tbPerfilAcessoMenu.OrderBy(pam => pam.tbMenu.men_ordem).ToList();
                        objRetorno.intCodigoErro = 0;
                        objRetorno.objRetorno = objPerfilAcesso;
                    }
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

        public Retorno RetornaListaPerfilAcesso()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrPerfilAcesso = _objCtx.tbPerfilAcesso.AsNoTracking()
                                                      .OrderBy(pac => pac.pac_descricao)
                                                      .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrPerfilAcesso;
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

        public Retorno RetornaListaPerfilAcesso(string strCodigo, string strNome, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                var arrPerfilAcesso = _objCtx.tbPerfilAcesso.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(strCodigo))
                {
                    int intCodigo = Convert.ToInt32(strCodigo);
                    arrPerfilAcesso = arrPerfilAcesso.Where(pac => pac.pac_codigo == intCodigo).AsQueryable();
                }
                if (!string.IsNullOrWhiteSpace(strNome))
                    arrPerfilAcesso = arrPerfilAcesso.Where(pac => pac.pac_descricao.Contains(strNome)).AsQueryable();
                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrPerfilAcesso.Count();
                objRetorno.objRetorno = arrPerfilAcesso.OrderByDescending(pac => pac.pac_codigo).Skip(intSkip).Take(intTake).ToList();
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

        public Retorno RetornaListaMenu()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrMenu = _objCtx.tbMenu.AsNoTracking()
                                                      .Where(men => men.men_status)
                                                      .OrderBy(men => men.men_ordem)
                                                      .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrMenu;
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

        public Retorno SalvarPerfilAcesso(tbPerfilAcesso objPerfilAcesso, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaPerfilAcesso(objPerfilAcesso);
            try
            {
                if (strValidacao == string.Empty)
                {
                    var arrPerfilAcessoMenuSalvar = objPerfilAcesso.tbPerfilAcessoMenu.Where(pam => pam.pam_permiteAlteracao
                                                                                                    || pam.pam_permiteExclusao
                                                                                                    || pam.pam_permiteInclusao
                                                                                                    || pam.pam_permiteVisualizacao).ToList();
                    foreach (var objPerfilAcessoMenu in objPerfilAcesso.tbPerfilAcessoMenu.Where(pam => pam.pam_permiteAlteracao
                                                                                || pam.pam_permiteExclusao
                                                                                || pam.pam_permiteInclusao
                                                                                || pam.pam_permiteVisualizacao).ToList())
                    {
                        ComplementaPerfilAcessoMenu(objPerfilAcessoMenu.pac_codigo, objPerfilAcessoMenu.tbMenu, ref arrPerfilAcessoMenuSalvar);
                    }

                    objPerfilAcesso.tbPerfilAcessoMenu.Clear();
                    foreach (var objPerfilAcessoMenuSalvar in arrPerfilAcessoMenuSalvar)
                    {
                        objPerfilAcessoMenuSalvar.tbMenu = null;
                        objPerfilAcessoMenuSalvar.tbPerfilAcesso = null;
                        objPerfilAcessoMenuSalvar.pac_codigo = objPerfilAcesso.pac_codigo;
                        objPerfilAcesso.tbPerfilAcessoMenu.Add(objPerfilAcessoMenuSalvar);
                    }

                    enOperacao enTipoOperacao;
                    if (objPerfilAcesso.pac_codigo == 0)
                    {
                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbPerfilAcesso.Add(objPerfilAcesso);
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objPerfilAcessoContexto = _objCtx.tbPerfilAcesso
                                                               .Include(pam => pam.tbPerfilAcessoMenu).FirstOrDefault(pac => pac.pac_codigo == objPerfilAcesso.pac_codigo);
                        if (objPerfilAcessoContexto != null)
                        {
                            _objCtx.tbPerfilAcessoMenu.RemoveRange(objPerfilAcessoContexto.tbPerfilAcessoMenu);
                            _objCtx.Entry(objPerfilAcessoContexto).CurrentValues.SetValues(objPerfilAcesso);
                        }

                        foreach (var objItem in objPerfilAcesso.tbPerfilAcessoMenu)
                            _objCtx.tbPerfilAcessoMenu.Add(objItem);
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objPerfilAcesso.pac_codigo, enTipoOperacao, objPerfilAcesso, intFunCodigo);
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

        public Retorno ExcluirPerfilAcesso(int intCodigo)
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
                            var objPerfilAcesso = objContexto.tbPerfilAcesso.Include(pam => pam.tbPerfilAcessoMenu).FirstOrDefault(pac => pac.pac_codigo == intCodigo);
                            if (objPerfilAcesso != null)
                            {
                                //Tenta excluir o perfil no contexto isolado
                                objContexto.tbPerfilAcesso.Remove(objPerfilAcesso);
                                objContexto.SaveChanges();
                                transacao.Commit();

                                objRetorno.intCodigoErro = 0;
                                objRetorno.objRetorno = true;
                            }
                            else
                            {
                                objRetorno.intCodigoErro = 48;
                                objRetorno.strMsgErro = "Perfil não encontrado para exclusão";
                            }
                        }
                        catch (Exception)
                        {
                            //Se deu erro é porque o perfil tem  registros relacionado
                            transacao.Rollback();
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = "Perfil não pode ser excluido pois há registros relacionados ao mesmo";
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

        private string ValidaPerfilAcesso(tbPerfilAcesso objPerfilAcesso)
        {
            if (string.IsNullOrEmpty(objPerfilAcesso.pac_descricao) || string.IsNullOrWhiteSpace(objPerfilAcesso.pac_descricao))
                return "A Descrição deve ser informada.";

            if (_objCtx.tbPerfilAcesso.AsNoTracking().Any(pac => (pac.pac_descricao.Equals(objPerfilAcesso.pac_descricao)) && pac.pac_codigo != objPerfilAcesso.pac_codigo))
                return "Já existe perfil com esse nome.";

            var arrPerfilAcessoMenuSalvar = objPerfilAcesso.tbPerfilAcessoMenu.Where(pam => pam.pam_permiteAlteracao
                                                                                            || pam.pam_permiteExclusao
                                                                                            || pam.pam_permiteInclusao
                                                                                            || pam.pam_permiteVisualizacao).ToList();
            return arrPerfilAcessoMenuSalvar.Count == 0 ? "Não foram informados nenhum item no menu de definições gerais." : string.Empty;
        }

        private void ComplementaPerfilAcessoMenu(int intCodigoPerfilAcesso, tbMenu objMenu, ref List<tbPerfilAcessoMenu> arrPerfilAcessoMenu)
        {
            while (true)
            {
                if (objMenu.men_codigoPai == null) return;
                var objMenuMatriz = _objCtx.tbMenu.AsNoTracking().FirstOrDefault(men => men.men_status && men.men_codigo == objMenu.men_codigoPai);
                var objPerfilAcessoMenu = arrPerfilAcessoMenu.FirstOrDefault(pem => objMenuMatriz != null && pem.men_codigo == objMenuMatriz.men_codigo);
                if (objPerfilAcessoMenu == null)
                {
                    if (objMenuMatriz == null) return;
                    arrPerfilAcessoMenu.Add(new tbPerfilAcessoMenu {pac_codigo = intCodigoPerfilAcesso, men_codigo = Convert.ToInt32(objMenuMatriz.men_codigo), pam_permiteAlteracao = true, pam_permiteExclusao = true, pam_permiteInclusao = true, pam_permiteVisualizacao = true, pam_toolBar = false, tbMenu = objMenuMatriz});
                    objMenu = objMenuMatriz;
                }
                else
                {
                    objMenu = objPerfilAcessoMenu.tbMenu;
                }
            }
        }

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
