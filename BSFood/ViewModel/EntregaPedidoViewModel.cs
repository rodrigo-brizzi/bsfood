﻿using BSFood.View;
using BSFood.Apoio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using BSFoodFramework.Models;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.BusinessLogic;
using BSFoodFramework.Apoio;

namespace BSFood.ViewModel
{
    public class EntregaPedidoViewModel : TelaViewModel
    {
        public ICommand SalvarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand LogCommand { get; set; }
        public ICommand ClienteCommand { get; set; }
        public ICommand FuncionarioEntregadorCommand { get; set; }
        public ICommand FormaPagamentoCommand { get; set; }
        public ICommand AdicionaProdutoCommand { get; set; }
        public ICommand AdicionaEnderecoCommand { get; set; }
        public ICommand ImprimirCupomCommand { get; set; }
        public ICommand CaixaCommand { get; set; }
        public ICommand PedidoFocusCommand { get; set; }

        public EntregaPedidoViewModel()
        {
            SalvarCommand = new DelegateCommand(Salvar, CanSalvar);
            CancelarCommand = new DelegateCommand(Cancelar);
            LogCommand = new DelegateCommand(Log);
            ClienteCommand = new DelegateCommand(Cliente);
            FuncionarioEntregadorCommand = new DelegateCommand(FuncionarioEntregador);
            FormaPagamentoCommand = new DelegateCommand(FormaPagamento);
            AdicionaProdutoCommand = new DelegateCommand(AdicionaProduto);
            AdicionaEnderecoCommand = new DelegateCommand(AdicionaEndereco);
            ImprimirCupomCommand = new DelegateCommand(ImprimirCupom);
            CaixaCommand = new DelegateCommand(Caixa);
            PedidoFocusCommand = new DelegateCommand(PedidoFocus);
            Caixa(null);
        }


        #region Propriedades

        [Required(ErrorMessage = "Telefone obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Telefone obrigatório")]
        public string ped_telefone
        {
            get { return objPedido == null ? string.Empty : objPedido.ped_telefone; }
            set
            {
                if (objPedido.ped_telefone != value)
                    Cliente(value);
            }
        }

