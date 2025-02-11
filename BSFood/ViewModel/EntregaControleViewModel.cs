﻿using BSFood.View;
using BSFood.Apoio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using BSFoodFramework.Models;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.BusinessLogic;
using BSFoodFramework.Apoio;

namespace BSFood.ViewModel
{
    public class EntregaControleViewModel : TelaViewModel
    {
        public ICommand NavegarCommand { get; set; }
        public ICommand NovoCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand ExcluirCommand { get; set; }
        public ICommand PesquisarCommand { get; set; }
        public ICommand FuncionarioEntregadorCommand { get; set; }
        public ICommand CaixaCommand { get; set; }
        private DispatcherTimer timerAtualizaPedido;
        private IcBoxClassLibrary.IcBox objBina;

        public EntregaControleViewModel(EntregaViewModel _objEntregaViewModel)
        {
            NavegarCommand = new DelegateCommand(Navegar);
            NovoCommand = new DelegateCommand(Novo, CanNovo);
            EditarCommand = new DelegateCommand(Editar, CanEditar);
            ExcluirCommand = new DelegateCommand(Excluir, CanExcluir);
            PesquisarCommand = new DelegateCommand(Pesquisar);
            FuncionarioEntregadorCommand = new DelegateCommand(FuncionarioEntregador);
            CaixaCommand = new DelegateCommand(Caixa);

            objEntregaViewModel = _objEntregaViewModel;
            Caixa(null);
            _intCaiCodigoPesquisa = 0;
            _blnProducaoPesquisa = true;
            _blnEntregaPesquisa = true;
            base.intQtdeRegPagina = 100;
            Pesquisar(0);

            timerAtualizaPedido = new DispatcherTimer();
            timerAtualizaPedido.Interval = new TimeSpan(0, 1, 0); // 1 minuto
            timerAtualizaPedido.Tick += timerAtualizaPedido_Tick;
            timerAtualizaPedido.Start();

            _objCorMensagemBina = new SolidColorBrush(Colors.Red);
            _strMensagemBina = "Conectando...";
        }


        #region Propriedades

        private bool _blnProducaoPesquisa;
        public bool blnProducaoPesquisa
        {
            get { return _blnProducaoPesquisa; }
            set
            {
                if (_blnProducaoPesquisa != value)
                {
                    _blnProducaoPesquisa = value;
                    RaisePropertyChanged();
                    Pesquisar(0);
                }
            }
        }

        private bool _blnEntregaPesquisa;
        public bool blnEntregaPesquisa
        {
            get { return _blnEntregaPesquisa; }
            set
            {
                if (_blnEntregaPesquisa != value)
                {
                    _blnEntregaPesquisa = value;
                    RaisePropertyChanged();
                    Pesquisar(0);
                }
            }
        }

        private bool _blnFinalizadoPesquisa;
        public bool blnFinalizadoPesquisa
        {
            get { return _blnFinalizadoPesquisa; }
            set
            {
                if (_blnFinalizadoPesquisa != value)
                {
                    _blnFinalizadoPesquisa = value;
                    RaisePropertyChanged();
                    Pesquisar(0);
                }
            }
        }

        private bool _blnExcluidoPesquisa;
        public bool blnExcluidoPesquisa
        {
            get { return _blnExcluidoPesquisa; }
            set
            {
                if (_blnExcluidoPesquisa != value)
                {
                    _blnExcluidoPesquisa = value;
                    RaisePropertyChanged();
                    Pesquisar(0);
                }
            }
        }

        private int? _intFunCodigoPesquisa;
        public int? intFunCodigoPesquisa
        {
            get { return _intFunCodigoPesquisa; }
            set
            {
                if (_intFunCodigoPesquisa != value)
                {
                    _intFunCodigoPesquisa = value == null ? 0 : (int)value;
                    FuncionarioEntregador(_intFunCodigoPesquisa);
                }
            }
        }

