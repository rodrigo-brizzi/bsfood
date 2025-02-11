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
    public class Pedidos : IPedidos, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public Pedidos()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public Pedidos(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        #region Entrega

        public Retorno RetornaPedido(int intCodigo, enNavegacao? enDirecao, enOrigemPedido enOrigem)
        {
            var objRetorno = new Retorno();
            try
            {
                var strOrigem = ((char)enOrigem).ToString();
                tbPedido objPedido = null;
                if (enDirecao == null)
                    objPedido = _objCtx.tbPedido.AsNoTracking().Include(ppr => ppr.tbPedidoProduto.Select(pro => pro.tbProduto))
                                                      .Include(cli => cli.tbCliente)
                                                      .Include(cen => cen.tbCliente.tbClienteEndereco.Select(bai => bai.tbBairro))
                                                      .Include(ctl => ctl.tbCliente.tbClienteTelefone)
                                                      .Include(fun => fun.tbFuncionarioEntregador)
                                                      .Include(fpg => fpg.tbFormaPagamento)
                                                      .FirstOrDefault(ped => ped.ped_codigo == intCodigo && ped.ped_origem == strOrigem);
                if (enDirecao == enNavegacao.Proximo)
                    objPedido = _objCtx.tbPedido.AsNoTracking().Include(ppr => ppr.tbPedidoProduto.Select(pro => pro.tbProduto))
                                                      .Include(cli => cli.tbCliente)
                                                      .Include(cen => cen.tbCliente.tbClienteEndereco.Select(bai => bai.tbBairro))
                                                      .Include(ctl => ctl.tbCliente.tbClienteTelefone)
                                                      .Include(fun => fun.tbFuncionarioEntregador)
                                                      .Include(fpg => fpg.tbFormaPagamento)
                                                      .Where(ped => ped.ped_codigo > intCodigo && ped.ped_origem == strOrigem)
                                                      .OrderBy(ped => ped.ped_codigo).FirstOrDefault();
                if (enDirecao == enNavegacao.Anterior)
                    objPedido = _objCtx.tbPedido.AsNoTracking().Include(ppr => ppr.tbPedidoProduto.Select(pro => pro.tbProduto))
                                                      .Include(cli => cli.tbCliente)
                                                      .Include(cen => cen.tbCliente.tbClienteEndereco.Select(bai => bai.tbBairro))
                                                      .Include(ctl => ctl.tbCliente.tbClienteTelefone)
                                                      .Include(fun => fun.tbFuncionarioEntregador)
                                                      .Include(fpg => fpg.tbFormaPagamento)
                                                      .Where(ped => ped.ped_codigo < intCodigo && ped.ped_origem == strOrigem)
                                                      .OrderByDescending(ped => ped.ped_codigo).FirstOrDefault();
                if (objPedido != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objPedido;
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

        public Retorno RetornaListaPedidoEntrega(bool blnProducao, bool blnEntrega, bool blnFinalizado, bool blnExcluido, int intFunCodigo, int intCaiCodigo, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                List<int> arrCaiCodigo = new List<int>();
                if (intCaiCodigo > 0)
                    arrCaiCodigo.Add(intCaiCodigo);
                else
                {
                    arrCaiCodigo = _objCtx.tbCaixa.AsNoTracking()
                                                    .Where(cai => cai.cai_dataFechamento == null)
                                                    .Select(cai => cai.cai_codigo)
                                                    .ToList();
                }

                var arrPedido = _objCtx.tbPedido.Include(fun => fun.tbFuncionarioEntregador)
                                                    .Include(ppr => ppr.tbPedidoProduto)
                                                    .Include(fpg => fpg.tbFormaPagamento).AsNoTracking()
                                                    .Where(ped => ped.ped_origem == "E" && arrCaiCodigo.Contains(ped.cai_codigo)).AsQueryable();
                if (!blnProducao)
                    arrPedido = arrPedido.Where(ped => ped.ped_status != "P").AsQueryable();
                if (!blnEntrega)
                    arrPedido = arrPedido.Where(ped => ped.ped_status != "E").AsQueryable();
                if (!blnFinalizado)
                    arrPedido = arrPedido.Where(ped => ped.ped_status != "F").AsQueryable();
                if (!blnExcluido)
                    arrPedido = arrPedido.Where(ped => ped.ped_status != "X").AsQueryable();
                if (intFunCodigo > 0)
                    arrPedido = arrPedido.Where(ped => ped.fun_funcionarioEntregador == intFunCodigo).AsQueryable();

                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrPedido.Count();
                objRetorno.objRetorno = arrPedido.OrderBy(ped => ped.ped_codigo).Skip(intSkip).Take(intTake).ToList();
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

        public Retorno SalvarPedidoEntrega(tbPedido objPedido, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaPedidoEntrega(objPedido);
            using (var objContexto = new EFContexto())
            {
                using (var transacao = objContexto.Database.BeginTransaction())
                {
                    try
                    {
                        if (strValidacao == string.Empty)
                        {
                            //Prepara o cliente
                            objPedido.tbCliente.tbClienteGrupo = null;
                            foreach (var objClienteEndereco in objPedido.tbCliente.tbClienteEndereco)
                            {
                                objClienteEndereco.tbBairro = null;
                                objClienteEndereco.tbCliente = null;
                            }
                            foreach (var objClienteTelefone in objPedido.tbCliente.tbClienteTelefone)
                                objClienteTelefone.tbCliente = null;
                            //Atualiza o cliente
                            if (objPedido.tbCliente.cli_codigo == 0)
                            {
                                objContexto.tbCliente.Add(objPedido.tbCliente);
                                objContexto.SaveChanges();
                                objPedido.cli_codigo = objPedido.tbCliente.cli_codigo;
                            }
                            else
                            {
                                var objClienteContexto = objContexto.tbCliente.Include(cen => cen.tbClienteEndereco)
                                                                          .Include(ctl => ctl.tbClienteTelefone)
                                                                          .FirstOrDefault(cli => cli.cli_codigo == objPedido.tbCliente.cli_codigo);
                                if (objClienteContexto != null)
                                {
                                    objContexto.tbClienteEndereco.RemoveRange(objClienteContexto.tbClienteEndereco);
                                    objContexto.tbClienteTelefone.RemoveRange(objClienteContexto.tbClienteTelefone);
                                    objContexto.Entry(objClienteContexto).CurrentValues.SetValues(objPedido.tbCliente);
                                }

                                foreach (var objItemEndereco in objPedido.tbCliente.tbClienteEndereco)
                                {
                                    objItemEndereco.cli_codigo = objPedido.tbCliente.cli_codigo;
                                    objContexto.tbClienteEndereco.Add(objItemEndereco);
                                }
                                foreach (var objItemTelefone in objPedido.tbCliente.tbClienteTelefone)
                                {
                                    objItemTelefone.cli_codigo = objPedido.tbCliente.cli_codigo;
                                    objContexto.tbClienteTelefone.Add(objItemTelefone);
                                }
                            }

                            //Prepara o pedido
                            objPedido.tbCliente = null;
                            objPedido.tbFuncionarioEntregador = null;
                            objPedido.tbFormaPagamento = null;
                            foreach (var objPedidoProduto in objPedido.tbPedidoProduto)
                            {
                                objPedidoProduto.tbProduto = null;
                                objPedidoProduto.tbPedido = null;
                            }

                            enOperacao enTipoOperacao;
                            if (objPedido.ped_codigo == 0)
                            {
                                enTipoOperacao = enOperacao.Inclusao;

                                objPedido.ped_data = DateTime.Now;
                                objPedido.ped_origem = "E"; //"E" = Entrega, "C" = Comanda
                                objPedido.ped_status = enStatusPedido.P.ToString(); //"P" = Produção, "E" = Entrega, "F" = Finalizado, "X" = Excluido
                                if (objPedido.fun_funcionarioEntregador != null)
                                {
                                    objPedido.ped_status = enStatusPedido.E.ToString();
                                    objPedido.ped_dataEntrega = DateTime.Now;
                                }
                                objPedido.fun_codigo = intFunCodigo;
                                if (objPedido.ped_cobranca == enFormaCobranca.P.ToString())//"P"
                                    objPedido.ped_observacao = (string.IsNullOrWhiteSpace(objPedido.ped_observacao) ? "" : Environment.NewLine) + "**PEDIDO CONVENIO - NAO COBRAR**";

                                //Atualizando a ordem de pedido
                                var objCaixaContexto = objContexto.tbCaixa.FirstOrDefault(cai => cai.cai_codigo == objPedido.cai_codigo);
                                if (objCaixaContexto != null)
                                {
                                    objCaixaContexto.cai_ordemPedido++;
                                    objPedido.ped_ordem = objCaixaContexto.cai_ordemPedido;
                                    objContexto.Entry(objCaixaContexto).State = EntityState.Modified;
                                }
                                objContexto.tbPedido.Add(objPedido);
                            }
                            else
                            {
                                enTipoOperacao = enOperacao.Alteracao;

                                var objPedidoContexto = objContexto.tbPedido.Include(ppr => ppr.tbPedidoProduto).FirstOrDefault(ped => ped.ped_codigo == objPedido.ped_codigo);

                                if (objPedidoContexto != null && (objPedidoContexto.fun_funcionarioEntregador == null && objPedido.fun_funcionarioEntregador != null))
                                {
                                    objPedido.ped_status = enStatusPedido.E.ToString();//"E";
                                    objPedido.ped_dataEntrega = DateTime.Now;
                                }

                                if (objPedidoContexto != null)
                                {
                                    objContexto.tbPedidoProduto.RemoveRange(objPedidoContexto.tbPedidoProduto);
                                    objContexto.Entry(objPedidoContexto).CurrentValues.SetValues(objPedido);
                                }

                                foreach (var objItemProduto in objPedido.tbPedidoProduto)
                                {
                                    objItemProduto.ped_codigo = objPedido.ped_codigo;
                                    objContexto.tbPedidoProduto.Add(objItemProduto);
                                }
                            }
                            objContexto.SaveChanges();
                            transacao.Commit();
                            using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                                objBll.SalvarAuditoria(objPedido.ped_codigo, enTipoOperacao, objPedido, intFunCodigo);
                            objRetorno = RetornaPedido(objPedido.ped_codigo, null, enOrigemPedido.Entrega);
                        }
                        else
                        {
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = strValidacao;
                        }
                    }
                    catch (Exception ex)
                    {
                        transacao.Rollback();
                        Util.LogErro(ex);
                        objRetorno.intCodigoErro = 16;
                        objRetorno.strMsgErro = ex.Message;
                        objRetorno.strExceptionToString = ex.ToString();
                    }
                }
            }
            return objRetorno;
        }

        public Retorno SalvarPedido(tbPedido objPedido, enOrigemPedido enOrigem, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaPedido(objPedido, enOrigem);
            try
            {
                if (strValidacao == string.Empty)
                {
                    objPedido.tbFuncionarioEntregador = null;
                    objPedido.tbCliente = null;
                    objPedido.tbFormaPagamento = null;
                    foreach (var objPedidoProduto in objPedido.tbPedidoProduto)
                    {
                        objPedidoProduto.tbProduto = null;
                        objPedidoProduto.tbPedido = null;
                    }
                    enOperacao enTipoOperacao;
                    if (objPedido.ped_codigo == 0)
                    {
                        objPedido.ped_data = DateTime.Now;
                        objPedido.ped_origem = ((char)enOrigem).ToString(); //"E" = Entrega, "C" = Comanda
                        objPedido.ped_status = enStatusPedido.P.ToString(); //"P" = Produção, "E" = Entrega, "F" = Finalizado, "X" = Excluido
                        if (objPedido.fun_funcionarioEntregador != null)
                        {
                            objPedido.ped_status = enStatusPedido.E.ToString();
                            objPedido.ped_dataEntrega = DateTime.Now;
                        }
                        objPedido.fun_codigo = Util.objConfigStorage.objFuncionario.fun_codigo;
                        objPedido.cai_codigo = Util.objConfigStorage.intCaiCodigo;
                        if (objPedido.ped_cobranca == enFormaCobranca.P.ToString())//"P"
                            objPedido.ped_observacao = (string.IsNullOrWhiteSpace(objPedido.ped_observacao) ? "" : Environment.NewLine) + "**PEDIDO CONVENIO - NAO COBRAR**";

                        if (enOrigem == enOrigemPedido.Entrega)
                        {
                            var objCaixaContexto = _objCtx.tbCaixa.FirstOrDefault(cai => cai.cai_codigo == objPedido.cai_codigo);
                            if (objCaixaContexto != null)
                            {
                                objCaixaContexto.cai_ordemPedido++;
                                objPedido.ped_ordem = objCaixaContexto.cai_ordemPedido;
                                _objCtx.Entry(objCaixaContexto).State = EntityState.Modified;
                            }
                        }

                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbPedido.Add(objPedido);
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objPedidoContexto = _objCtx.tbPedido.Include(ppr => ppr.tbPedidoProduto).FirstOrDefault(ped => ped.ped_codigo == objPedido.ped_codigo);

                        if (objPedidoContexto != null && (objPedidoContexto.fun_funcionarioEntregador == null && objPedido.fun_funcionarioEntregador != null))
                        {
                            objPedido.ped_status = enStatusPedido.E.ToString();//"E";
                            objPedido.ped_dataEntrega = DateTime.Now;
                        }

                        if (objPedidoContexto != null)
                        {
                            _objCtx.tbPedidoProduto.RemoveRange(objPedidoContexto.tbPedidoProduto);
                            _objCtx.Entry(objPedidoContexto).CurrentValues.SetValues(objPedido);
                        }

                        foreach (var objItemProduto in objPedido.tbPedidoProduto)
                        {
                            objItemProduto.ped_codigo = objPedido.ped_codigo;
                            _objCtx.tbPedidoProduto.Add(objItemProduto);
                        }
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objPedido.ped_codigo, enTipoOperacao, objPedido, intFunCodigo);
                    objRetorno = RetornaPedido(objPedido.ped_codigo, null, enOrigem);
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

        public Retorno ExcluirPedido(int intCodigo, string strMotivoCancelamento, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                var objPedido = _objCtx.tbPedido.FirstOrDefault(ped => ped.ped_codigo == intCodigo);
                if (objPedido != null)
                {
                    //Verificar se o caixa que refere o pedido está aberto, se não estiver, não permitir o fechamento
                    var objCaixa = _objCtx.tbCaixa.FirstOrDefault(cai => cai.cai_codigo == objPedido.cai_codigo && cai.cai_dataFechamento == null);
                    if (objCaixa != null)
                    {
                        objPedido.ped_status = enStatusPedido.X.ToString();//"X";
                        objPedido.ped_motivoCancelamento = strMotivoCancelamento;
                        _objCtx.Entry(objPedido).State = EntityState.Modified;
                        _objCtx.SaveChanges();
                        using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                            objBll.SalvarAuditoria(objPedido.ped_codigo, enOperacao.Exclusao, objPedido, intFunCodigo);

                        objRetorno.intCodigoErro = 0;
                        objRetorno.objRetorno = true;
                    }
                    else
                    {
                        objRetorno.intCodigoErro = 48;
                        objRetorno.strMsgErro = "O caixa referente ao pedido está fechado!";
                    }
                }
                else
                {
                    objRetorno.intCodigoErro = 48;
                    objRetorno.strMsgErro = "Pedido não encontrado para exclusão";
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

        public Retorno SalvarEntregador(tbPedido objPedido, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                objPedido.tbFuncionarioEntregador = null;
                objPedido.tbPedidoProduto = null;
                objPedido.tbFormaPagamento = null;
                objPedido.ped_status = enStatusPedido.E.ToString();//"E";
                objPedido.ped_dataEntrega = DateTime.Now;
                var objPedidoContexto = _objCtx.tbPedido.FirstOrDefault(ped => ped.ped_codigo == objPedido.ped_codigo);
                _objCtx.Entry(objPedidoContexto).CurrentValues.SetValues(objPedido);
                _objCtx.SaveChanges();
                using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                    objBll.SalvarAuditoria(objPedido.ped_codigo, enOperacao.Outro, objPedido, intFunCodigo);
                objRetorno = RetornaPedido(objPedido.ped_codigo, null, enOrigemPedido.Entrega);
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

        private string ValidaPedido(tbPedido objPedido, enOrigemPedido enOrigem)
        {
            if (enOrigem == enOrigemPedido.Entrega)
            {
                if (objPedido.cli_codigo == 0)
                    return "O cliente do Pedido deve ser informado.";

                if (objPedido.tbPedidoProduto.Count == 0)
                    return "Não foram informados nenhum produto.";

                if (objPedido.fpg_codigo == 0)
                    return "A forma de pagamento deve ser informada.";
            }
            return string.Empty;
        }

        private string ValidaPedidoEntrega(tbPedido objPedido)
        {
            if (objPedido.tbCliente.tbClienteEndereco.Count == 0)
                return "Não foram informados nenhum endereço.";

            if (objPedido.tbPedidoProduto.Count == 0)
                return "Não foram informados nenhum produto.";

            if (objPedido.fpg_codigo == 0)
                return "A forma de pagamento deve ser informada.";
            return string.Empty;
        }

        #endregion Entrega


        #region Comanda

        public Retorno RetornaPedidoComanda(int intNumero)
        {
            var objRetorno = new Retorno();
            try
            {
                tbMesa objMesa = null;
                objMesa = _objCtx.tbMesa.AsNoTracking().Include(ped => ped.tbPedido.tbFuncionario).FirstOrDefault(mes => mes.mes_numero == intNumero);
                if (objMesa != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objMesa;
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

        public Retorno SalvarPedidoComanda(tbPedido objPedido, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaPedidoMesa(objPedido);
            try
            {
                if (strValidacao == string.Empty)
                {
                    objPedido.tbFuncionario = null;
                    foreach (var objPedidoProduto in objPedido.tbPedidoProduto)
                    {
                        objPedidoProduto.tbProduto = null;
                        objPedidoProduto.tbPedido = null;
                    }
                    enOperacao enTipoOperacao;
                    if (objPedido.ped_codigo == 0)
                    {
                        objPedido.ped_data = DateTime.Now;
                        objPedido.ped_origem = "C"; //"E" = Entrega, "C" = Comanda
                        objPedido.ped_status = "P"; //"P" = Produção, "E" = Entrega, "F" = Finalizado, "X" = Excluido                       
                        enTipoOperacao = enOperacao.Inclusao;
                        _objCtx.tbPedido.Add(objPedido);
                        _objCtx.SaveChanges();
                        var objMesaContexto = _objCtx.tbMesa.FirstOrDefault(mes => mes.mes_numero == objPedido.ped_numeroMesa);
                        objMesaContexto.ped_codigo = objPedido.ped_codigo;
                        objMesaContexto.mes_status = "O";
                        _objCtx.Entry(objMesaContexto).State = EntityState.Modified;
                    }
                    else
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objPedidoContexto = _objCtx.tbPedido.FirstOrDefault(ped => ped.ped_codigo == objPedido.ped_codigo);
                        if (objPedidoContexto != null)
                        {
                            //Inclui os produtos que vieram na coleção sem codigo
                            foreach (var objItem in objPedido.tbPedidoProduto.Where(ppr => ppr.ppr_codigo == 0))
                            {
                                objItem.ped_codigo = objPedido.ped_codigo;
                                _objCtx.tbPedidoProduto.Add(objItem);
                            }

                            //Atualiza o pedido
                            _objCtx.Entry(objPedidoContexto).CurrentValues.SetValues(objPedido);
                        }
                    }
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objPedido.ped_codigo, enTipoOperacao, objPedido, intFunCodigo);
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = _objCtx.tbPedido.AsNoTracking().Include(fun => fun.tbFuncionario).Include(ppr => ppr.tbPedidoProduto)
                        .FirstOrDefault(ped => ped.ped_codigo == objPedido.ped_codigo);
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

        #endregion Comanda


        #region Mesa

        public Retorno RetornaListaMesa(int? intNumero, bool blnLivre = true, bool blnOcupado = true, string strTerminal = "")
        {
            var objRetorno = new Retorno();
            try
            {
                if(!string.IsNullOrWhiteSpace(strTerminal))
                {
                    //Limpa a seleção da mesa no banco de dados
                    _objCtx.tbMesa.Where(mes => mes.mes_terminal == strTerminal).ToList().ForEach(mes =>
                    {
                        mes.mes_terminal = null;
                    });
                    _objCtx.SaveChanges();
                }

                var query = _objCtx.tbMesa.AsNoTracking().OrderBy(mes => mes.mes_numero).AsQueryable();
                if (intNumero != null && intNumero > 0)
                    query = query.Where(mes => mes.mes_numero == intNumero).AsQueryable();
                if (!blnLivre)
                    query = query.Where(mes => mes.mes_status != enStatusMesa.L.ToString()).AsQueryable();
                if (!blnOcupado)
                    query = query.Where(ped => ped.mes_status != enStatusMesa.O.ToString()).AsQueryable();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = query.OrderBy(mes => mes.mes_numero).ToList();
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

        public Retorno RetornaPedidoMesa(int intNumero, string strTerminal)
        {
            var objRetorno = new Retorno();
            try
            {
                tbMesa objMesa = null;
                objMesa = _objCtx.tbMesa.AsNoTracking().Include(ped => ped.tbPedido.tbFuncionario)
                                                       .Include(ped => ped.tbPedido.tbFormaPagamento)
                                                       .Include(ped => ped.tbPedido.tbPedidoProduto.Select(pro => pro.tbProduto))
                                                       .FirstOrDefault(mes => mes.mes_numero == intNumero);
                if (objMesa != null)
                {
                    //Seleciona a mesa no banco de dados

                    var objMesaContexto = _objCtx.tbMesa.FirstOrDefault(mes => mes.mes_terminal == strTerminal);
                    if(objMesaContexto != null && objMesaContexto.mes_numero != intNumero)
                    {
                        objMesaContexto.mes_terminal = null;
                        _objCtx.Entry(objMesaContexto).State = EntityState.Modified;
                    }

                    objMesa.mes_terminal = strTerminal;
                    _objCtx.Entry(objMesa).State = EntityState.Modified;
                    _objCtx.SaveChanges();

                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objMesa;
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

        public Retorno FecharPedidoMesa(tbPedido objPedido)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaPedidoMesa(objPedido);
            try
            {
                if (strValidacao == string.Empty)
                {
                    objPedido.tbFuncionario = null;
                    objPedido.tbFormaPagamento = null;
                    foreach (var objPedidoProduto in objPedido.tbPedidoProduto)
                    {
                        objPedidoProduto.tbProduto = null;
                        objPedidoProduto.tbPedido = null;
                    }
                    enOperacao enTipoOperacao;
                    if (objPedido.ped_codigo > 0)
                    {
                        enTipoOperacao = enOperacao.Alteracao;
                        var objPedidoContexto = _objCtx.tbPedido.Include(ppr => ppr.tbPedidoProduto).FirstOrDefault(ped => ped.ped_codigo == objPedido.ped_codigo);
                        //Remover produtos que não vieram na coleçao
                        var arrPsgrCodigo = objPedido.tbPedidoProduto.Select(ppr => ppr.ppr_codigo).ToArray();
                        if (objPedidoContexto != null)
                        {
                            _objCtx.tbPedidoProduto.RemoveRange(objPedidoContexto.tbPedidoProduto.Where(ppr => !arrPsgrCodigo.Contains(ppr.ppr_codigo)));

                            //Alterar os produtos que vieram na coleção
                            foreach (var objPedidoProdutoContexto in objPedidoContexto.tbPedidoProduto.Where(ppr => arrPsgrCodigo.Contains(ppr.ppr_codigo)))
                                _objCtx.Entry(objPedidoProdutoContexto).CurrentValues.SetValues(objPedido.tbPedidoProduto.FirstOrDefault(ppr => ppr.ppr_codigo == objPedidoProdutoContexto.ppr_codigo));

                            //Inclui os produtos que vieram na coleção sem codigo
                            foreach (var objItem in objPedido.tbPedidoProduto.Where(ppr => ppr.ppr_codigo == 0))
                            {
                                objItem.ped_codigo = objPedido.ped_codigo;
                                _objCtx.tbPedidoProduto.Add(objItem);
                            }

                            objPedido.ped_status = "F";

                            //Atualiza o pedido
                            _objCtx.Entry(objPedidoContexto).CurrentValues.SetValues(objPedido);
                        }
                        _objCtx.SaveChanges();

                        using (var objContexto = new EFContexto())
                        {
                            var objMesaContexto = objContexto.tbMesa.FirstOrDefault(mes => mes.mes_numero == objPedido.ped_numeroMesa);
                            if (objMesaContexto != null)
                            {
                                objMesaContexto.mes_status = "L";
                                objMesaContexto.ped_codigo = null;
                                objMesaContexto.tbPedido = null;
                                objContexto.Entry(objMesaContexto).State = EntityState.Modified;
                                objContexto.SaveChanges();
                            }
                        }
                        
                        using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                            objBll.SalvarAuditoria(objPedido.ped_codigo, enTipoOperacao, objPedido, objPedido.fun_codigo);
                        objRetorno.intCodigoErro = 0;
                    }
                    else
                    {
                        objRetorno.intCodigoErro = 48;
                        objRetorno.strMsgErro = "Mesa não pode ser fechada pois a mesa não foi aberta pela comanda!";
                    }
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

        private string ValidaPedidoMesa(tbPedido objPedido)
        {
            if (objPedido.tbPedidoProduto.Count == 0)
                return "Não foram informados nenhum produto.";

            if (objPedido.fun_codigo == 0)
                return "O funcionário atendente deve ser informado.";
            return string.Empty;
        }

        #endregion Mesa

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}