        [Required(ErrorMessage = "Nome obrigatório")]
        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string ped_nomeCliente
        {
            get { return objPedido == null ? string.Empty : objPedido.ped_nomeCliente; }
            set
            {
                if (objPedido.ped_nomeCliente != value)
                {
                    objPedido.ped_nomeCliente = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int? fun_funcionarioEntregador
        {
            get
            {
                if (objPedido == null || objPedido.fun_funcionarioEntregador == 0)
                    return null;
                else
                    return objPedido.fun_funcionarioEntregador;
            }
            set
            {
                if (objPedido.fun_funcionarioEntregador != value)
                {
                    objPedido.fun_funcionarioEntregador = value;
                    FuncionarioEntregador(objPedido.fun_funcionarioEntregador);
                }
            }
        }
        public string fun_nomeEntregador
        {
            get { return objPedido == null ? string.Empty : (objPedido.tbFuncionarioEntregador == null ? string.Empty : objPedido.tbFuncionarioEntregador.fun_nome); }
            set
            {
                if (objPedido.tbFuncionarioEntregador.fun_nome != value)
                {
                    objPedido.tbFuncionarioEntregador.fun_nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Forma de pagamento obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Forma de pagamento obrigatória")]
        public int? fpg_codigo
        {
            get
            {
                if (objPedido == null || objPedido.fpg_codigo == 0)
                    return null;
                else
                    return objPedido.fpg_codigo;
            }
            set
            {
                if (objPedido.fpg_codigo != value)
                {
                    objPedido.fpg_codigo = value == null ? 0 : (int)value;
                    FormaPagamento(objPedido.fpg_codigo);
                }
            }
        }
        public string fpg_descricao
        {
            get { return objPedido == null ? string.Empty : objPedido.tbFormaPagamento.fpg_descricao; }
            set
            {
                if (objPedido.tbFormaPagamento.fpg_descricao != value)
                {
                    objPedido.tbFormaPagamento.fpg_descricao = value;
                    RaisePropertyChanged();
                }
            }
        }

        private tbPedido _objPedido;
        public tbPedido objPedido
        {
            get { return _objPedido; }
            set
            {
                _objPedido = value;
                if (_objPedido != null)
                {
                    ObservableCollection<EntregaPedidoProdutoViewModel> arrEntregaPedidoProdutoViewModelAux = new ObservableCollection<EntregaPedidoProdutoViewModel>();
                    foreach (tbPedidoProduto objPedidoProduto in _objPedido.tbPedidoProduto)
                    {
                        EntregaPedidoProdutoViewModel objEntregaPedidoProdutoViewModel = new EntregaPedidoProdutoViewModel(objPedidoProduto);
                        objEntregaPedidoProdutoViewModel.OnDispose += objEntregaPedidoProdutoViewModel_OnDispose;
                        objEntregaPedidoProdutoViewModel.PropertyChanged += objEntregaPedidoProdutoViewModel_PropertyChanged;
                        arrEntregaPedidoProdutoViewModelAux.Add(objEntregaPedidoProdutoViewModel);
                    }
                    _arrEntregaPedidoProdutoViewModel = arrEntregaPedidoProdutoViewModelAux;

                    ObservableCollection<EntregaPedidoEnderecoViewModel> arrEntregaPedidoEnderecoViewModelAux = new ObservableCollection<EntregaPedidoEnderecoViewModel>();
                    foreach (tbClienteEndereco objClienteEndereco in _objPedido.tbCliente.tbClienteEndereco)
                    {
                        EntregaPedidoEnderecoViewModel objEntregaPedidoEnderecoViewModel = new EntregaPedidoEnderecoViewModel(objClienteEndereco);
                        objEntregaPedidoEnderecoViewModel.OnDispose += ObjEntregaPedidoEnderecoViewModel_OnDispose;
                        objEntregaPedidoEnderecoViewModel.PropertyChanged += ObjEntregaPedidoEnderecoViewModel_PropertyChanged;
                        arrEntregaPedidoEnderecoViewModelAux.Add(objEntregaPedidoEnderecoViewModel);
                    }
                        
                    _arrEntregaPedidoEnderecoViewModel = arrEntregaPedidoEnderecoViewModelAux;
                    if (_objPedido.ped_codigo > 0)
                    {
                        //Aqui pode acontecer um possível problema, se o usuario excluir o endereço do cliente e esse endereço foi utilizado para
                        //alguma entrega, o mesmo não será encontrado e resultará em um erro
                        //uma possivel solução seria reconsruir um objeto de endereço para o cliente a partir dos dados do pedido
                        _arrEntregaPedidoEnderecoViewModel.Where(cen => cen.cen_logradouro == _objPedido.ped_logradouro).FirstOrDefault().blnSelecionado = true;
                    }
                    else
                        _arrEntregaPedidoEnderecoViewModel.FirstOrDefault().blnSelecionado = true;
                    if (_objPedido.tbFuncionarioEntregador == null)
                        _objPedido.tbFuncionarioEntregador = new tbFuncionario();
                }
                else
                {
                    _arrEntregaPedidoProdutoViewModel = null;
                    _arrEntregaPedidoEnderecoViewModel = null;
                }

                //Prepara propriedades da viewmodel
                RaisePropertyChanged(null);
            }
        }

        private ObservableCollection<EntregaPedidoEnderecoViewModel> _arrEntregaPedidoEnderecoViewModel;
        public ObservableCollection<EntregaPedidoEnderecoViewModel> arrEntregaPedidoEnderecoViewModel
        {
            get { return _arrEntregaPedidoEnderecoViewModel; }
            set
            {
                _arrEntregaPedidoEnderecoViewModel = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<EntregaPedidoProdutoViewModel> _arrEntregaPedidoProdutoViewModel;
        public ObservableCollection<EntregaPedidoProdutoViewModel> arrEntregaPedidoProdutoViewModel
        {
            get { return _arrEntregaPedidoProdutoViewModel; }
            set
            {
                _arrEntregaPedidoProdutoViewModel = value;
                RaisePropertyChanged();
            }
        }

        [Required(ErrorMessage = "Caixa obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Caixa obrigatório")]
        public int cai_codigo
        {
            get { return objPedido == null ? 0 : objPedido.cai_codigo; }
            set
            {
                if (objPedido.cai_codigo != value)
                {
                    objPedido.cai_codigo = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal ped_valorSubTotal
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorSubTotal; }
            set
            {
                if (objPedido.ped_valorSubTotal != value)
                {
                    objPedido.ped_valorSubTotal = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal ped_valorTaxaEntrega
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorTaxaEntrega; }
            set
            {
                if (objPedido.ped_valorTaxaEntrega != value)
                {
                    objPedido.ped_valorTaxaEntrega = value;
                    CalculaValores();
                    RaisePropertyChanged();
                }
            }
        }

        public decimal ped_valorDespesa
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorDespesa; }
            set
            {
                if (objPedido.ped_valorDespesa != value)
                {
                    objPedido.ped_valorDespesa = value;
                    CalculaValores();
                    RaisePropertyChanged();
                }
            }
        }

        public decimal ped_valorDesconto
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorDesconto; }
            set
            {
                if (objPedido.ped_valorDesconto != value)
                {
                    objPedido.ped_valorDesconto = value;
                    CalculaValores();
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Valor recebido obrigatório")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Valor recebido inválido")]
        public decimal ped_valorRecebido
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorRecebido; }
            set
            {
                if (objPedido.ped_valorRecebido != value)
                {
                    objPedido.ped_valorRecebido = value;
                    CalculaValores();
                    RaisePropertyChanged();
                }
            }
        }

        public decimal ped_valorTroco
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorTroco; }
            set
            {
                if (objPedido.ped_valorTroco != value)
                {
                    objPedido.ped_valorTroco = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal ped_valorTotal
        {
            get { return objPedido == null ? 0 : objPedido.ped_valorTotal; }
            set
            {
                if (objPedido.ped_valorTotal != value)
                {
                    objPedido.ped_valorTotal = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(250, ErrorMessage = "É permitido apenas 250 caracteres")]
        public string ped_observacao
        {
            get { return objPedido == null ? string.Empty : objPedido.ped_observacao; }
            set
            {
                if (objPedido.ped_observacao != value)
                {
                    objPedido.ped_observacao = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(250, ErrorMessage = "É permitido apenas 250 caracteres")]
        public string ped_motivoCancelamento
        {
            get { return objPedido == null ? string.Empty : objPedido.ped_motivoCancelamento; }
            set
            {
                if (objPedido.ped_motivoCancelamento != value)
                {
                    objPedido.ped_motivoCancelamento = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _strRelatorioTela;
        public string strRelatorioTela
        {
            get { return _strRelatorioTela; }
            set
            {
                _strRelatorioTela = value;
                RaisePropertyChanged("strRelatorioTela", false);
            }
        }

        public List<tbAuditoria> arrAuditoria { get; set; }

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

        private bool _blnTelefoneFocus;
        public bool blnTelefoneFocus
        {
            get { return _blnTelefoneFocus; }
            set
            {
                _blnTelefoneFocus = value;
                RaisePropertyChanged();
            }
        }

        private bool _blnNomeFocus;
        public bool blnNomeFocus
        {
            get { return _blnNomeFocus; }
            set
            {
                _blnNomeFocus = value;
                RaisePropertyChanged();
            }
        }

        private bool _blnFormaPagamentoFocus;
        public bool blnFormaPagamentoFocus
        {
            get { return _blnFormaPagamentoFocus; }
            set
            {
                _blnFormaPagamentoFocus = value;
                RaisePropertyChanged();
            }
        }

        private bool _blnValorRecebidoFocus;
        public bool blnValorRecebidoFocus
        {
            get { return _blnValorRecebidoFocus; }
            set
            {
                if (ped_valorRecebido == 0)
                    ped_valorRecebido = ped_valorTotal;
                _blnValorRecebidoFocus = value;
                RaisePropertyChanged();
            }
        }

        #endregion Propriedades



        #region Comandos

        private bool CanSalvar(object objParam)
        {
            return _objPedido != null;
        }
        private void Salvar(object objParam)
        {
            var focusedElement = Keyboard.FocusedElement as FrameworkElement;
            if (focusedElement is TextBox)
            {
                var expression = focusedElement.GetBindingExpression(TextBox.TextProperty);
                if (expression != null && expression.ParentBinding.UpdateSourceTrigger == System.Windows.Data.UpdateSourceTrigger.LostFocus)
                    expression.UpdateSource();
            }

            bool blnAchouErroProduto = false;
            foreach (EntregaPedidoProdutoViewModel objEntregaPedidoProdutoViewModel in arrEntregaPedidoProdutoViewModel)
            {
                objEntregaPedidoProdutoViewModel.Validate();
                blnAchouErroProduto = objEntregaPedidoProdutoViewModel.HasErrors;
                if (blnAchouErroProduto)
                    break;
            }

            bool blnAchouErroEndereco = false;
            foreach (EntregaPedidoEnderecoViewModel objEntregaPedidoEnderecoViewModel in arrEntregaPedidoEnderecoViewModel)
            {
                objEntregaPedidoEnderecoViewModel.Validate();
                blnAchouErroEndereco = objEntregaPedidoEnderecoViewModel.HasErrors;
                if (blnAchouErroEndereco)
                    break;
            }

            Validate();
            if (!HasErrors && !blnAchouErroProduto && !blnAchouErroEndereco)
            {
                bool blnContinuar = false;
                if(objPedido.ped_formaPagamentoTipo == (int)enFormaPagamentoTipo.Convenio && objPedido.ped_valorTotal > objPedido.tbCliente.cli_limiteCredito)
                {
                    if (MessageBox.Show("Limite de credito excedido, deseja continuar?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        blnContinuar = true;
                }
                else
                    blnContinuar = true;

                if (blnContinuar)
                {
                    int intImprimirCupom = objPedido.ped_codigo;
                    objPedido.tbPedidoProduto.Clear();
                    foreach (EntregaPedidoProdutoViewModel objEntregaPedidoProdutoViewModel in arrEntregaPedidoProdutoViewModel)
                        objPedido.tbPedidoProduto.Add(objEntregaPedidoProdutoViewModel.objPedidoProduto);

                    objPedido.tbCliente.tbClienteEndereco.Clear();
                    foreach (EntregaPedidoEnderecoViewModel objEntregaPedidoEnderecoViewModel in arrEntregaPedidoEnderecoViewModel)
                    {
                        objPedido.tbCliente.tbClienteEndereco.Add(objEntregaPedidoEnderecoViewModel.objClienteEndereco);
                        if (objEntregaPedidoEnderecoViewModel.blnSelecionado)
                        {
                            objPedido.ped_logradouro = objEntregaPedidoEnderecoViewModel.objClienteEndereco.cen_logradouro;
                            objPedido.ped_numero = objEntregaPedidoEnderecoViewModel.objClienteEndereco.cen_numero;
                            objPedido.ped_bairro = objEntregaPedidoEnderecoViewModel.objClienteEndereco.tbBairro.bai_nome;
                            objPedido.ped_cep = objEntregaPedidoEnderecoViewModel.objClienteEndereco.cen_cep;
                            objPedido.ped_complemento = objEntregaPedidoEnderecoViewModel.objClienteEndereco.cen_complemento;
                            objPedido.cid_codigo = objEntregaPedidoEnderecoViewModel.objClienteEndereco.cid_codigo;
                            objPedido.est_codigo = objEntregaPedidoEnderecoViewModel.objClienteEndereco.est_codigo;
                            objPedido.bai_codigo = objEntregaPedidoEnderecoViewModel.objClienteEndereco.bai_codigo;
                        }
                    }
                    objPedido.tbCliente.cli_nome = objPedido.ped_nomeCliente;

                    Retorno objRetorno;
                    using (var objBLL = new Pedidos())
                    {
                        objRetorno = objBLL.SalvarPedidoEntrega(objPedido, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        if (intImprimirCupom == 0)
                            ImprimirCupom((tbPedido)objRetorno.objRetorno);
                        objPedido = null;
                        ClearAllErrorsAsync();
                        Dispose();
                    }
                    else
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
            }
        }

        private void Cancelar(object objParam)
        {
            if (MessageBox.Show("Todas as alterações serão perdidas, deseja cancelar a edição?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                objPedido = null;
                ClearAllErrorsAsync();
                Dispose();
            }
        }

        private void Log(object objParam)
        {
            if (objParam != null)
            {
                if (objParam.ToString() == "AbrirTela")
                {
                    Retorno objRetorno;
                    using (Auditoria objBLL = new Auditoria())
                    {
                        objRetorno = objBLL.RetornaListaAuditoria(objPedido.ped_codigo, objPedido);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        arrAuditoria = (List<tbAuditoria>)objRetorno.objRetorno;
                        winAuditoria objTelaAuditoria = new winAuditoria();
                        objTelaAuditoria.Title = "Auditoria - " + base.strNomeTela;
                        objTelaAuditoria.DataContext = this;
                        objTelaAuditoria.Owner = (Window)Application.Current.MainWindow;
                        objTelaAuditoria.Closed += (sen, eve) =>
                        {
                            arrAuditoria = null;
                            objTelaAuditoria = null;
                        };
                        objTelaAuditoria.ShowDialog();
                    }
                    else
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
                else if (objParam.GetType() == typeof(winAuditoria))
                {
                    ((Window)objParam).Close();
                }
            }
        }
        
        private void Cliente(object objParam)
        {
            int intNumero;
            if (objParam != null)
            {
                if (objParam.GetType() == typeof(tbCliente))
                {
                    objPedido.tbCliente = (tbCliente)objParam;
                    objPedido.cli_codigo = objPedido.tbCliente.cli_codigo;
                    if (objPedido.tbCliente.cli_codigo > 0)
                    {
                        ObservableCollection<EntregaPedidoEnderecoViewModel> arrEntregaPedidoEnderecoViewModelAux = new ObservableCollection<EntregaPedidoEnderecoViewModel>();
                        foreach (tbClienteEndereco objClienteEndereco in objPedido.tbCliente.tbClienteEndereco)
                        {
                            EntregaPedidoEnderecoViewModel objEntregaPedidoEnderecoViewModel = new EntregaPedidoEnderecoViewModel(objClienteEndereco);
                            objEntregaPedidoEnderecoViewModel.OnDispose += ObjEntregaPedidoEnderecoViewModel_OnDispose;
                            objEntregaPedidoEnderecoViewModel.PropertyChanged += ObjEntregaPedidoEnderecoViewModel_PropertyChanged;
                            arrEntregaPedidoEnderecoViewModelAux.Add(objEntregaPedidoEnderecoViewModel);
                        }
                        _arrEntregaPedidoEnderecoViewModel = arrEntregaPedidoEnderecoViewModelAux;
                        _arrEntregaPedidoEnderecoViewModel.FirstOrDefault().blnSelecionado = true;

                        objPedido.ped_nomeCliente = objPedido.tbCliente.cli_nome;
                        if(objPedido.tbCliente.tbClienteTelefone.Where(ctl => ctl.ctl_numero == objPedido.ped_telefone).Count() == 0)
                            objPedido.ped_telefone = objPedido.tbCliente.tbClienteTelefone.FirstOrDefault().ctl_numero;
                    }
                    else
                    {
                        objPedido.tbCliente.cli_tipo = "F";
                        objPedido.tbCliente.cli_sexo = "M";
                        objPedido.tbCliente.cgr_codigo = FrameworkUtil.objConfigStorage.objConfiguracao.cgr_codigo;
                        
                        objPedido.tbCliente.tbClienteTelefone = new List<tbClienteTelefone>();
                        objPedido.tbCliente.tbClienteTelefone.Add(new tbClienteTelefone { ctl_numero = objPedido.ped_telefone });

                        objPedido.tbCliente.tbClienteEndereco = new List<tbClienteEndereco>();
                        tbClienteEndereco objClienteEndereco = new tbClienteEndereco();
                        objClienteEndereco.est_codigo = FrameworkUtil.objConfigStorage.objEmpresa.est_codigo;
                        objClienteEndereco.cid_codigo = FrameworkUtil.objConfigStorage.objEmpresa.cid_codigo;
                        objClienteEndereco.tbBairro = new tbBairro();
                        _arrEntregaPedidoEnderecoViewModel = new ObservableCollection<EntregaPedidoEnderecoViewModel>();
                        EntregaPedidoEnderecoViewModel objEntregaPedidoEnderecoViewModel = new EntregaPedidoEnderecoViewModel(objClienteEndereco);
                        objEntregaPedidoEnderecoViewModel.OnDispose += ObjEntregaPedidoEnderecoViewModel_OnDispose;
                        objEntregaPedidoEnderecoViewModel.PropertyChanged += ObjEntregaPedidoEnderecoViewModel_PropertyChanged;
                        objEntregaPedidoEnderecoViewModel.blnSelecionado = true;
                        _arrEntregaPedidoEnderecoViewModel.Add(objEntregaPedidoEnderecoViewModel);

                        objPedido.ped_nomeCliente = string.Empty;
                    }
                    RaisePropertyChanged("ped_telefone");
                    RaisePropertyChanged("ped_nomeCliente");
                    RaisePropertyChanged("arrEntregaPedidoEnderecoViewModel");
                }
                else if (objParam.ToString() == "Pesquisar")
                {
                    winCadastro objTelaCadastro = new winCadastro();
                    ClienteViewModel objClienteViewModel = new ClienteViewModel();
                    objClienteViewModel.OnDispose += (sen1, eve1) => { objTelaCadastro.Close(); };
                    objClienteViewModel.blnJanela = true;
                    objTelaCadastro.Title = "Cadastro - " + objClienteViewModel.strNomeTela;
                    objTelaCadastro.DataContext = objClienteViewModel;
                    objTelaCadastro.Owner = (Window)Application.Current.MainWindow;
                    objTelaCadastro.Closed += (sen, eve) =>
                    {
                        if(objClienteViewModel.objCliente != null)
                            Cliente(objClienteViewModel.objCliente.cli_codigo);
                        objClienteViewModel = null;
                        objTelaCadastro = null;
                    };
                    objTelaCadastro.ShowDialog();
                }
                else if (int.TryParse(objParam.ToString(), out intNumero))
                {
                    objPedido.ped_telefone = objParam.ToString();

                    Retorno objRetorno;
                    using (var objBLL = new Clientes())
                    {
                        objRetorno = objBLL.RetornaCliente(intNumero, null);
                    }
                    if (objRetorno.intCodigoErro == 0)
                        Cliente((tbCliente)objRetorno.objRetorno);
                    else
                    {
                        if (objRetorno.intCodigoErro == 48)
                            Cliente(new tbCliente());
                        else
                            MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                    } 
                }
                else
                    Cliente("Pesquisar");
            }
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
                        objPedido.fun_funcionarioEntregador = ((tbFuncionario)objParam).fun_codigo;
                        objPedido.tbFuncionarioEntregador.fun_nome = ((tbFuncionario)objParam).fun_nome;
                    }
                    else
                    {
                        objPedido.fun_funcionarioEntregador = null;
                        objPedido.tbFuncionarioEntregador.fun_nome = string.Empty;
                    }
                    RaisePropertyChanged("fun_funcionarioEntregador");
                    RaisePropertyChanged("fun_nomeEntregador");
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

        private void FormaPagamento(object objParam)
        {
            int intCodigo;
            if (objParam != null)
            {
                blnValorRecebidoFocus = false;
                if (objParam.GetType() == typeof(tbFormaPagamento))
                {
                    if (((tbFormaPagamento)objParam).fpg_codigo > 0)
                    {
                        objPedido.fpg_codigo = ((tbFormaPagamento)objParam).fpg_codigo;
                        objPedido.tbFormaPagamento.fpg_descricao = ((tbFormaPagamento)objParam).fpg_descricao;
                        objPedido.ped_cobranca = ((tbFormaPagamento)objParam).tbFormaPagamentoTipo.fpt_cobranca;
                        objPedido.ped_formaPagamentoTipo = ((tbFormaPagamento)objParam).fpt_codigo;
                        objPedido.ped_formaPagamentoDescricao = ((tbFormaPagamento)objParam).fpg_descricao;
                        _blnValorRecebidoFocus = true;
                    }
                    else
                    {
                        objPedido.fpg_codigo = 0;
                        objPedido.tbFormaPagamento.fpg_descricao = string.Empty;
                        objPedido.ped_cobranca = string.Empty;
                        objPedido.ped_formaPagamentoTipo = 0;
                        objPedido.ped_formaPagamentoDescricao = string.Empty;
                    }
                    RaisePropertyChanged("fpg_codigo");
                    RaisePropertyChanged("fpg_descricao");
                    RaisePropertyChanged("blnValorRecebidoFocus");
                }
                else if (objParam.ToString() == "Pesquisar")
                {
                    winCadastro objTelaCadastro = new winCadastro();
                    FormaPagamentoViewModel objFormaPagamentoViewModel = new FormaPagamentoViewModel();
                    objFormaPagamentoViewModel.OnDispose += (sen1, eve1) => { objTelaCadastro.Close(); };
                    objFormaPagamentoViewModel.blnJanela = true;
                    objTelaCadastro.Title = "Cadastro - " + objFormaPagamentoViewModel.strNomeTela;
                    objTelaCadastro.DataContext = objFormaPagamentoViewModel;
                    objTelaCadastro.Owner = (Window)Application.Current.MainWindow;
                    objTelaCadastro.Closed += (sen, eve) =>
                    {
                        FormaPagamento(objFormaPagamentoViewModel.objFormaPagamento);
                        objFormaPagamentoViewModel = null;
                        objTelaCadastro = null;
                    };
                    objTelaCadastro.ShowDialog();
                }
                else if (int.TryParse(objParam.ToString(), out intCodigo))
                {
                    Retorno objRetorno;
                    using (var objBLL = new FormaPagamento())
                    {
                        objRetorno = objBLL.RetornaFormaPagamento(intCodigo, null);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        FormaPagamento((tbFormaPagamento)objRetorno.objRetorno);
                    }
                    else
                    {
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                        FormaPagamento(new tbFormaPagamento());
                    }
                }
                else
                    FormaPagamento("Pesquisar");
            }
        }

        private void AdicionaProduto(object objParam)
        {
            tbPedidoProduto objPedidoProduto = new tbPedidoProduto();
            objPedidoProduto.tbProduto = new tbProduto();
            EntregaPedidoProdutoViewModel objEntregaPedidoProdutoViewModel = new EntregaPedidoProdutoViewModel(objPedidoProduto);
            objEntregaPedidoProdutoViewModel.blnCodigoFocus = true;
            objEntregaPedidoProdutoViewModel.OnDispose += objEntregaPedidoProdutoViewModel_OnDispose;
            objEntregaPedidoProdutoViewModel.PropertyChanged += objEntregaPedidoProdutoViewModel_PropertyChanged;
            arrEntregaPedidoProdutoViewModel.Add(objEntregaPedidoProdutoViewModel);
        }

        private void AdicionaEndereco(object objParam)
        {
            tbClienteEndereco objClienteEndereco = new tbClienteEndereco();
            objClienteEndereco.est_codigo = FrameworkUtil.objConfigStorage.objEmpresa.est_codigo;
            objClienteEndereco.cid_codigo = FrameworkUtil.objConfigStorage.objEmpresa.cid_codigo;
            objClienteEndereco.tbBairro = new tbBairro();
            EntregaPedidoEnderecoViewModel objEntregaPedidoEnderecoViewModel = new EntregaPedidoEnderecoViewModel(objClienteEndereco);
            if (arrEntregaPedidoEnderecoViewModel.Count() == 0)
                objEntregaPedidoEnderecoViewModel.blnSelecionado = true;
            objEntregaPedidoEnderecoViewModel.blnLogradouroFocus = true;
            objEntregaPedidoEnderecoViewModel.OnDispose += ObjEntregaPedidoEnderecoViewModel_OnDispose;
            objEntregaPedidoEnderecoViewModel.PropertyChanged += ObjEntregaPedidoEnderecoViewModel_PropertyChanged;
            arrEntregaPedidoEnderecoViewModel.Add(objEntregaPedidoEnderecoViewModel);
        }

        private void ImprimirCupom(object objParam)
        {
            try
            {
                if (objParam != null)
                {
                    Retorno objRetorno;
                    using (Relatorios objBLL = new Relatorios())
                    {
                        if(objParam.ToString() == "Tela")
                            objRetorno = objBLL.RetornaCupomEntrega(objPedido);
                        else
                            objRetorno = objBLL.RetornaCupomEntrega((tbPedido)objParam);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        string strConteudo = (string)objRetorno.objRetorno;
                        string[] arrLinhas = strConteudo.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        FrameworkUtil.objGerenciaCupom.AbreRelatorio();
                        for (int i = 0; i < arrLinhas.Length; i++)
                        {
                            FrameworkUtil.objGerenciaCupom.LinhaRelatorio(arrLinhas[i]);
                        }
                        if (objParam != null && objParam.ToString() == "Tela")
                            strRelatorioTela = FrameworkUtil.objGerenciaCupom.RetornaRelatorioTexto();
                        else
                        {
                            if (string.IsNullOrWhiteSpace(FrameworkUtil.objConfigStorage.objConfiguracao.cfg_impressoraEntrega))
                                MessageBox.Show("Impressora não configurada!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                            else
                                FrameworkUtil.objGerenciaCupom.FechaRelatorio(FrameworkUtil.objConfigStorage.objConfiguracao.cfg_impressoraEntrega);
                        }
                    }
                    else
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                arrCaixa = (List<tbCaixa>)objRetorno.objRetorno;
            else
                MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
        }

        public void PedidoFocus(object objParam)
        {
            if(objParam != null)
            {
                if(arrEntregaPedidoEnderecoViewModel.Count > 0)
                    arrEntregaPedidoEnderecoViewModel.FirstOrDefault().blnLogradouroFocus = false;
                if(arrEntregaPedidoProdutoViewModel.Count > 0)
                    arrEntregaPedidoProdutoViewModel.FirstOrDefault().blnCodigoFocus = false;
                blnTelefoneFocus = false;
                blnNomeFocus = false;
                blnFormaPagamentoFocus = false;

                if (objParam.ToString() == "Logradouro")
                {
                    if (arrEntregaPedidoEnderecoViewModel.Count > 0)
                        arrEntregaPedidoEnderecoViewModel.FirstOrDefault().blnLogradouroFocus = true;
                }
                    
                if (objParam.ToString() == "CodigoProduto")
                {
                    if (arrEntregaPedidoProdutoViewModel.Count > 0)
                        arrEntregaPedidoProdutoViewModel.FirstOrDefault().blnCodigoFocus = true;
                }
                    
                if (objParam.ToString() == "Telefone")
                    blnTelefoneFocus = true;
                if (objParam.ToString() == "Nome")
                    blnNomeFocus = true;
                if (objParam.ToString() == "FormaPagamento")
                    blnFormaPagamentoFocus = true;
            }
        }

        #endregion Comandos



        #region Eventos

        void objEntregaPedidoProdutoViewModel_OnDispose(object sender, EventArgs e)
        {
            arrEntregaPedidoProdutoViewModel.Remove((EntregaPedidoProdutoViewModel)sender);
            CalculaValores();
        }

        void objEntregaPedidoProdutoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ppr_quantidade")
                CalculaValores();
        }

        private void ObjEntregaPedidoEnderecoViewModel_OnDispose(object sender, EventArgs e)
        {
            arrEntregaPedidoEnderecoViewModel.Remove((EntregaPedidoEnderecoViewModel)sender);
            if (((EntregaPedidoEnderecoViewModel)sender).blnSelecionado == true && arrEntregaPedidoEnderecoViewModel.Count() > 0)
                arrEntregaPedidoEnderecoViewModel.FirstOrDefault().blnSelecionado = true;
        }

        private void ObjEntregaPedidoEnderecoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "bai_taxaEntrega")
            {
                if(arrEntregaPedidoEnderecoViewModel.Count > 0)
                    ped_valorTaxaEntrega = arrEntregaPedidoEnderecoViewModel.FirstOrDefault(cen => cen.blnSelecionado == true).bai_taxaEntrega;
            }
        }

        #endregion Eventos



        #region Métodos

        private void CalculaValores()
        {
            ped_valorSubTotal = arrEntregaPedidoProdutoViewModel.Sum(ppr => ppr.ppr_valorTotal);
            ped_valorTotal = (ped_valorSubTotal + ped_valorTaxaEntrega + ped_valorDespesa) - ped_valorDesconto;
            if ((ped_valorRecebido - ped_valorTotal) > 0)
                ped_valorTroco = ped_valorRecebido - ped_valorTotal;
            else
                ped_valorTroco = 0;
        }

        #endregion Métodos
    }
}