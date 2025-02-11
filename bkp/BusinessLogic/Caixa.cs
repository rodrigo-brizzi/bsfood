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
    public class Caixa : ICaixa, IDisposable
    {
        private readonly bool _blnFecharCon;
        private EFContexto _objCtx;
        private GerenciaTransacao _objTransacao;

        public Caixa()
        {
            _objCtx = new EFContexto();
            _objTransacao = new GerenciaTransacao(ref _objCtx);
            _blnFecharCon = true;
        }

        public Caixa(ref EFContexto objCtx, ref GerenciaTransacao objTransacao)
        {
            _objCtx = objCtx;
            _objTransacao = objTransacao;
            _blnFecharCon = false;
        }

        public Retorno RetornaCaixa(int intCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                tbCaixa objCaixa;
                if (intCodigo > 0)
                {
                    objCaixa = _objCtx.tbCaixa.AsNoTracking()
                                                      .Include(cai => cai.tbCaixaMovimento)
                                                      .Include(cai => cai.tbCaixaMovimento.Select(fpg => fpg.tbFormaPagamento))
                                                      .FirstOrDefault(cai => cai.cai_codigo == intCodigo);
                }
                else
                {
                    objCaixa = _objCtx.tbCaixa.AsNoTracking().Include(cai => cai.tbCaixaMovimento)
                                                      .Include(cai => cai.tbCaixaMovimento.Select(fpg => fpg.tbFormaPagamento))
                                                      .Where(cai => cai.cai_dataFechamento == null)
                                                      .OrderByDescending(cai => cai.cai_codigo)
                                                      .FirstOrDefault();
                }
                if (objCaixa != null)
                {
                    objRetorno.intCodigoErro = 0;
                    objRetorno.objRetorno = objCaixa;
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

        public Retorno RetornaListaCaixa(string strCodigo, string strFuncionario, string strStatus, string strDataAbertura, int intSkip, int intTake)
        {
            var objRetorno = new Retorno();
            try
            {
                var arrCaixa = _objCtx.tbCaixa.Include(fun => fun.tbFuncionario).AsNoTracking().AsQueryable();
                if (!string.IsNullOrWhiteSpace(strCodigo))
                {
                    int intCodigo = Convert.ToInt32(strCodigo);
                    arrCaixa = arrCaixa.Where(cai => cai.cai_codigo == intCodigo).AsQueryable();
                }

                if (!string.IsNullOrWhiteSpace(strFuncionario))
                {
                    arrCaixa = arrCaixa.Where(cai => cai.tbFuncionario.fun_nome.Contains(strFuncionario)).AsQueryable();
                }

                if (!string.IsNullOrWhiteSpace(strStatus))
                {
                    switch (strStatus)
                    {
                        case "A":
                            arrCaixa = arrCaixa.Where(cai => cai.cai_dataFechamento == null).AsQueryable();
                            break;
                        case "F":
                            arrCaixa = arrCaixa.Where(cai => cai.cai_dataFechamento != null).AsQueryable();
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(strDataAbertura))
                {
                    DateTime dtData = new DateTime(Convert.ToDateTime(strDataAbertura).Year,
                        Convert.ToDateTime(strDataAbertura).Month,
                        Convert.ToDateTime(strDataAbertura).Day, 0, 0, 0);

                    DateTime dtDataFim = new DateTime(Convert.ToDateTime(strDataAbertura).Year,
                        Convert.ToDateTime(strDataAbertura).Month,
                        Convert.ToDateTime(strDataAbertura).Day, 23, 59, 59);
                    arrCaixa = arrCaixa.Where(cai => cai.cai_dataAbertura >= dtData && cai.cai_dataAbertura <= dtDataFim).AsQueryable();
                }

                objRetorno.intCodigoErro = 0;
                if (intSkip == 0)
                    objRetorno.intQtdeRegistro = arrCaixa.Count();
                objRetorno.objRetorno = arrCaixa.OrderBy(cai => cai.cai_codigo).Skip(intSkip).Take(intTake).ToList();
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

        public Retorno RetornaListaCaixaAberto()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrCaixa = _objCtx.tbCaixa.Include(Func => Func.tbFuncionario).AsNoTracking()
                                                    .Where(cai => cai.cai_dataFechamento == null)
                                                    .OrderBy(cai => cai.cai_codigo)
                                                    .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrCaixa;
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

        public Retorno RetornaListaCaixaOperacao()
        {
            var objRetorno = new Retorno();
            try
            {
                var arrCaixaOperacao = _objCtx.tbCaixaOperacao.AsNoTracking()
                                                      .OrderBy(caio => caio.caio_descricao)
                                                      .ToList();
                objRetorno.intCodigoErro = 0;
                objRetorno.objRetorno = arrCaixaOperacao;
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

        public Retorno AbrirCaixa(tbCaixa objCaixa, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaAberturaCaixa(objCaixa);
            try
            {
                if (strValidacao == string.Empty)
                {
                    objCaixa.tbFuncionario = null;
                    foreach (var objCaixaMovimento in objCaixa.tbCaixaMovimento)
                    {
                        if (objCaixaMovimento.caim_data == null)
                            objCaixaMovimento.caim_data = DateTime.Now;
                        objCaixaMovimento.caio_codigo = 1;//ABERTURA DE CAIXA
                        objCaixaMovimento.tbCaixa = null;
                        objCaixaMovimento.tbCaixaOperacao = null;
                        objCaixaMovimento.tbFormaPagamento = null;
                    }

                    objCaixa.cai_dataAbertura = DateTime.Now;
                    _objCtx.tbCaixa.Add(objCaixa);
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objCaixa.cai_codigo, enOperacao.Inclusao, objCaixa, intFunCodigo);
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

        public Retorno LancarMovimentoCaixa(tbCaixaMovimento objCaixaMovimento, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            var strValidacao = ValidaMovimentoCaixa(objCaixaMovimento);
            try
            {
                if (strValidacao == string.Empty)
                {
                    if (objCaixaMovimento.caim_data == null)
                        objCaixaMovimento.caim_data = DateTime.Now;
                    objCaixaMovimento.tbCaixa = null;
                    objCaixaMovimento.tbCaixaOperacao = null;
                    objCaixaMovimento.tbFormaPagamento = null;

                    _objCtx.tbCaixaMovimento.Add(objCaixaMovimento);
                    _objCtx.SaveChanges();
                    using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                        objBll.SalvarAuditoria(objCaixaMovimento.caim_codigo, enOperacao.Inclusao, objCaixaMovimento, intFunCodigo);
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

        //public Retorno AbrirCaixa(tbCaixa objCaixa, int intFunCodigo)
        //{
        //    var objRetorno = new Retorno();
        //    var strValidacao = ValidaAberturaCaixa(objCaixa);
        //    try
        //    {
        //        if (strValidacao == string.Empty)
        //        {
        //            foreach (var objCaixaMovimento in objCaixa.tbCaixaMovimento)
        //            {
        //                if (objCaixaMovimento.caim_data == null)
        //                    objCaixaMovimento.caim_data = DateTime.Now;
        //                objCaixaMovimento.caio_codigo = 1;//ABERTURA DE CAIXA
        //                objCaixaMovimento.tbCaixa = null;
        //                objCaixaMovimento.tbCaixaOperacao = null;
        //                objCaixaMovimento.tbFormaPagamento = null;
        //            }
        //            enOperacao enTipoOperacao;
        //            if (objCaixa.cai_codigo == 0)
        //            {
        //                objCaixa.cai_dataAbertura = DateTime.Now;
        //                objCaixa.fun_codigo = Util.objConfigStorage.objFuncionario.fun_codigo;
        //                enTipoOperacao = enOperacao.Inclusao;
        //                _objCtx.tbCaixa.Add(objCaixa);
        //            }
        //            else
        //            {
        //                enTipoOperacao = enOperacao.Alteracao;
        //                var objCaixaContexto = _objCtx.tbCaixa.Include(caim => caim.tbCaixaMovimento).FirstOrDefault(cai => cai.cai_codigo == objCaixa.cai_codigo);
        //                //Remover movimento de caixa que não vieram na coleçao
        //                var arrCaimCodigo = objCaixa.tbCaixaMovimento.Select(caim => caim.caim_codigo).ToArray();
        //                if (objCaixaContexto != null)
        //                {
        //                    _objCtx.tbCaixaMovimento.RemoveRange(objCaixaContexto.tbCaixaMovimento.Where(caim => !arrCaimCodigo.Contains(caim.caim_codigo)));

        //                    //Alterar os movimentos de caixa que vieram na coleção
        //                    foreach (var objCaixaMovimentoContexto in objCaixaContexto.tbCaixaMovimento.Where(caim => arrCaimCodigo.Contains(caim.caim_codigo)))
        //                        _objCtx.Entry(objCaixaMovimentoContexto).CurrentValues.SetValues(objCaixa.tbCaixaMovimento.FirstOrDefault(caim => caim.caim_codigo == objCaixaMovimentoContexto.caim_codigo));

        //                    //Inclui os movimentos de caixa que vieram na coleção sem codigo
        //                    foreach (var objItem in objCaixa.tbCaixaMovimento.Where(caim => caim.caim_codigo == 0))
        //                    {
        //                        objItem.caim_codigo = objCaixa.cai_codigo;
        //                        _objCtx.tbCaixaMovimento.Add(objItem);
        //                    }

        //                    //Atualiza o caixa
        //                    _objCtx.Entry(objCaixaContexto).CurrentValues.SetValues(objCaixa);
        //                }
        //            }
        //            _objCtx.SaveChanges();
        //            using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
        //                objBll.SalvarAuditoria(objCaixa.cai_codigo, enTipoOperacao, objCaixa, intFunCodigo);
        //            objRetorno = RetornaCaixa(objCaixa.cai_codigo);
        //        }
        //        else
        //        {
        //            objRetorno.intCodigoErro = 48;
        //            objRetorno.strMsgErro = strValidacao;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Util.LogErro(ex);
        //        objRetorno.intCodigoErro = 16;
        //        objRetorno.strMsgErro = ex.Message;
        //        objRetorno.strExceptionToString = ex.ToString();
        //    }
        //    return objRetorno;
        //}

        public Retorno RetornaCaixaFechamento(int intCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                tbCaixa objCaixa;
                if (intCodigo > 0)
                {
                    objCaixa = _objCtx.tbCaixa.AsNoTracking().Include(cai => cai.tbCaixaMovimento)
                                                      .Include(cai => cai.tbFuncionario).Include(cai => cai.tbCaixaMovimento.Select(fpg => fpg.tbFormaPagamento))
                                                      .FirstOrDefault(cai => cai.cai_codigo == intCodigo);
                }
                else
                {
                    objCaixa = _objCtx.tbCaixa.AsNoTracking().Include(cai => cai.tbCaixaMovimento)
                                                      .Include(cai => cai.tbFuncionario)
                                                      .Include(cai => cai.tbCaixaMovimento.Select(fpg => fpg.tbFormaPagamento))
                                                      .OrderByDescending(cai => cai.cai_codigo)
                                                      .FirstOrDefault();
                }
                if (objCaixa != null)
                {
                    var objFechamentoCaixa = new FechamentoCaixa
                    {
                        objCaixa = objCaixa,
                        arrFechamentoCaixaFormaEntrega = (from ped in _objCtx.tbPedido
                            .AsNoTracking()
                            .Where(ped => ped.ped_status != "X" &&
                                          ped.ped_origem == "E" &&
                                          ped.cai_codigo == objCaixa.cai_codigo)
                            group ped by new
                            {
                                codigo = ped.fpg_codigo,
                                descricao = ped.tbFormaPagamento.fpg_descricao
                            }
                            into grupo
                            select new FechamentoCaixaForma
                            {
                                fpg_codigo = grupo.Key.codigo,
                                fpg_descricao = grupo.Key.descricao,
                                decValorTotalForma = grupo.Sum(ped => ped.ped_valorTotal)
                            }).ToList(),
                        arrFechamentoCaixaFormaComanda = (from ped in _objCtx.tbPedido
                            .AsNoTracking()
                            .Where(ped => ped.ped_status == "F" &&
                                          ped.ped_origem == "C" &&
                                          ped.cai_codigo == objCaixa.cai_codigo)
                            group ped by new
                            {
                                codigo = ped.fpg_codigo,
                                descricao = ped.tbFormaPagamento.fpg_descricao
                            }
                            into grupo
                            select new FechamentoCaixaForma
                            {
                                fpg_codigo = grupo.Key.codigo,
                                fpg_descricao = grupo.Key.descricao,
                                decValorTotalForma = grupo.Sum(ped => ped.ped_valorTotal)
                            }).ToList()
                    };

                    //Formas de pagamento dos pedidos de entrega

                    //Formas de pagamento dos pedidos de comanda

                    objFechamentoCaixa.strStatusCaixa = objCaixa.cai_dataFechamento == null ? enStatusCaixa.Aberto.ToString() : enStatusCaixa.Fechado.ToString();

                    using (var objBll = new Relatorios())
                    {
                        objRetorno = objBll.RetornaCaixaFechamento(objCaixa);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        objFechamentoCaixa.strRelatorio = objRetorno.objRetorno as string;
                        objRetorno.objRetorno = objFechamentoCaixa;
                    }
                    #region base
                    //    GlobalRetorno objRetorno = new GlobalRetorno();
                    //    try
                    //    {
                    //        //se for informado código zero e direcao nula, retorna o ultimo caixa movimento
                    //        if (intCodigoCaixa == 0 && enDirecao == null)
                    //        {
                    //            if (blnPermiteVerFechamentoTodosCaixas == true)
                    //                intCodigoCaixa = this.objCtx.tbCaixa.Max(cai => cai.cai_codigo);
                    //            else
                    //                intCodigoCaixa = this.objCtx.tbCaixa.Where(cai => cai.pes_codigo == intCodigoUsuario).Max(cai => cai.cai_codigo);
                    //        }

                    //        var query = this.objCtx.tbCaixa.AsNoTracking().Include(cai => cai.tbPessoa)
                    //                                                      .Include(cai => cai.tbTerminal);

                    //        if (enDirecao == null)
                    //        {
                    //            query = query.Where(cai => cai.cai_codigo == intCodigoCaixa);
                    //        }
                    //        else if (enDirecao == enNavegacao.Anterior)
                    //        {
                    //            query = query.Where(cai => cai.cai_codigo < intCodigoCaixa).OrderByDescending(cai => cai.cai_codigo);
                    //        }
                    //        else if (enDirecao == enNavegacao.Proximo)
                    //        {
                    //            query = query.Where(cai => cai.cai_codigo > intCodigoCaixa).OrderBy(cai => cai.cai_codigo);
                    //        }

                    //        tbCaixa objCaixa = query.FirstOrDefault();

                    //        if (objCaixa != null)
                    //        {
                    //            //preciso checar se o caixa pertente ao usuario informado
                    //            if (blnPermiteVerFechamentoTodosCaixas != true)
                    //            {
                    //                if (objCaixa.pes_codigo != intCodigoUsuario)
                    //                    throw new GlobalException("Esse caixa pertente a outro usuário. É necessário permissão especial para visualizar o caixa de outro usuário");
                    //            }

                    //            intCodigoCaixa = objCaixa.cai_codigo;

                    //            FechamentoCaixa objFechamento = new FechamentoCaixa();

                    //            objFechamento.CodigoCaixa = objCaixa.cai_codigo;
                    //            objFechamento.DataAbertura = objCaixa.cai_dataAbertura;
                    //            objFechamento.DataFechamento = objCaixa.cai_dataFechamento;
                    //            objFechamento.CodigoFuncionario = objCaixa.tbPessoa.pes_codigo;
                    //            objFechamento.NomeFuncionario = objCaixa.tbPessoa.pes_nome;
                    //            objFechamento.CodigoTerminal = objCaixa.ter_codigo;
                    //            objFechamento.Terminal = objCaixa.tbTerminal.ter_nome;


                    //            objFechamento.ObservacaoFechamento = objCaixa.cai_observacaoFechamento;

                    //            //recuperar as vendas finalizadas(F) feitas no caixa incluindo os detalhes das formas de pagamento utilizadas
                    //            objFechamento.Vendas = this.objCtx.tbVendaCupom.AsNoTracking()
                    //                                                           .Include(vec => vec.tbVendaCupomDetalhe)
                    //                                                           .Include(vec => vec.tbVendaCupomDetalhe.Select(vecd => vecd.tbProduto))
                    //                                                           .Include(vec => vec.tbPessoaFuncionario)
                    //                                                           .Include(vec => vec.tbCaixaMovimento.Select(caim => caim.tbFormaPagamento.tbTipoFormaPagamento))
                    //                                                           .Where(vec => vec.cai_codigo == intCodigoCaixa && vec.vec_statusCupom == "F")
                    //                                                           .ToList();

                    //            // deixo apenas os movimentos da venda apenas do tipo de venda, pois a venda pode ter movimentos de caixa de troco e haver
                    //            foreach (tbVendaCupom objVendaCupom in objFechamento.Vendas)
                    //            {
                    //                objVendaCupom.tbCaixaMovimento = objVendaCupom.tbCaixaMovimento.Where(caim => caim.caio_codigo == (int)enCaixaOperacao.VendaCupom && caim.caim_cancelado == false).ToList();
                    //            }


                    //            //recuperar as vendas canceladas(C) no caixa
                    //            objFechamento.VendasCanceladas = this.objCtx.tbVendaCupom.AsNoTracking()
                    //                                                                     .Include(vec => vec.tbPessoaFuncionario)
                    //                                                                     .Where(vec => vec.cai_codigo == intCodigoCaixa && vec.vec_statusCupom == "C")
                    //                                                                     .ToList();

                    //            // deixo apenas os movimentos da venda apenas do tipo de venda, pois a venda pode ter movimentos de caixa de troco e haver
                    //            foreach (tbVendaCupom objVendaCupom in objFechamento.VendasCanceladas)
                    //            {
                    //                objVendaCupom.tbCaixaMovimento = objVendaCupom.tbCaixaMovimento.Where(caim => caim.caio_codigo == (int)enCaixaOperacao.VendaCupom).ToList();
                    //            }


                    //            //recuperar as sangrias incluindo os movimentos de caixas gerados pela sangria; inclui a forma de pagamento a nivel de exibição
                    //            objFechamento.Sangrias = this.objCtx.tbSangria.AsNoTracking()
                    //                                                          .Include(san => san.tbCaixaMovimento
                    //                                                          .Select(fpg => fpg.tbFormaPagamento))
                    //                                                          .Where(san => san.cai_codigo == intCodigoCaixa &&
                    //                                                                    (san.san_excluido == null || san.san_excluido == false)).ToList();

                    //            //recuperar os suprimentos, incluindo os movimentos de caixas gerados pelo suprimento; inclui a forma de pagamento a nivel de exibição
                    //            objFechamento.Suprimentos = this.objCtx.tbSuprimento.AsNoTracking()
                    //                                                                .Include(sup => sup.tbCaixaMovimento
                    //                                                                .Select(fpg => fpg.tbFormaPagamento))
                    //                                                                .Where(sup => sup.cai_codigo == intCodigoCaixa && sup.edi_codigo == null && sup.crd_codigo == null && sup.sup_excluido == false)
                    //                                                                .ToList();

                    //            ////recuperar os movimentos de caixa do tipo venda, agrupados pelo tipo da forma, somando o total de cada forma
                    //            int intCodigoOperacaoVenda = (int)enCaixaOperacao.VendaCupom;
                    //            objFechamento.FormasRecebidas = (from caim in this.objCtx.tbCaixaMovimento
                    //                                                                      .AsNoTracking()
                    //                                                                      .Where(caim => caim.cai_codigo == intCodigoCaixa &&
                    //                                                                                     caim.caim_cancelado == false &&
                    //                                                                                     (caim.caio_codigo == intCodigoOperacaoVenda ||
                    //                                                                                      caim.caio_codigo == (int)enCaixaOperacao.Troco
                    //                                                                                     )
                    //                                                                        )
                    //                                             group caim by new
                    //                                             {
                    //                                                 codigo = caim.fpg_codigo,
                    //                                                 descricao = caim.tbFormaPagamento.fpg_descricao
                    //                                             } into grupo
                    //                                             select new FormaFechamentoCaixa
                    //                                             {
                    //                                                 fpg_codigo = grupo.Key.codigo,
                    //                                                 fpg_descricao = grupo.Key.descricao,
                    //                                                 decValorTotalForma = grupo.Where(g => g.caio_codigo == (int)intCodigoOperacaoVenda).Sum(caim => caim.caim_valor)
                    //                                                                      -
                    //                                                                      (grupo.Where(g => g.caio_codigo == (int)enCaixaOperacao.Troco).Sum(caim => caim.caim_valor)
                    //                                                                      == null ? 0 : grupo.Where(g => g.caio_codigo == (int)enCaixaOperacao.Troco).Sum(caim => caim.caim_valor))
                    //                                             }).ToList();


                    //            //recuperar os haveres criados
                    //            int intCodigoOperacaoHaver = (int)enCaixaOperacao.GeracaoHaver;
                    //            objFechamento.HaverCriados = this.objCtx.tbCaixaMovimento
                    //                                                     .AsNoTracking()
                    //                                                     .Include(caim => caim.tbContaPagarHaverCriado)
                    //                                                     .Where(caim => caim.cai_codigo == intCodigoCaixa && caim.caio_codigo == intCodigoOperacaoHaver && caim.caim_cancelado == false)
                    //                                                     .Select(caim => caim.tbContaPagarHaverCriado)
                    //                                                     .ToList();

                    //            //recuperar cheque troco
                    //            int intCodigoOperacaoTroco = (int)enCaixaOperacao.Troco;
                    //            int intCodigoOperacaoChequeTroco = (int)enCaixaOperacao.ChequeTroco;
                    //            objFechamento.ChequeTroco = this.objCtx.tbCheque
                    //                                                   .AsNoTracking()
                    //                                                   .Include(chq => chq.tbChequeMovimentoLote)
                    //                                                   .Include(chq => chq.tbCaixaMovimento)
                    //                                                   .Where(chq => chq.tbCaixaMovimento.Where(caim => caim.cai_codigo == intCodigoCaixa && caim.caio_codigo == intCodigoOperacaoChequeTroco && caim.caim_cancelado == false).Count() > 0)
                    //                                                   .ToList();

                    //            //recuperar aferições
                    //            objFechamento.Afericoes = (from abap in this.objCtx.tbAbastecimentoPdv
                    //                                                            .Include(abap => abap.tbProduto)
                    //                                                            .AsNoTracking()
                    //                                                            .Where(abap => abap.abps_codigo == 4 && abap.cai_codigo == intCodigoCaixa)
                    //                                       group abap by new
                    //                                       {
                    //                                           codigo = abap.bco_codigo,
                    //                                           bico = abap.abap_bico,
                    //                                           produto = abap.tbProduto.pro_nome
                    //                                       } into grupo

                    //                                       select new Afericao
                    //                                       {
                    //                                           bco_codigo = grupo.Key.codigo,
                    //                                           bco_numeroBico = grupo.Key.bico,
                    //                                           pro_nome = grupo.Key.produto,
                    //                                           abap_litragem = grupo.Sum(abap => abap.abap_litragem)
                    //                                       }

                    //                                      ).ToList();

                    //            //recuperar vales emitidos pelo caixa
                    //            objFechamento.Vales = (from crec in this.objCtx.tbContaReceber
                    //                                   join pes in this.objCtx.tbPessoa on crec.pes_codigo equals pes.pes_codigo
                    //                                   join caim in this.objCtx.tbCaixaMovimento on crec.caim_codigo equals caim.caim_codigo
                    //                                   join cai in this.objCtx.tbCaixa on caim.cai_codigo equals cai.cai_codigo

                    //                                   where crec.olct_codigo == 16 && cai.cai_codigo == intCodigoCaixa && crec.emp_codigo == Util.objConfigStorage.emp_codigo

                    //                                   select new Vale
                    //                                   {
                    //                                       crec_codigo = crec.crec_codigo,
                    //                                       crec_documento = crec.crec_documento,
                    //                                       pes_nome = pes.pes_nome,
                    //                                       crec_dataLancamento = crec.crec_dataLancamento,
                    //                                       crec_valor = crec.crec_valor,
                    //                                       crec_descricao = crec.crec_descricao
                    //                                   }
                    //                                      ).ToList();

                    //            //recuperar contas a receber vindo do suprimento    
                    //            objFechamento.arrContasReceberPagas = (from sup in this.objCtx.tbSuprimento
                    //                                                   join crd in this.objCtx.tbContaReceberDetalhe on sup.crd_codigo equals crd.crd_codigo
                    //                                                   join crec in this.objCtx.tbContaReceber on crd.crec_codigo equals crec.crec_codigo
                    //                                                   join pes in this.objCtx.tbPessoa on crec.pes_codigo equals pes.pes_codigo
                    //                                                   where
                    //                                                       sup.cai_codigo == intCodigoCaixa
                    //                                                       &&
                    //                                                       sup.emp_codigo == Util.objConfigStorage.emp_codigo
                    //                                                       &&
                    //                                                       sup.sup_excluido == false
                    //                                                   select new ContaReceberPaga
                    //                                                    {
                    //                                                        crec_codigo = crec.crec_codigo,
                    //                                                        crec_documento = crec.crec_documento,
                    //                                                        crec_dataLancamento = crec.crec_dataLancamento,
                    //                                                        crec_valor = crec.crec_valor,
                    //                                                        crec_valorPago = crd.crd_valorPago + crd.crd_valorPagoJuros + crd.crd_valorPagoMulta + crd.crd_valorPagoTaxa,
                    //                                                        pes_nome = pes.pes_nome
                    //                                                    }
                    //                                                ).Distinct().ToList();

                    //            //recuperar os suprimentos que são as entradas diversas, incluindo os movimentos de caixas gerados pelo suprimento; inclui a forma de pagamento a nivel de exibição
                    //            objFechamento.EntradasDiversas = (from sup in this.objCtx.tbSuprimento
                    //                                              join edi in this.objCtx.tbEntradaDiversa on sup.edi_codigo equals edi.edi_codigo
                    //                                              join pes in this.objCtx.tbPessoa on sup.pes_codigoCliente equals pes.pes_codigo
                    //                                              where sup.cai_codigo == intCodigoCaixa && sup.sup_excluido == false
                    //                                              select new EntradaDiversa
                    //                                              {
                    //                                                  sup_codigo = sup.sup_codigo,
                    //                                                  pes_nome = pes.pes_nome,
                    //                                                  edi_descricao = edi.edi_descricao,
                    //                                                  sup_valor = sup.sup_valor
                    //                                              }).ToList();

                    //            objFechamento.EntradasDiversasEstornos = (from sup in this.objCtx.tbSuprimento
                    //                                                      join edi in this.objCtx.tbEntradaDiversa on sup.edi_codigo equals edi.edi_codigo
                    //                                                      join pes in this.objCtx.tbPessoa on sup.pes_codigoCliente equals pes.pes_codigo
                    //                                                      where sup.cai_codigo == intCodigoCaixa && sup.sup_excluido == true
                    //                                                      select new EntradaDiversaEstorno
                    //                                                      {
                    //                                                          san_codigo = sup.sup_codigo,
                    //                                                          pes_nome = pes.pes_nome,
                    //                                                          edi_descricao = edi.edi_descricao,
                    //                                                          san_valor = sup.sup_valor
                    //                                                      }).ToList();




                    //            ////########################  CARTA FRETE ###########################\\\
                    //            //***********************                        **********************\\
                    //            List<int?> arrCartasVinculadas = this.objCtx.tbCaixaMovimento.Include(caim => caim.tbContaReceber)
                    //                   .AsNoTracking().Where(caim => caim.carf_codigo != null && caim.caim_cancelado == false).Select(caim => caim.carf_codigo).ToList();

                    //            objFechamento.CartasFretes = this.objCtx.tbCartaFrete.AsNoTracking()
                    //                                                                 .Include(carf => carf.tbCaixaMovimento)
                    //                                                                 .Include(carf => carf.tbContaReceber)
                    //                                                                 .Include(carf => carf.tbPessoa)
                    //                                                                 .Where(carf =>
                    //                                                                    (carf.tbCaixaMovimento.Any(caim => caim.cai_codigo == intCodigoCaixa && caim.caim_cancelado == false && caim.vec_codigo != null && caim.caio_codigo == 2))
                    //                                                                    && carf.carf_excluida == false
                    //                                                                    && carf.tbContaReceber.crec_dataPagamento == null
                    //                                                                     ).Select(carf => new CartaFrete
                    //                                                                     {
                    //                                                                         carf_codigo = carf.carf_codigo,
                    //                                                                         carf_numero = carf.carf_numero,
                    //                                                                         carf_valor = carf.tbCaixaMovimento.Where(caim => caim.carf_codigo == carf.carf_codigo).Select(caim => caim.caim_valor).FirstOrDefault() ?? carf.carf_valor,
                    //                                                                         carf_descricao = carf.carf_observacoes,
                    //                                                                         pes_codigo = carf.pes_codigo,
                    //                                                                         pes_nome = carf.tbPessoa.pes_nome,
                    //                                                                         caim_codigo = carf.tbCaixaMovimento.Where(caim => caim.carf_codigo == carf.carf_codigo).Select(caim => caim.caim_codigo).FirstOrDefault()
                    //                                                                     })
                    //                                                                    .ToList();

                    //            //********************   SELECIONA CAIXAS MOVIMENTOS SEM VINCULOS  ***********************\\
                    //            List<CartaFrete> arrMovimentoCartaFreteSemVinculo = this.objCtx.tbCaixaMovimento.AsNoTracking()
                    //                                         .Include(caim => caim.tbFormaPagamento)
                    //                                         .Where(caim => caim.cai_codigo == intCodigoCaixa &&
                    //                                                         caim.caim_cancelado == false &&
                    //                                                         caim.vec_codigo != null &&
                    //                                                         caim.caio_codigo == 2 &&
                    //                                                         caim.carf_codigo == null &&
                    //                                                         caim.tbFormaPagamento.tfp_codigo == (int)enTipoFormaPagamento.CartaFrete)
                    //                                                         .Select(caim => new CartaFrete
                    //                                                         {
                    //                                                             carf_codigo = null,
                    //                                                             carf_numero = null,
                    //                                                             carf_valor = caim.caim_valor,
                    //                                                             carf_descricao = "Codigo Movimento: " + caim.caim_codigo.ToString(),
                    //                                                             pes_codigo = null,
                    //                                                             pes_nome = null,
                    //                                                             caim_codigo = caim.caim_codigo
                    //                                                         }).OrderBy(cf => cf.carf_codigo).ToList();

                    //            //*********************** ADICIONA NO ARRAY ***********************\\
                    //            foreach (CartaFrete objCartaFreteAdd in arrMovimentoCartaFreteSemVinculo)
                    //                objFechamento.CartasFretes.Add(objCartaFreteAdd);

                    //            //************************** ORDENAÇÃO ***********************\\
                    //            if (objFechamento.CartasFretes != null)
                    //                objFechamento.CartasFretes = objFechamento.CartasFretes.OrderBy(cf => cf.carf_codigo).ToList();
                    //            //********************************************************************\\
                    //            ////##################### FIM  CARTA FRETE ###########################\\\


                    //            //recuperar os caixas movimentos do tipo venda em que a forma seja do tipo cheque a vista ou cheque prazo
                    //            var arrCaixaMovimentoCheque = this.objCtx.tbCaixaMovimento.AsNoTracking()
                    //                                                                      .Include(caim => caim.tbCheque)
                    //                                                                      .Where(caim => caim.cai_codigo == intCodigoCaixa &&
                    //                                                                                      caim.caim_cancelado == false &&
                    //                                                                                     (caim.caio_codigo == (int)enCaixaOperacao.VendaCupom || caim.caio_codigo == (int)enCaixaOperacao.GeracaoHaver || caim.caio_codigo == (int)enCaixaOperacao.EntradasDiversas) &&
                    //                                                                                     (caim.tbFormaPagamento.tfp_codigo == (int)enTipoFormaPagamento.ChequePrazo ||
                    //                                                                                      caim.tbFormaPagamento.tfp_codigo == (int)enTipoFormaPagamento.ChequeVista
                    //                                                                                     )
                    //                                                                              ).ToList();

                    //            objFechamento.ChequeRecebido = new List<ChequeRecebido>();
                    //            //percorrer os movimentos de cheque para adicionar na coleção os cheques existentes, ou criar o cheque com o valor do movimento, para que o cheque seja preenchido pelo usuário
                    //            foreach (var objCaixaMovimentoCheque in arrCaixaMovimentoCheque)
                    //            {
                    //                ChequeRecebido objChequeRecebido = new ChequeRecebido();
                    //                objChequeRecebido.caim_codigo = objCaixaMovimentoCheque.caim_codigo;
                    //                if (objCaixaMovimentoCheque.tbCheque != null)
                    //                    objChequeRecebido.objCheque = objCaixaMovimentoCheque.tbCheque;
                    //                else
                    //                {
                    //                    objChequeRecebido.blnPrecisaCadastrarCheque = true;
                    //                    objChequeRecebido.objCheque = new tbCheque { chq_valor = objCaixaMovimentoCheque.caim_valor };
                    //                }

                    //                objFechamento.ChequeRecebido.Add(objChequeRecebido);
                    //            }



                    //            //movimentos de fechamento
                    //            int intCodigoOperacaoFechamento = (int)enCaixaOperacao.FechamentoDeCaixa;
                    //            objFechamento.MovimentoFechamento = this.objCtx.tbCaixaMovimento
                    //                                                            .Include(caim => caim.tbFormaPagamento)
                    //                                                            .AsNoTracking()
                    //                                                            .Where(caim => caim.cai_codigo == intCodigoCaixa && caim.caio_codigo == intCodigoOperacaoFechamento && caim.caim_cancelado == false)
                    //                                                            .ToList();



                    //            // encerrantes
                    //            DateTime dtAgora = DateTime.Now;
                    //            if (objCaixa.cai_dataFechamento != null)
                    //                dtAgora = objCaixa.cai_dataFechamento.Value;

                    //            objFechamento.Encerrantes = this.objCtx.spEncerrantesFechamentoCaixa(objCaixa.cai_dataAbertura, dtAgora, intCodigoCaixa, Util.objConfigStorage.objEmpresa.emp_codigo).ToList();


                    //            // itens vendidos(agrupados por produto e preço)
                    //            objFechamento.ItensVendidos = this.RetornarItensVendidoCaixa(intCodigoCaixa);

                    //            // RESUMO
                    //            objFechamento.Resumo = new List<FechamentoCaixaResumo>();

                    //            decimal? decValorAbertura = this.objCtx.tbCaixa.AsNoTracking()
                    //                                                           .Where(cai => cai.cai_codigo == intCodigoCaixa)
                    //                                                           .Select(cai => cai.cai_valor).FirstOrDefault();
                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(+) Abertura de Caixa",
                    //                decValor = decValorAbertura
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(+) Suprimento",
                    //                decValor = objFechamento.Suprimentos.Sum(s => s.sup_valor) - decValorAbertura
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(+) Entradas Diversas",
                    //                decValor = objFechamento.EntradasDiversas.Sum(s => s.sup_valor)
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(-) Sangria",
                    //                decValor = objFechamento.Sangrias.Sum(s => s.san_valor)
                    //            });


                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(+) Recebimento de Vendas",
                    //                decValor = objFechamento.FormasRecebidas.Sum(f => f.decValorTotalForma)
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(+) Recebimento de Conta Receber",
                    //                decValor = objFechamento.arrContasReceberPagas.Sum(f => f.crec_valorPago)
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(-) Troco(Dinheiro)",
                    //                decValor = this.objCtx.tbCaixaMovimento.AsNoTracking()
                    //                                                       .Where(caim => caim.cai_codigo == intCodigoCaixa &&
                    //                                                                      caim.caio_codigo == intCodigoOperacaoTroco &&
                    //                                                                      caim.caim_cancelado == false
                    //                                                             )
                    //                                                        .Sum(caim => caim.caim_valor)
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(-) Vales",
                    //                decValor = objFechamento.Vales.Sum(v => v.crec_valor)
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "\n"
                    //            }); // pular linha

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(=) Troco(Cheque)",
                    //                decValor = this.objCtx.tbCaixaMovimento.AsNoTracking()
                    //                                                       .Where(caim => caim.cai_codigo == intCodigoCaixa &&
                    //                                                                      caim.caio_codigo == intCodigoOperacaoChequeTroco &&
                    //                                                                      caim.caim_cancelado == false
                    //                                                             )
                    //                                                        .Sum(caim => caim.caim_valor)
                    //            });

                    //            objFechamento.Resumo.Add(new FechamentoCaixaResumo
                    //            {
                    //                strDescricao = "(=) Haver Criado",
                    //                decValor = objFechamento.HaverCriados.Sum(h => h.cpg_valor)
                    //            });


                    //            //formas resumo
                    //            objFechamento.FormasResumo = this.RetornaFormasResumoAgrupadas(intCodigoCaixa);

                    //            // recuperar as formas do tipo nota assinada para atualizar o valor dessas, pois o valor delas não será informado pelo usuário
                    //            List<tbCaixaMovimento> objMovCaixaNota = objFechamento.MovimentoFechamento.Where(f => f.tbFormaPagamento != null && f.tbFormaPagamento.tfp_codigo == (int)enTipoFormaPagamento.NotaAssinada).ToList();
                    //            foreach (var objMovimentoFechamentoNotaAssinada in objMovCaixaNota)
                    //            {
                    //                objMovimentoFechamentoNotaAssinada.caim_valor = objFechamento.FormasResumo.Where(f => f.fpg_codigo == objMovimentoFechamentoNotaAssinada.fpg_codigo)
                    //                                                                                          .FirstOrDefault()
                    //                                                                                          .decValorTotalForma;
                    //            }

                    //            //se o caixa não tem movimentos de fechamento, irá criar os movimento de fechamento, com valor zero(exceto para TEF e nota assinada que irá sugerir o valor)
                    //            if (objFechamento.MovimentoFechamento.Count == 0)
                    //            {
                    //                foreach (var objFormaResumo in objFechamento.FormasResumo)
                    //                {
                    //                    tbCaixaMovimento objMovimentoFechamento = new tbCaixaMovimento();
                    //                    objMovimentoFechamento.tbFormaPagamento = this.objCtx.tbFormaPagamento
                    //                                                                         .AsNoTracking()
                    //                                                                         .Where(fpg => fpg.fpg_codigo == objFormaResumo.fpg_codigo)
                    //                                                                         .FirstOrDefault();

                    //                    int intTipoForma = objMovimentoFechamento.tbFormaPagamento.tfp_codigo;
                    //                    objMovimentoFechamento.fpg_codigo = objFormaResumo.fpg_codigo;

                    //                    // irá sugerir o valor da forma TEF
                    //                    if (objMovimentoFechamento.tbFormaPagamento.fpg_tef == true || objMovimentoFechamento.tbFormaPagamento.tfp_codigo == (int)enTipoFormaPagamento.NotaAssinada || objMovimentoFechamento.tbFormaPagamento.tfp_codigo == (int)enTipoFormaPagamento.CartaFrete)
                    //                        objMovimentoFechamento.caim_valor = objFormaResumo.decValorTotalForma;
                    //                    else
                    //                        objMovimentoFechamento.caim_valor = 0;

                    //                    objFechamento.MovimentoFechamento.Add(objMovimentoFechamento);
                    //                }
                    //            }

                    //            objFechamento.decValorFinal = objFechamento.FormasResumo.Sum(f => f.decValorTotalForma);

                    //            objRetorno.objRetorno = objFechamento;
                    //        }
                    //        else
                    //        {
                    //            objRetorno.blnTemErro = true;
                    //            objRetorno.strMsgErro = "Registro não encontrado";
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Util.LogErro(ex);
                    //        objRetorno.blnTemErro = true;
                    //        objRetorno.strMsgErro = ex.Message;
                    //        objRetorno.strExceptionToString = ex.ToString();
                    //    }
                    //    return objRetorno;
                    //}

                    //public List<ItemVendido> RetornarItensVendidoCaixa(int intCodigoCaixa)
                    //{
                    //    return this.objCtx.tbVendaCupomDetalhe.Include(vecd => vecd.tbVendaCupom)
                    //                                          .Include(vecd => vecd.tbProduto)
                    //                                          .Where(vecd => vecd.tbVendaCupom.cai_codigo == intCodigoCaixa && // filtrar o código do caixa
                    //                                                         vecd.vecd_cancelado == false && // filtrar itens não cancelados
                    //                                                         vecd.tbVendaCupom.vec_statusCupom == "F" // filtrar itens de cupons finalizados
                    //                                           ).GroupBy(g =>
                    //                                               new
                    //                                                {
                    //                                                    pro_codigo = g.pro_codigo,
                    //                                                    pro_nome = g.tbProduto.pro_nome,
                    //                                                    decValorUnitario = g.vecd_valorUnitario
                    //                                                }
                    //                                            ).Select(s => new ItemVendido
                    //                                                  {
                    //                                                      pro_codigo = s.Key.pro_codigo,
                    //                                                      pro_nome = s.Key.pro_nome,
                    //                                                      decValorUnitario = s.Key.decValorUnitario,
                    //                                                      decQuantidade = s.Sum(g => g.vecd_quantidade),
                    //                                                      decValorTotal = s.Key.decValorUnitario * s.Sum(g => g.vecd_quantidade)
                    //                                                  }
                    //                                              ).ToList();

                    //}

                    ///// <summary>
                    ///// Agrupa por forma de pagamento todos os movimentos de caixa que são diferentes de FechamentoCaixa e Cheque troco, então soma a entrada e saida de cada forma agrupada dando o total
                    ///// </summary>
                    ///// <param name="intCodigoCaixa"></param>
                    ///// <returns></returns>
                    //private List<FormaFechamentoCaixa> RetornaFormasResumoAgrupadas(int intCodigoCaixa)
                    //{
                    //    List<FormaFechamentoCaixa> arrResumo = new List<FormaFechamentoCaixa>();

                    //    int intCodigoOperacaoFechamento = (int)enCaixaOperacao.FechamentoDeCaixa;

                    //    //agrupar os movimentos de caixa que não são de operação de fechamento, agrupando por forma e tipo de operação(entrada/saida)

                    //    var formasResumo = (from caim in this.objCtx.tbCaixaMovimento.Include(caim => caim.tbCaixaOperacao)
                    //                                                                 .Include(caim => caim.tbFormaPagamento)
                    //                                                                 .Include(caim => caim.tbCheque)
                    //                                                                 .Where(caim => caim.cai_codigo == intCodigoCaixa &&
                    //                                                                                caim.caio_codigo != intCodigoOperacaoFechamento &&
                    //                                                                     //caim.caio_codigo != (int)enCaixaOperacao.ChequeTroco &&
                    //                                                                                caim.caim_cancelado == false
                    //                                                                        )
                    //                        group caim by new
                    //                        {
                    //                            codigoForma = caim.fpg_codigo,
                    //                            descricaoForma = caim.tbFormaPagamento.fpg_descricao,
                    //                            tipoOperacao = caim.tbCaixaOperacao.caio_tipoOperacao
                    //                        } into grupo

                    //                        //retora o codigo da forma, o tipo da operacao, e valor total da forma para a operação
                    //                        select new
                    //                        {
                    //                            fpg_codigo = grupo.Key.codigoForma,
                    //                            fpg_descricao = grupo.Key.descricaoForma,
                    //                            decValorTotalForma = grupo.Sum(s => s.caim_valor),
                    //                            tipoOperacao = grupo.Key.tipoOperacao
                    //                        }).ToList();



                    //    //percorre os movimentos que estao agrupados por formas e tipo de operacao
                    //    //se na lista de resumo nao tem a forma, se nao recupera a forma ja adicionada. soma ou subtrai o valor de acordo com o tipo da operação
                    //    foreach (var item in formasResumo)
                    //    {
                    //        FormaFechamentoCaixa objFormaResumo = null;
                    //        if (arrResumo.Where(f => f.fpg_codigo == item.fpg_codigo).Count() == 0)
                    //        {
                    //            objFormaResumo = new FormaFechamentoCaixa();
                    //            objFormaResumo.fpg_codigo = item.fpg_codigo;
                    //            objFormaResumo.fpg_descricao = item.fpg_descricao;
                    //            objFormaResumo.decValorTotalForma = 0;
                    //            arrResumo.Add(objFormaResumo);
                    //        }
                    //        else
                    //        {
                    //            objFormaResumo = arrResumo.Where(fpg => fpg.fpg_codigo == item.fpg_codigo).FirstOrDefault();
                    //        }

                    //        if (item.tipoOperacao == "E")
                    //            objFormaResumo.decValorTotalForma += item.decValorTotalForma;
                    //        else
                    //            objFormaResumo.decValorTotalForma -= item.decValorTotalForma;
                    //    }

                    //    return arrResumo;
                    //}                   
                    #endregion base
                    //objRetorno.intCodigoErro = 0;
                    //objRetorno.objRetorno = objFechamentoCaixa;
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

        public Retorno FecharCaixa(FechamentoCaixa objFechamentoCaixa, int intFunCodigo)
        {
            var objRetorno = new Retorno();
            try
            {
                var objCaixaContexto = _objCtx.tbCaixa.FirstOrDefault(cai => cai.cai_codigo == objFechamentoCaixa.objCaixa.cai_codigo);

                objFechamentoCaixa.objCaixa.tbCaixaMovimento = null;
                objFechamentoCaixa.objCaixa.tbFuncionario = null;
                objFechamentoCaixa.objCaixa.tbPedido = null;
                objFechamentoCaixa.objCaixa.tbVenda = null;
                objFechamentoCaixa.objCaixa.cai_dataFechamento = DateTime.Now;

                _objCtx.Entry(objCaixaContexto).CurrentValues.SetValues(objFechamentoCaixa.objCaixa);
                _objCtx.SaveChanges();
                using (var objBll = new Auditoria(ref _objCtx, ref _objTransacao))
                    objBll.SalvarAuditoria(objFechamentoCaixa.objCaixa.cai_codigo, enOperacao.Alteracao, objFechamentoCaixa.objCaixa, intFunCodigo);
                objRetorno = RetornaCaixaFechamento(objFechamentoCaixa.objCaixa.cai_codigo);
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

        public Retorno ExcluirCaixa(int intCodigo)
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
                            var objCaixa = objContexto.tbCaixa.FirstOrDefault(cai => cai.cai_codigo == intCodigo);
                            if (objCaixa != null)
                            {
                                //Tenta excluir o caixa no contexto isolado
                                objContexto.tbCaixa.Remove(objCaixa);
                                objContexto.SaveChanges();
                                transacao.Commit();

                                objRetorno.intCodigoErro = 0;
                                objRetorno.objRetorno = true;
                            }
                            else
                            {
                                objRetorno.intCodigoErro = 48;
                                objRetorno.strMsgErro = "Caixa não encontrado para exclusão";
                            }
                        }
                        catch (Exception)
                        {
                            //Se deu erro é porque o caixa tem  registros relacionado
                            transacao.Rollback();
                            objRetorno.intCodigoErro = 48;
                            objRetorno.strMsgErro = "Caixa não pode ser excluido pois há registros relacionados ao mesmo.";
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

        private string ValidaAberturaCaixa(tbCaixa objCaixa)
        {
            //É necessário validar se já existe caixa aberto para o funcionário especificado, caso exista, a abertura deve ser negada.
            var objCaixaAberto = _objCtx.tbCaixa.FirstOrDefault(cai => cai.fun_codigo == objCaixa.fun_codigo && cai.cai_dataFechamento == null);
            if (objCaixaAberto != null)
                return "Já existe caixa aberto para o funcionário especificado!";

            //É necessário validar se existe formas de pagamento informada e se são válidas
            if (objCaixa.tbCaixaMovimento.Where(cai => cai.fpg_codigo == 0).Count() > 0)
                return "Forma de pagamento inválida!";

            return objCaixa.tbCaixaMovimento.Count == 0 ? "Não foram informados nenhuma forma de pagamento." : string.Empty;
        }

        private string ValidaMovimentoCaixa(tbCaixaMovimento objCaixaMovimento)
        {
            //if (string.IsNullOrEmpty(objProdutoGrupo.pgr_nome) || string.IsNullOrWhiteSpace(objProdutoGrupo.pgr_nome))
            //    return "O nome deve ser informado.";

            //if (this.objCtx.tbProdutoGrupo.AsNoTracking()
            //                             .Where(pgr => (pgr.pgr_nome.Equals(objProdutoGrupo.pgr_nome)) && pgr.pgr_codigo != objProdutoGrupo.pgr_codigo)
            //                             .Count() > 0)
            //    return "Já existe produto grupo com esse nome.";
            //Retorno objRetorno = RetornaCaixa(0);
            //if(objRetorno.objRetorno != null)
            //    return "Já existe caixa aberto, feche o sistema e entre novamente!";

            return objCaixaMovimento.cai_codigo == 0 ? "Caixa não informado!" : string.Empty;
        }

        public void Dispose()
        {
            if (!_blnFecharCon) return;
            _objCtx.Dispose();
            _objCtx = null;
        }
    }
}