        private string _strFunNomePesquisa;
        public string strFunNomePesquisa
        {
            get { return _strFunNomePesquisa; }
            set
            {
                if (_strFunNomePesquisa != value)
                {
                    _strFunNomePesquisa = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _intCaiCodigoPesquisa;
        public int intCaiCodigoPesquisa
        {
            get { return _intCaiCodigoPesquisa; }
            set
            {
                _intCaiCodigoPesquisa = value;
                RaisePropertyChanged();
                Pesquisar(0);
            }
        }

        private string _strMotivoCancelamento;
        [StringLength(250, ErrorMessage = "É permitido apenas 250 caracteres")]
        public string strMotivoCancelamento
        {
            get { return _strMotivoCancelamento; }
            set
            {
                if (_strMotivoCancelamento != value)
                {
                    _strMotivoCancelamento = value;
                    RaisePropertyChanged();
                }
            }
        }

        private List<tbCaixa> _arrCaixa;
        public List<tbCaixa> arrCaixa
        {
            get { return _arrCaixa; }
            set
            {
                _arrCaixa = value;
                RaisePropertyChanged();
            }
        }

        public EntregaViewModel objEntregaViewModel { get; set; }

        private ObservableCollection<EntregaControleDetalheViewModel> _arrEntregaControleDetalheViewModel;
        public ObservableCollection<EntregaControleDetalheViewModel> arrEntregaControleDetalheViewModel
        {
            get { return _arrEntregaControleDetalheViewModel; }
            set
            {
                _arrEntregaControleDetalheViewModel = value;
                RaisePropertyChanged();
            }
        }

        private string _strMensagemBina;
        public string strMensagemBina
        {
            get { return _strMensagemBina; }
            set
            {
                if(_strMensagemBina != value)
                {
                    _strMensagemBina = value;
                    RaisePropertyChanged();
                }
            }
        }

        private SolidColorBrush _objCorMensagemBina;
        public SolidColorBrush objCorMensagemBina
        {
            get { return _objCorMensagemBina; }
            set
            {
                if(_objCorMensagemBina != value)
                {
                    _objCorMensagemBina = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion Propriedades



        #region Comandos

        private void Navegar(object objParam)
        {
            if (objParam != null)
            {
                if (objParam.ToString() == "Proximo")
                {
                    if (base.intPaginaAtual < base.intTotalPagina)
                    {
                        Pesquisar(base.intPaginaAtual * base.intQtdeRegPagina);
                        base.intPaginaAtual++;
                    }
                }
                else if (objParam.ToString() == "Anterior")
                {
                    if (base.intPaginaAtual > 1)
                    {
                        base.intPaginaAtual--;
                        if (base.intPaginaAtual == 1)
                            Pesquisar(0);
                        else
                            Pesquisar((base.intPaginaAtual - 1) * base.intQtdeRegPagina);
                    }
                }
            }
        }

        private bool CanNovo(object objParam)
        {
            return objEntregaViewModel.blnPermiteInclusaoRegistro;
        }
        private void Novo(object objParam)
        {
            tbPedido objPedidoAux = new tbPedido();
            objPedidoAux.ped_data = DateTime.Now;
            objPedidoAux.tbCliente = null;

            int intNumero;
            if (objParam != null && int.TryParse(objParam.ToString(), out intNumero))
            {
                Retorno objRetorno;
                using (var objBLL = new Clientes())
                {
                    objRetorno = objBLL.RetornaCliente(intNumero, null);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    objPedidoAux.tbCliente = (tbCliente)objRetorno.objRetorno;
                    objPedidoAux.cli_codigo = objPedidoAux.tbCliente.cli_codigo;
                    objPedidoAux.ped_nomeCliente = objPedidoAux.tbCliente.cli_nome;
                    objPedidoAux.ped_telefone = intNumero.ToString();
                }
                else
                {
                    if (objRetorno.intCodigoErro == 48)
                        objPedidoAux.ped_telefone = intNumero.ToString();
                    else
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
            }

            if (objPedidoAux.tbCliente == null)
            {
                objPedidoAux.tbCliente = new tbCliente();
                objPedidoAux.tbCliente.cli_tipo = "F";
                objPedidoAux.tbCliente.cli_sexo = "M";
                objPedidoAux.tbCliente.cgr_codigo = FrameworkUtil.objConfigStorage.objConfiguracao.cgr_codigo;
                objPedidoAux.tbCliente.tbClienteEndereco = new List<tbClienteEndereco>();

                if(objParam != null && int.TryParse(objParam.ToString(), out intNumero))
                {
                    objPedidoAux.tbCliente.tbClienteTelefone = new List<tbClienteTelefone>();
                    objPedidoAux.tbCliente.tbClienteTelefone.Add(new tbClienteTelefone { ctl_numero = intNumero.ToString() });
                }

                tbClienteEndereco objClienteEndereco = new tbClienteEndereco();
                objClienteEndereco.est_codigo = FrameworkUtil.objConfigStorage.objEmpresa.est_codigo;
                objClienteEndereco.cid_codigo = FrameworkUtil.objConfigStorage.objEmpresa.cid_codigo;
                objClienteEndereco.tbBairro = new tbBairro();
                objPedidoAux.tbCliente.tbClienteEndereco.Add(objClienteEndereco);
            }

            objPedidoAux.tbFuncionarioEntregador = new tbFuncionario();

            objPedidoAux.tbFormaPagamento = new tbFormaPagamento();

            objPedidoAux.tbPedidoProduto = new List<tbPedidoProduto>();

            tbPedidoProduto objPedidoProduto = new tbPedidoProduto();
            objPedidoProduto.tbProduto = new tbProduto();
            objPedidoAux.tbPedidoProduto.Add(objPedidoProduto);

            var objCaixa = arrCaixa.FirstOrDefault(cai => cai.fun_codigo == FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
            if (objCaixa != null)
                objPedidoAux.cai_codigo = objCaixa.cai_codigo;
            else
            {
                var objCaixaPadrao = arrCaixa.FirstOrDefault(cai => cai.cai_codigo > 0);
                if (objCaixaPadrao != null)
                    objPedidoAux.cai_codigo = objCaixaPadrao.cai_codigo;
            }

            EntregaPedidoViewModel objEntregaPedidoViewModel = new EntregaPedidoViewModel();
            objEntregaPedidoViewModel.OnDispose += ObjEntregaPedidoViewModel_OnDispose;
            objEntregaPedidoViewModel.objPedido = objPedidoAux;
            if (!string.IsNullOrWhiteSpace(objPedidoAux.ped_telefone))
                objEntregaPedidoViewModel.strNomeTela = objPedidoAux.ped_telefone;
            if (objPedidoAux.tbCliente.cli_codigo > 0)
                objEntregaPedidoViewModel.PedidoFocus("CodigoProduto");
            else
            {
                if (!string.IsNullOrWhiteSpace(objPedidoAux.ped_telefone))
                    objEntregaPedidoViewModel.blnNomeFocus = true;
                else
                    objEntregaPedidoViewModel.blnTelefoneFocus = true;
            }
            objEntregaViewModel.arrEntregaPedidoViewModel.Add(objEntregaPedidoViewModel);
            objEntregaViewModel.objEntregaPedidoViewModel = objEntregaPedidoViewModel;
        }

        private bool CanEditar(object objParam)
        {
            return objEntregaViewModel.blnPermiteAlteracaoRegistro;
        }
        private void Editar(object objParam)
        {
            if (objParam != null)
            {
                Retorno objRetorno;
                using (var objBLL = new Pedidos())
                {
                    objRetorno = objBLL.RetornaPedido((int)objParam, null, enOrigemPedido.Entrega);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    EntregaPedidoViewModel objEntregaPedidoViewModel = new EntregaPedidoViewModel();
                    objEntregaPedidoViewModel.OnDispose += ObjEntregaPedidoViewModel_OnDispose;
                    objEntregaPedidoViewModel.objPedido = (tbPedido)objRetorno.objRetorno;
                    objEntregaViewModel.arrEntregaPedidoViewModel.Add(objEntregaPedidoViewModel);
                    objEntregaViewModel.objEntregaPedidoViewModel = objEntregaPedidoViewModel;
                }
                else
                {
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
            }
        }

        private bool CanExcluir(object objParam)
        {
            return objEntregaViewModel.blnPermiteExclusaoRegistro;
        }
        private void Excluir(object objParam)
        {
            if (objParam.GetType() != typeof(winMotivoExclusaoPedido))
            {
                winMotivoExclusaoPedido objTelaMotivoExclusaoPedido = new winMotivoExclusaoPedido();
                objTelaMotivoExclusaoPedido.DataContext = this;
                objTelaMotivoExclusaoPedido.Tag = objParam;
                objTelaMotivoExclusaoPedido.Owner = (Window)Application.Current.MainWindow;
                objTelaMotivoExclusaoPedido.Closed += (sen, eve) => { objTelaMotivoExclusaoPedido = null; };
                objTelaMotivoExclusaoPedido.ShowDialog();
            }
            else
            {
                if (((Window)objParam).Tag != null)
                {
                    if (!string.IsNullOrWhiteSpace(strMotivoCancelamento))
                    {
                        if (MessageBox.Show("Tem Certeza que deseja excluir o registro selecionado?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            Retorno objRetorno;
                            using (var objBLL = new Pedidos())
                            {
                                objRetorno = objBLL.ExcluirPedido((int)((Window)objParam).Tag, strMotivoCancelamento, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                            }
                            if (objRetorno.intCodigoErro == 0)
                                Pesquisar(0);
                            else
                                MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                        }
                        else
                            strMotivoCancelamento = string.Empty;
                        ((Window)objParam).Close();
                    }
                    else
                        MessageBox.Show("Informe o motivo da exclusão!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    ((Window)objParam).Close();
            }
        }

        public void Pesquisar(object objParam)
        {
            int intSkip;
            if (objParam == null || !int.TryParse(objParam.ToString(), out intSkip))
                intSkip = 0;

            Retorno objRetorno;
            using (var objBLL = new Pedidos())
            {
                objRetorno = objBLL.RetornaListaPedidoEntrega( 
                    blnProducaoPesquisa,
                    blnEntregaPesquisa, 
                    blnFinalizadoPesquisa, 
                    blnExcluidoPesquisa,
                    intFunCodigoPesquisa == null ? 0 : (int)intFunCodigoPesquisa,
                    intCaiCodigoPesquisa,
                    intSkip,
                    base.intQtdeRegPagina);
            }
            if (objRetorno.intCodigoErro == 0)
            {
                if (objRetorno.intQtdeRegistro > 0)
                {
                    if ((objRetorno.intQtdeRegistro % base.intQtdeRegPagina) > 0)
                        base.intTotalPagina = (int)(objRetorno.intQtdeRegistro / base.intQtdeRegPagina) + 1;
                    else
                        base.intTotalPagina = (int)(objRetorno.intQtdeRegistro / base.intQtdeRegPagina);
                    base.intPaginaAtual = 1;
                    base.intQtdeReg = objRetorno.intQtdeRegistro;
                }
                ObservableCollection<EntregaControleDetalheViewModel> arrEntregaControleDetalheViewModelAux = new ObservableCollection<EntregaControleDetalheViewModel>();
                foreach (tbPedido objPedido in (List<tbPedido>)objRetorno.objRetorno)
                {
                    if (objPedido.tbFuncionarioEntregador == null)
                        objPedido.tbFuncionarioEntregador = new tbFuncionario();
                    EntregaControleDetalheViewModel objEntregaControleDetalheViewModel = new EntregaControleDetalheViewModel(objPedido);
                    if (objEntregaControleDetalheViewModel.objPedido.ped_dataEntrega == null)
                        objEntregaControleDetalheViewModel.tsProducao = DateTime.Now.Subtract(objPedido.ped_data.Value);
                    else
                        objEntregaControleDetalheViewModel.tsProducao = objEntregaControleDetalheViewModel.objPedido.ped_dataEntrega.Value.Subtract(objPedido.ped_data.Value);
                    arrEntregaControleDetalheViewModelAux.Add(objEntregaControleDetalheViewModel);
                }
                arrEntregaControleDetalheViewModel = arrEntregaControleDetalheViewModelAux;
                if (arrEntregaControleDetalheViewModel.Count() == 0)
                {
                    base.intTotalPagina = 1;
                    base.intPaginaAtual = 1;
                    base.intQtdeReg = 0;
                }
            }
            else
                MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
        }

        private void FuncionarioEntregador(object objParam)
        {
            int intCodigo;
            if (objParam != null)
            {
                if (objParam.GetType() == typeof(tbFuncionario))
                {
                    if (((tbFuncionario)objParam).fun_codigo > 0)
                    {
                        _intFunCodigoPesquisa = ((tbFuncionario)objParam).fun_codigo;
                        _strFunNomePesquisa = ((tbFuncionario)objParam).fun_nome;
                    }
                    else
                    {
                        _intFunCodigoPesquisa = null;
                        _strFunNomePesquisa = string.Empty;
                    }
                    RaisePropertyChanged("intFunCodigoPesquisa");
                    RaisePropertyChanged("strFunNomePesquisa");
                }
                else if (objParam.ToString() == "Pesquisar")
                {
                    winCadastro objTelaCadastro = new winCadastro();
                    FuncionarioViewModel objFuncionarioViewModel = new FuncionarioViewModel();
                    objFuncionarioViewModel.OnDispose += (sen1, eve1) => { objTelaCadastro.Close(); };
                    objFuncionarioViewModel.blnJanela = true;
                    objTelaCadastro.Title = "Cadastro - " + objFuncionarioViewModel.strNomeTela;
                    objTelaCadastro.DataContext = objFuncionarioViewModel;
                    objTelaCadastro.Owner = (Window)Application.Current.MainWindow;
                    objTelaCadastro.Closed += (sen, eve) =>
                    {
                        FuncionarioEntregador(objFuncionarioViewModel.objFuncionario);
                        objFuncionarioViewModel = null;
                        objTelaCadastro = null;
                    };
                    objTelaCadastro.ShowDialog();
                }
                else if (int.TryParse(objParam.ToString(), out intCodigo))
                {
                    Retorno objRetorno;
                    using (var objBLL = new Funcionarios())
                    {
                        objRetorno = objBLL.RetornaFuncionario(intCodigo, null);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        FuncionarioEntregador((tbFuncionario)objRetorno.objRetorno);
                    }
                    else
                    {
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                        FuncionarioEntregador(new tbFuncionario());
                    }
                }
                else
                    FuncionarioEntregador("Pesquisar");
            }
        }

        private void Caixa(object objParam)
        {
            Retorno objRetorno;
            using (var objBLL = new Caixa())
            {
                objRetorno = objBLL.RetornaListaCaixaAberto();
            }
            if (objRetorno.intCodigoErro == 0)
            {
                List<tbCaixa> arrCaixaAux = new List<tbCaixa>();
                arrCaixaAux.Add(new tbCaixa { tbFuncionario = new tbFuncionario { fun_nome = "TODOS" } });
                foreach (var objCaixa in (List<tbCaixa>)objRetorno.objRetorno)
                    arrCaixaAux.Add(objCaixa);
                arrCaixa = arrCaixaAux;
                if (arrCaixa.Count() > 0)
                    intCaiCodigoPesquisa = 0;
            }
            else
                MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
        }

        #endregion Comandos



        #region Eventos

        private void ObjEntregaPedidoViewModel_OnDispose(object sender, EventArgs e)
        {
            objEntregaViewModel.arrEntregaPedidoViewModel.Remove((ViewModelBase)sender);
            Pesquisar(0);
        }

        void timerAtualizaPedido_Tick(object sender, EventArgs e)
        {
            Pesquisar(0);
            if(objBina == null)
                AtivarBina();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int stp = 0;
                string evento = string.Empty;
                do
                {
                    stp += 10;
                    if (stp == 100)
                    {
                        evento = objBina.getEvent(500);
                        if (evento.Length > 0)
                        {
                            (sender as BackgroundWorker).ReportProgress(0, evento);
                        }
                        stp = 0;
                    }
                    evento = string.Empty;
                    Thread.Sleep(100);
                    (sender as BackgroundWorker).ReportProgress(1, "Conectado");
                }
                while (objBina.isConnected());
                (sender as BackgroundWorker).ReportProgress(2, "Desconectado");
            }
            catch (Exception ex)
            {
                (sender as BackgroundWorker).ReportProgress(2, ex.Message);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 0 && e.UserState != null)
            {
                string evento = e.UserState.ToString();
                if (evento.Contains("E") && evento.Contains("I"))
                {
                    string strNumero = evento.Trim();
                    strNumero = strNumero.Substring(3, strNumero.IndexOf("E") - 3);

                    objCorMensagemBina = new SolidColorBrush(Colors.Blue);
                    strMensagemBina = "Número detectado: " + strNumero;

                    Novo(strNumero);
                }
            }

            if (e.ProgressPercentage == 1 && e.UserState != null)
            {
                objCorMensagemBina = new SolidColorBrush(Colors.Blue);
                strMensagemBina = e.UserState.ToString();
            }

            if (e.ProgressPercentage == 2 && e.UserState != null)
            {
                objCorMensagemBina = new SolidColorBrush(Colors.Red);
                strMensagemBina = e.UserState.ToString();
            }

        }

        #endregion Eventos



        #region Métodos

        private void AtivarBina()
        {
            bool blnBinaConectada = false;

            objBina = new IcBoxClassLibrary.IcBox();
            objBina.initialize("");

            objCorMensagemBina = new SolidColorBrush(Colors.Red);
            var arrPorta = objBina.GetPorts();
            for(int i = 0; i < arrPorta.Length; i++)
            {
                strMensagemBina = "Tentando conexão na porta: " + arrPorta.GetValue(i).ToString();
                if (objBina.openCom(arrPorta.GetValue(i).ToString()))
                {
                    Thread.Sleep(100);
                    if (objBina.isConnected())
                    {
                        int intId;
                        if(int.TryParse(objBina.getId(),out intId))
                        {
                            blnBinaConectada = true;
                            break;
                        }
                    }
                    objBina.closeCom();
                }
            }

            if (blnBinaConectada)
            {
                objCorMensagemBina = new SolidColorBrush(Colors.Blue);
                strMensagemBina = "Conectado";

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerAsync();
            }
            else
            {
                objCorMensagemBina = new SolidColorBrush(Colors.Red);
                strMensagemBina = "Desconectado";
            }
        }

        #endregion Métodos
    }
}