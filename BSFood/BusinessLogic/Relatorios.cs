﻿using BSFood.DataAccess;
using BSFood.DataTransfer;
using BSFood.Apoio;
using BSFood.BusinessLogic.Interfaces;
using BSFood.Models;
using System;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace BSFood.BusinessLogic
{
    public class Relatorios : IRelatorios, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public Relatorios()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public Relatorios(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }
        
        #region Cupons

        public Retorno RetornaCupomEntrega(tbPedido objPedido, int? intPedCodigo = null)
        {
            var objRetorno = new Retorno();
            try
            {
                var sbConteudo = new StringBuilder();
                if (intPedCodigo != null)
                    objPedido = _objCtx.tbPedido.AsNoTracking().FirstOrDefault(ped => ped.ped_codigo == intPedCodigo);

                sbConteudo.Append("        COMPROVANTE DE ENTREGA" + Environment.NewLine);
                sbConteudo.Append(" " + Environment.NewLine);
                if (objPedido != null)
                {
                    sbConteudo.Append("PEDIDO..: " + objPedido.ped_codigo.ToString("0000000000") + new string(' ', 5) + " ORDEM: " + objPedido.ped_ordem.ToString() + Environment.NewLine);
                    sbConteudo.Append("CLIENTE.: " + objPedido.ped_nomeCliente.PadRight(30) + Environment.NewLine);
                    sbConteudo.Append((objPedido.ped_logradouro + "," + objPedido.ped_numero).PadRight(30) + Environment.NewLine);
                    if (!string.IsNullOrWhiteSpace(objPedido.ped_complemento))
                        sbConteudo.Append(objPedido.ped_complemento.PadRight(30) + Environment.NewLine);
                    sbConteudo.Append("BAIRRO..: " + objPedido.ped_bairro.PadRight(30) + Environment.NewLine);
                    sbConteudo.Append("TELEFONE: " + objPedido.ped_telefone.PadRight(30) + Environment.NewLine);
                    sbConteudo.Append(Environment.NewLine);
                    sbConteudo.Append("QTD PRODUTO               VALOR   TOTAL" + Environment.NewLine);
                    sbConteudo.Append("---------------------------------------" + Environment.NewLine);
                    foreach (var objPedidoProduto in objPedido.tbPedidoProduto)
                    {
                        var strQuantidade = objPedidoProduto.ppr_quantidade.ToString(objPedidoProduto.ppr_quantidade < 1 ? "0.0" : "000");
                        sbConteudo.Append(strQuantidade + " " +
                                          objPedidoProduto.ppr_descricao.PadRight(19, ' ').Substring(0, 19) + " " +
                                          objPedidoProduto.ppr_valorUnitario.ToString("#,##0.00").PadLeft(7) + " " +
                                          objPedidoProduto.ppr_valorTotal.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    }
                    sbConteudo.Append("              TOTAL PRODUTOS -> " + objPedido.ped_valorSubTotal.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    sbConteudo.Append("                                -------" + Environment.NewLine);
                    if (objPedido.ped_valorTaxaEntrega > 0 || objPedido.ped_valorDespesa > 0 || objPedido.ped_valorDesconto > 0)
                    {
                        if (objPedido.ped_valorTaxaEntrega > 0)
                            sbConteudo.Append("                 TX. ENTREGA -> " + objPedido.ped_valorTaxaEntrega.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                        if (objPedido.ped_valorDespesa > 0)
                            sbConteudo.Append("                    DESPESAS -> " + objPedido.ped_valorDespesa.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                        if (objPedido.ped_valorDesconto > 0)
                            sbConteudo.Append("                    DESCONTO -> " + objPedido.ped_valorDesconto.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                        sbConteudo.Append("             TOTAL DO PEDIDO -> " + objPedido.ped_valorTotal.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    }
                    sbConteudo.Append("              VALOR RECEBIDO -> " + objPedido.ped_valorRecebido.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    sbConteudo.Append("                       TROCO -> " + objPedido.ped_valorTroco.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    sbConteudo.Append("FORMA PAGTO: " + objPedido.tbFormaPagamento.fpg_descricao);
                    sbConteudo.Append("OBSERVACOES: " + objPedido.ped_observacao);
                }

                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = sbConteudo.ToString();
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

        public Retorno RetornaCaixaFechamento(tbCaixa objCaixa)
        {
            var objRetorno = new Retorno();
            try
            {
                //if (objCaixa.cai_dataFechamento != null)
                //{
                var arrPedidoVista = _objCtx.tbPedido.AsNoTracking().Where(ped => ped.cai_codigo == objCaixa.cai_codigo &&
                                                                                ped.tbFormaPagamento.fpg_cobranca == "V").ToList();
                decimal decTotalVista = 0;
                if (arrPedidoVista.Count > 0)
                    decTotalVista = arrPedidoVista.Sum(ped => ped.ped_valorTotal);


                var arrPedidoPrazo = _objCtx.tbPedido.AsNoTracking().Where(ped => ped.cai_codigo == objCaixa.cai_codigo &&
                                                                                ped.tbFormaPagamento.fpg_cobranca == "P").ToList();
                decimal decTotalPrazo = 0;
                if (arrPedidoPrazo.Count > 0)
                    decTotalPrazo = arrPedidoPrazo.Sum(ped => ped.ped_valorTotal);


                var arrCaixaMovimentoSaida = _objCtx.tbCaixaMovimento.Include(caio => caio.tbCaixaOperacao).AsNoTracking()
                    .Where(caim => caim.tbCaixaOperacao.caio_tipoOperacao == "S" && caim.cai_codigo == objCaixa.cai_codigo).ToList();

                decimal decTotalSaida = 0;
                if(arrCaixaMovimentoSaida.Count > 0)
                    decTotalSaida = arrCaixaMovimentoSaida.Sum(caim => caim.caim_valor);


                var arrCaixaMovimentoAbertura = _objCtx.tbCaixaMovimento.Include(fpg => fpg.tbFormaPagamento).AsNoTracking()
                    .Where(caim => caim.caio_codigo == 1 && caim.cai_codigo == objCaixa.cai_codigo).ToList();

                decimal decTotalAbertura = 0;
                if (arrCaixaMovimentoAbertura.Count > 0)
                    decTotalAbertura = arrCaixaMovimentoAbertura.Sum(caim => caim.caim_valor);


                var arrPedidoEntrega = (from ped in _objCtx.tbPedido
                                            .AsNoTracking()
                                            .Where(ped => ped.cai_codigo == objCaixa.cai_codigo &&
                                            ped.ped_origem == "E" &&
                                            (ped.ped_status == "F" || ped.ped_status == "E"))
                                        group ped by new
                                        {
                                            codigo = ped.fun_funcionarioEntregador,
                                            nome = ped.tbFuncionarioEntregador.fun_nome
                                        }
                                            into grupo
                                            select new
                                            {
                                                grupo.Key.codigo, 
                                                grupo.Key.nome,
                                                decValorTotal = grupo.Sum(ped => ped.ped_valorTotal)
                                            }).ToList();
                decimal decTotalEntrega = 0;
                if (arrPedidoEntrega.Count > 0)
                    decTotalEntrega = arrPedidoEntrega.Sum(ped => ped.decValorTotal);


                var arrPedidoComanda = _objCtx.tbPedido
                                                .AsNoTracking()
                                                .Where(ped => ped.cai_codigo == objCaixa.cai_codigo &&
                                                ped.ped_origem == "C" && ped.ped_status == "F").ToList();
                decimal decTotalComanda = 0;
                if (arrPedidoComanda.Count > 0)
                    decTotalComanda = arrPedidoComanda.Sum(ped => ped.ped_valorTotal);

                var arrPedidoExcluido = _objCtx.tbPedido
                                            .AsNoTracking()
                                            .Where(ped => ped.cai_codigo == objCaixa.cai_codigo &&
                                            ped.ped_status == "X").ToList();
                decimal decTotalExcluido = 0;
                if (arrPedidoExcluido.Count > 0)
                    decTotalExcluido = arrPedidoExcluido.Sum(ped => ped.ped_valorTotal);

                decimal decTotalRecebimento = 0;

                var arrPedidoForma = (from ped in _objCtx.tbPedido
                                            .AsNoTracking()
                                            .Where(ped => ped.cai_codigo == objCaixa.cai_codigo)
                                      group ped by new
                                      {
                                          codigo = ped.fpg_codigo,
                                          descricao = ped.tbFormaPagamento.fpg_descricao,
                                          cobranca = ped.tbFormaPagamento.fpg_cobranca
                                      }
                                          into grupo
                                          select new
                                          {
                                              grupo.Key.codigo, 
                                              grupo.Key.descricao, 
                                              grupo.Key.cobranca,
                                              decValorTotalForma = grupo.Sum(ped => ped.ped_valorTotal)
                                          }).ToList();
                decimal decTotalForma = 0;
                if (arrPedidoForma.Count > 0)
                    decTotalForma = arrPedidoForma.Sum(ped => ped.decValorTotalForma);

                var sbRelatorio = new StringBuilder();
                sbRelatorio.Append("          FECHAMENTO DE CAIXA          " + Environment.NewLine);
                sbRelatorio.Append(" " + Environment.NewLine);
                if (objCaixa.cai_dataAbertura != null)
                    sbRelatorio.Append("Data da abertura..: " + objCaixa.cai_dataAbertura.Value.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine);
                if (objCaixa.cai_dataFechamento != null)
                    sbRelatorio.Append("Data do fechamento: " + objCaixa.cai_dataFechamento.Value.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine);
                sbRelatorio.Append("Data da impressao.: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + Environment.NewLine);
                sbRelatorio.Append(" " + Environment.NewLine);
                sbRelatorio.Append("             RESUMO PEDIDOS            " + Environment.NewLine);
                sbRelatorio.Append(" " + Environment.NewLine);
                sbRelatorio.Append("TOTAL ABERTURA.............: " + decTotalAbertura.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("TOTAL A VISTA..............: " + decTotalVista.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("TOTAL A PRAZO..............: " + decTotalPrazo.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("TOTAL SAIDAS...............: " + decTotalSaida.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("TOTAL......................: " + ((decTotalAbertura + decTotalVista + decTotalPrazo) - decTotalSaida).ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("                ABERTURA               " + Environment.NewLine);
                foreach (var objCaixaMovimentoAbertura in arrCaixaMovimentoAbertura)
                {
                    sbRelatorio.Append(objCaixaMovimentoAbertura.tbFormaPagamento.fpg_descricao.PadRight(28).Substring(0, 28) + " " +
                        objCaixaMovimentoAbertura.caim_valor.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                }
                sbRelatorio.Append("TOTAL......................: " + decTotalAbertura.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("                A VISTA                " + Environment.NewLine);
                sbRelatorio.Append("ENTREGAS...................: " + decTotalEntrega.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                foreach (var objEntregador in arrPedidoEntrega)
                {
                    sbRelatorio.Append("  " +
                        (objEntregador.codigo.ToString() + " " + objEntregador.nome).PadRight(15).Substring(0, 15) + " " +
                        (objEntregador.decValorTotal * 14 / 100).ToString("#,##0.00").PadLeft(10) + " " +
                        objEntregador.decValorTotal.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                }
                sbRelatorio.Append("COMANDAS...................: " + decTotalComanda.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("RECEBIMENTOS...............: " + decTotalRecebimento.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("                A PRAZO                " + Environment.NewLine);
                foreach (var objPedidoPrazo in arrPedidoPrazo)
                {
                    sbRelatorio.Append(objPedidoPrazo.ped_nomeCliente.PadRight(28).Substring(0, 28) + " " +
                        objPedidoPrazo.ped_valorTotal.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                }
                sbRelatorio.Append("TOTAL......................: " + decTotalPrazo.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("                SAIDAS                 " + Environment.NewLine);
                foreach (var objCaixaMovimento in arrCaixaMovimentoSaida)
                {
                    string strDescricao = objCaixaMovimento.tbCaixaOperacao.caio_descricao + "-" + objCaixaMovimento.caim_observacao;
                    sbRelatorio.Append(strDescricao.PadRight(28).Substring(0, 28) + " " +
                        objCaixaMovimento.caim_valor.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                }
                sbRelatorio.Append("TOTAL......................: " + decTotalSaida.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("               EXCLUIDO                " + Environment.NewLine);
                foreach (var objPedidoExcluido in arrPedidoExcluido)
                {
                    if (objPedidoExcluido.ped_nomeCliente != null)
                    {
                        sbRelatorio.Append(objPedidoExcluido.ped_nomeCliente.PadRight(28).Substring(0, 28) + " " +
                            objPedidoExcluido.ped_valorTotal.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                    }
                }
                sbRelatorio.Append("TOTAL......................: " + decTotalExcluido.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("RESUMO DE PEDIDOS POR GRUPO DE PRODUTO " + Environment.NewLine);
                //sbRelatorio.Append("C/ CLB.....................: " + Environment.NewLine);
                //sbRelatorio.Append("TOTAL......................: " + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("         REGISTRO DE ALTERACAO         " + Environment.NewLine);
                //sbRelatorio.Append("M FULANO DE VA            VD           " + Environment.NewLine);
                //sbRelatorio.Append("E MARIA DA  VA            VD           " + Environment.NewLine);
                sbRelatorio.Append("---------------------------------------" + Environment.NewLine);
                sbRelatorio.Append("          FORMAS DE PAGAMENTO          " + Environment.NewLine);
                foreach (var objPedidoForma in arrPedidoForma)
                {
                    if (objPedidoForma.descricao != null)
                    {
                        sbRelatorio.Append((objPedidoForma.descricao + "(" + objPedidoForma.cobranca + ")").PadRight(28).Substring(0, 28) + " " +
                            objPedidoForma.decValorTotalForma.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);
                    }
                }
                sbRelatorio.Append("TOTAL......................: " + decTotalForma.ToString("#,##0.00").PadLeft(10) + Environment.NewLine);

                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = sbRelatorio.ToString();
                //}
                //else
                //{
                //    objRetorno.intCodigoErro = 0;
                //    objRetorno.objRetorno = string.Empty;
                //}
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

        public Retorno RetornaTicketConferencia(tbPedido objPedido, int? intPedCodigo = null)
        {
            var objRetorno = new Retorno();
            try
            {
                var sbConteudo = new StringBuilder();
                if (intPedCodigo != null)
                    objPedido = _objCtx.tbPedido.AsNoTracking().FirstOrDefault(ped => ped.ped_codigo == intPedCodigo);

                sbConteudo.Append("         TICKET DE CONFERENCIA" + Environment.NewLine);
                sbConteudo.Append(" " + Environment.NewLine);
                sbConteudo.Append("QTD PRODUTO               VALOR   TOTAL" + Environment.NewLine);
                sbConteudo.Append("---------------------------------------" + Environment.NewLine);
                if (objPedido != null)
                {
                    foreach (var objPedidoProduto in objPedido.tbPedidoProduto)
                    {
                        var strQuantidade = objPedidoProduto.ppr_quantidade.ToString(objPedidoProduto.ppr_quantidade < 1 ? "0.0" : "000");
                        sbConteudo.Append(strQuantidade + " " +
                                          objPedidoProduto.ppr_descricao.PadRight(19, ' ').Substring(0, 19) + " " +
                                          objPedidoProduto.ppr_valorUnitario.ToString("#,##0.00").PadLeft(7) + " " +
                                          objPedidoProduto.ppr_valorTotal.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    }
                    sbConteudo.Append("                    SUBTOTAL -> " + objPedido.ped_valorSubTotal.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    sbConteudo.Append("                                -------" + Environment.NewLine);
                    if (objPedido.ped_valorTaxaEntrega > 0 || objPedido.ped_valorDespesa > 0 || objPedido.ped_valorDesconto > 0)
                    {
                        if (objPedido.ped_valorTaxaEntrega > 0)
                            sbConteudo.Append("                 TX. ENTREGA -> " + objPedido.ped_valorTaxaEntrega.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                        if (objPedido.ped_valorDespesa > 0)
                            sbConteudo.Append("                    DESPESAS -> " + objPedido.ped_valorDespesa.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                        if (objPedido.ped_valorDesconto > 0)
                            sbConteudo.Append("                    DESCONTO -> " + objPedido.ped_valorDesconto.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                        sbConteudo.Append("                       TOTAL -> " + objPedido.ped_valorTotal.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    }
                    sbConteudo.Append("                    RECEBIDO -> " + objPedido.ped_valorRecebido.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    sbConteudo.Append("                       TROCO -> " + objPedido.ped_valorTroco.ToString("#,##0.00").PadLeft(7) + Environment.NewLine);
                    sbConteudo.Append("OBSERVACOES: " + objPedido.ped_observacao + Environment.NewLine);
                }

                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = sbConteudo.ToString();
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

        public Retorno RetornaCupomComanda(tbPedido objPedido)
        {
            var objRetorno = new Retorno();
            try
            {
                var sbConteudo = new StringBuilder();
                if (objPedido != null)
                {
                    sbConteudo.Append("COMANDA MESA NRO: " + objPedido.ped_numeroMesa.ToString("000") + Environment.NewLine);
                    sbConteudo.Append("QTD DESCRICAO                 " + Environment.NewLine);
                    sbConteudo.Append("------------------------------" + Environment.NewLine);
                    foreach (var objPedidoProduto in objPedido.tbPedidoProduto
                        .Where(ppr => ppr.ppr_impresso == false)
                        .OrderBy(ppr => ppr.ppr_codigo))
                    {
                        var strQuantidade = objPedidoProduto.ppr_quantidade.ToString(objPedidoProduto.ppr_quantidade < 1 ? "0.0" : "000");
                        sbConteudo.Append(strQuantidade + " " +
                                          objPedidoProduto.ppr_descricao.PadRight(26, ' ').Substring(0, 26) + Environment.NewLine);

                        var objPedidoProdutoContexto = _objCtx.tbPedidoProduto.FirstOrDefault(ppr => ppr.ppr_codigo == objPedidoProduto.ppr_codigo);
                        objPedidoProdutoContexto.ppr_impresso = true;
                        _objCtx.Entry(objPedidoProdutoContexto).State = EntityState.Modified;
                        _objCtx.SaveChanges();
                    }
                    sbConteudo.Append("OBS.: " + objPedido.ped_observacao + Environment.NewLine);
                    sbConteudo.Append("ATENDENTE: " + objPedido.tbFuncionario.fun_nome);
                }
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = sbConteudo.ToString();
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

        #endregion



        #region Pedidos

        public Retorno RetornaPedidoPorEntregador(DateTime dtDataInicial, DateTime dtDataFinal, int? intFunCodigo, int intCaiCodigo, string strEmpresa)
        {
            var objRetorno = new Retorno();
            try
            {
                var dtIni = new DateTime(dtDataInicial.Year, dtDataInicial.Month, dtDataInicial.Day, 0, 0, 0);
                var dtFim = new DateTime(dtDataFinal.Year, dtDataFinal.Month, dtDataFinal.Day, 23, 59, 59);
                
                const string strTitulo = "RELATORIO DE PEDIDOS POR ENTREGADOR";
                var sbConteudo = new StringBuilder();
                var objQuery = _objCtx.tbPedido.AsNoTracking().Include(fun => fun.tbFuncionarioEntregador)
                                                        .Where(ped => ped.ped_origem=="E" &&
                                                            (ped.ped_status == "F" || ped.ped_status == "E"))
                                                      .AsQueryable();
                objQuery = intCaiCodigo > 0 
                    ? objQuery.Where(cai => cai.cai_codigo == intCaiCodigo) 
                    : objQuery.Where(ped => ped.ped_data >= dtIni && ped.ped_data <= dtFim);

                if (intFunCodigo != null)
                    objQuery = objQuery.Where(fun => fun.fun_funcionarioEntregador == intFunCodigo);

                var arrPedido = objQuery.OrderBy(ped => ped.fun_funcionarioEntregador).ThenBy(ped => ped.ped_codigo).ToList();

                if(arrPedido.Count > 0)
                {
                    var strFiltro = intCaiCodigo > 0
                        ? "Caixa: " + intCaiCodigo.ToString()
                        : dtDataInicial.ToShortDateString() + "&nbsp;à&nbsp;" + dtDataFinal.ToShortDateString();
                    sbConteudo.Append(RetornaCabecalho(strEmpresa, strTitulo, strFiltro, 1, 1));
                    int? intFunCodigoAux = 0;
                    decimal decTotal = 0;
                    arrPedido.ForEach(fun =>
                    {
                        if (intFunCodigoAux != fun.fun_funcionarioEntregador)
                        {
                            if (intFunCodigoAux == 0)
                                sbConteudo.Append("<table border=\"0\" width=\"100%\">" + Environment.NewLine);
                            else
                                sbConteudo.Append(string.Format("<tr><td colspan=\"6\" align=\"right\">Total: R$ {0}</td></tr>" + Environment.NewLine, decTotal.ToString("0.00")));
                            sbConteudo.Append(string.Format("<tr><td>Funcionário:</td><td>{0}&nbsp;-&nbsp;{1}</td><td>Quantidade:</td><td>{2}</td><td>&nbsp;</td><td>&nbsp;</td></tr>" + Environment.NewLine, 
                                fun.fun_funcionarioEntregador, 
                                fun.tbFuncionarioEntregador.fun_nome, 
                                arrPedido.Count(ped => ped.fun_funcionarioEntregador == fun.fun_funcionarioEntregador).ToString()));
                            sbConteudo.Append("<tr><th align=\"left\">PEDIDO</th><th align=\"left\">CLIENTE</th><th align=\"left\">DATA</th><th align=\"left\">CAIXA</th><th align=\"left\">ORDEM</th><th align=\"right\">VALOR</th></tr>" + Environment.NewLine);
                            intFunCodigoAux = fun.fun_funcionarioEntregador;
                            decTotal = 0;
                        }
                        if (fun.ped_data != null)
                            sbConteudo.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td align=\"right\">R$ {5}</td></tr>" + Environment.NewLine, 
                                fun.ped_codigo,
                                fun.cli_codigo + "&nbsp;-&nbsp;" + fun.ped_nomeCliente,
                                fun.ped_data.Value.ToShortDateString(),
                                fun.cai_codigo.ToString(),
                                fun.ped_ordem,
                                fun.ped_valorTotal.ToString("0.00")));
                        decTotal += fun.ped_valorTotal;
                    });
                    sbConteudo.Append(string.Format("<tr><td colspan=\"6\" align=\"right\">Total: R$ {0}</td></tr>" + Environment.NewLine, decTotal.ToString("0.00")));
                    sbConteudo.Append("</table>" + Environment.NewLine);
                    sbConteudo.Append(RetornaRodape());

                    var objAssembly = Assembly.GetExecutingAssembly();
                    var strTemplate = "&nbsp;";
                    using (var stream = objAssembly.GetManifestResourceStream("BSFood.Dependencia.RelTemplate.html"))
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream, Encoding.GetEncoding(CultureInfo.GetCultureInfo("pt-BR").TextInfo.ANSICodePage)))
                            {
                                strTemplate = reader.ReadToEnd();
                            }
                    }
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = string.Format(strTemplate, strTitulo, sbConteudo);
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

        #endregion Pedidos



        #region Apoio

        private string RetornaCabecalho(string strEmpresa, string strTitulo, string strFiltro, int intPaginaAtual, int intPaginaTotal)
        {
            var sbCabecalho = new StringBuilder();
            sbCabecalho.Append("<table width=\"800\" align=\"center\" border=\"0\">" + Environment.NewLine);
            sbCabecalho.Append("<tr>" + Environment.NewLine);
            sbCabecalho.Append("    <td width=\"80\" rowspan=\"2\"><img src=\"{0}\" width=\"64\" height=\"64\" /></td>" + Environment.NewLine);
            sbCabecalho.Append("    <td width=\"550\"><strong>{1}</strong></td>" + Environment.NewLine);
            sbCabecalho.Append("    <td width=\"170\" align=\"right\" rowspan=\"2\" valign=\"top\">Página:{2}/{3}</td>" + Environment.NewLine);
            sbCabecalho.Append("</tr>" + Environment.NewLine);
            sbCabecalho.Append("<tr>" + Environment.NewLine);
            sbCabecalho.Append("    <td valign=\"top\"><strong>{4}</strong></td>" + Environment.NewLine);
            sbCabecalho.Append("</tr>" + Environment.NewLine);
            sbCabecalho.Append("<tr>" + Environment.NewLine);
            sbCabecalho.Append("    <td colspan=\"3\"><hr /></td>" + Environment.NewLine);
            sbCabecalho.Append("</tr>" + Environment.NewLine);
            sbCabecalho.Append("<tr>" + Environment.NewLine);
            sbCabecalho.Append("    <td colspan=\"3\">" + Environment.NewLine);
            return string.Format(sbCabecalho.ToString(), 
                AppDomain.CurrentDomain.BaseDirectory + "logo" + Util.objConfigStorage.objEmpresa.emp_logoFormato, 
                strEmpresa, intPaginaAtual.ToString(), 
                intPaginaTotal.ToString(), 
                strTitulo + "<br />" + strFiltro);
        }

        private string RetornaRodape()
        {
            var sbRodape = new StringBuilder();
            sbRodape.Append("    </td>" + Environment.NewLine);
            sbRodape.Append("</tr>" + Environment.NewLine);
            sbRodape.Append("<tr>" + Environment.NewLine);
            sbRodape.Append("    <td colspan=\"3\"><hr /></td>" + Environment.NewLine);
            sbRodape.Append("</tr>" + Environment.NewLine);
            sbRodape.Append("<tr>" + Environment.NewLine);
            sbRodape.Append("    <td colspan=\"2\">www.brizzisoft.com.br</td>" + Environment.NewLine);
            sbRodape.Append("    <td align=\"right\">{0}</td>" + Environment.NewLine);
            sbRodape.Append("</tr>" + Environment.NewLine);
            sbRodape.Append("</table>" + Environment.NewLine);
            return string.Format(sbRodape.ToString(), DateTime.Now);
        }

        #endregion Apoio

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
