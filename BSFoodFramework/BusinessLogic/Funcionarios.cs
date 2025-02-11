﻿using BSFoodFramework.DataAccess;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.Apoio;
using BSFoodFramework.BusinessLogic.Interfaces;
using BSFoodFramework.Models;
using System;
using System.Linq;
using System.Data.Entity;
using Dapper;

namespace BSFoodFramework.BusinessLogic
{
    public class Funcionarios : IFuncionarios, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public Funcionarios()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public Funcionarios(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        public Retorno RetornaFuncionario(int intCodigo, enNavegacao? enDirecao)
        {
            var objRetorno = new Retorno();
            try
            {
                tbFuncionario objFuncionario = null;
                if (enDirecao == null)
                    objFuncionario = _objCtx.tbFuncionario.Include(pac => pac.tbPerfilAcesso)
                                                .AsNoTracking()
                                                .FirstOrDefault(fun => fun.fun_codigo == intCodigo);
                if (enDirecao == enNavegacao.Proximo)
                    objFuncionario = _objCtx.tbFuncionario
                                                .Include(pac => pac.tbPerfilAcesso).AsNoTracking()
                                                .Where(fun => fun.fun_codigo > intCodigo)
                                                .OrderBy(fun => fun.fun_codigo).FirstOrDefault();
                if (enDirecao == enNavegacao.Anterior)
                    objFuncionario = _objCtx.tbFuncionario
                                                .Include(pac => pac.tbPerfilAcesso).AsNoTracking()
                                                .Where(fun => fun.fun_codigo < intCodigo)
                                                .OrderByDescending(fun => fun.fun_codigo).FirstOrDefault();
                if (objFuncionario != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objFuncionario;
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

        public Retorno RetornaListaFuncionario(string strCodigo, string strNome, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                var arrFuncionario = _objCtx.tbFuncionario.AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(strCodigo))
                {
                    int intCodigo = Convert.ToInt32(strCodigo);
                    arrFuncionario = arrFuncionario.Where(fun => fun.fun_codigo == intCodigo).AsQueryable();
                }
                if (!string.IsNullOrWhiteSpace(strNome))
                    arrFuncionario = arrFuncionario.Where(fun => fun.fun_nome.Contains(strNome)).AsQueryable();
                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrFuncionario.Count();
                objRetorno.objRetorno = arrFuncionario.OrderByDescending(fun => fun.fun_codigo).Skip(intSkip).Take(intTake).ToList();
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

        public Retorno SalvarFuncionario(tbFuncionario objFuncionario, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaFuncionario(objFuncionario);
            try
            {
                if (strValidacao == string.Empty)
                {
                    objFuncionario.tbPerfilAcesso = null;
                    enOperacao enTipoOperacao;
                    if (objFuncionario.fun_codigo == 0)
                    {
                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbFuncionario.Add(objFuncionario);
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objFuncionarioContexto = _objCtx.tbFuncionario.FirstOrDefault(fun => fun.fun_codigo == objFuncionario.fun_codigo);
                        _objCtx.Entry(objFuncionarioContexto).CurrentValues.SetValues(objFuncionario);
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objFuncionario.fun_codigo, enTipoOperacao, objFuncionario, intFunCodigo);
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

        public Retorno ExcluirFuncionario(int intCodigo)
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
                            var objFuncionario = objContexto.tbFuncionario.FirstOrDefault(fun => fun.fun_codigo == intCodigo);
                            if (objFuncionario != null)
                            {
                                //Tenta excluir o perfil no contexto isolado
                                objContexto.tbFuncionario.Remove(objFuncionario);
                                objContexto.SaveChanges();
                                transacao.Commit();

                                objRetorno.intCodigoErro = 0;
                                objRetorno.objRetorno = true;
                            }
                            else
                            {
                                objRetorno.intCodigoErro = 48;
                                objRetorno.strMsgErro = "Funcionário não encontrado para exclusão";
                            }
                        }
                        catch (Exception)
                        {
                            //Se deu erro é porque o perfil tem  registros relacionado
                            transacao.Rollback();
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = "Funcionário não pode ser excluido pois há registros relacionados ao mesmo";
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

        private string ValidaFuncionario(tbFuncionario objFuncionario)
        {
            if (string.IsNullOrEmpty(objFuncionario.fun_nome) || string.IsNullOrWhiteSpace(objFuncionario.fun_nome))
                return "O nome deve ser informado.";

            return _objCtx.tbFuncionario.AsNoTracking().Any(fun => (fun.fun_nome.Equals(objFuncionario.fun_nome)) && fun.fun_codigo != objFuncionario.fun_codigo) ? "Já existe funcionário com esse nome." : string.Empty;
        }

        public Retorno AutenticaFuncionario(string strLogin, string strSenha)
        {
            var objRetorno = new Retorno();
            try
            {
                var objConfigStorage = new ConfigStorage();

                objConfigStorage.objConfiguracao = _objCtx.tbConfiguracao.AsNoTracking().FirstOrDefault();
                //objConfigStorage.objConfiguracao = _objCtx.Database.Connection.Query<tbConfiguracao>("Select * from tbConfiguracao").FirstOrDefault();

                //objConfigStorage.objFuncionario = strSenha == objConfigStorage.objConfiguracao.cfg_senhaMaster ? _objCtx.tbFuncionario.AsNoTracking().FirstOrDefault(fun => fun.fun_login == strLogin) : _objCtx.tbFuncionario.AsNoTracking().FirstOrDefault(fun => fun.fun_login == strLogin && fun.fun_senha == strSenha);
                if (strSenha == objConfigStorage.objConfiguracao.cfg_senhaMaster)
                {
                    //objConfigStorage.objFuncionario = _objCtx.tbFuncionario.AsNoTracking().FirstOrDefault(fun => fun.fun_login == strLogin);
                    objConfigStorage.objFuncionario = _objCtx.Database.Connection.Query<tbFuncionario>("select * from tbFuncionario where fun_login = '" + strLogin + "'").FirstOrDefault();
                }
                else
                {
                    //objConfigStorage.objFuncionario = _objCtx.tbFuncionario.AsNoTracking().FirstOrDefault(fun => fun.fun_login == strLogin && fun.fun_senha == strSenha);
                    objConfigStorage.objFuncionario = _objCtx.Database.Connection.Query<tbFuncionario>("select * from tbFuncionario where fun_login = '" + strLogin + "' and fun_senha = '" + strSenha + "'").FirstOrDefault();
                }
                
                if (objConfigStorage.objFuncionario != null)
                {
                    //objConfigStorage.objPerfilAcesso = _objCtx.tbPerfilAcesso
                    //    .AsNoTracking()
                    //    .Include(pem => pem.tbPerfilAcessoMenu.Select(men => men.tbMenu))
                    //    .FirstOrDefault(per => per.pac_codigo == objConfigStorage.objFuncionario.pac_codigo);

                    objConfigStorage.objPerfilAcesso = _objCtx.Database.Connection.Query<tbPerfilAcesso>("select * from tbPerfilAcesso where pac_codigo = " + objConfigStorage.objFuncionario.pac_codigo).FirstOrDefault();
                    string strQuery = "select *" +
                                    " from tbPerfilAcessoMenu pam inner join tbMenu men on pam.men_codigo = men.men_codigo" +
                                    " where pam.pac_codigo = " + objConfigStorage.objFuncionario.pac_codigo;
                    objConfigStorage.objPerfilAcesso.tbPerfilAcessoMenu = _objCtx.Database.Connection.Query<tbPerfilAcessoMenu, tbMenu, tbPerfilAcessoMenu>(strQuery, (objPerfilAcessoMenu, objMenu) =>
                    {
                        objPerfilAcessoMenu.tbMenu = objMenu;
                        return objPerfilAcessoMenu;
                    }, splitOn: "men_codigo").ToList();


                    //objConfigStorage.objEmpresa = _objCtx.tbEmpresa.AsNoTracking().FirstOrDefault();
                    objConfigStorage.objEmpresa = _objCtx.Database.Connection.Query<tbEmpresa>("select * from tbEmpresa").FirstOrDefault();

                    objConfigStorage.objConfiguracao.cfg_ultimoLogin = DateTime.Now;
                    _objCtx.Entry(objConfigStorage.objConfiguracao).State = EntityState.Modified;
                    _objCtx.SaveChanges();

                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objConfigStorage;
                    //using (var objBll = new Caixa())
                    //{
                    //    objRetorno = objBll.RetornaCaixa(0);
                    //}
                    //if (objRetorno.intCodigoErro != 16)
                    //{
                    //    if (objRetorno.objRetorno != null)
                    //        objConfigStorage.intCaiCodigo = ((tbCaixa)objRetorno.objRetorno).cai_codigo;

                    //    objConfigStorage.objConfiguracao.cfg_ultimoLogin = DateTime.Now;
                    //    _objCtx.Entry(objConfigStorage.objConfiguracao).State = EntityState.Modified;
                    //    _objCtx.SaveChanges();

                    //    objRetorno.intCodigoErro = 0;
                    //    objRetorno.objRetorno = objConfigStorage;
                    //}
                }
                else
                {
                    objRetorno.intCodigoErro = 48;
                    objRetorno.strMsgErro = "Login ou senha inválidos";
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

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
