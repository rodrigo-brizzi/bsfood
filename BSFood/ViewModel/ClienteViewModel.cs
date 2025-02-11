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
    public class ClienteViewModel : TelaViewModel
    {
        public ICommand NavegarCommand { get; set; }
        public ICommand NovoCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand SalvarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand ExcluirCommand { get; set; }
        public ICommand PesquisarCommand { get; set; }
        public ICommand LogCommand { get; set; }
        public ICommand ClienteGrupoCommand { get; set; }
        public ICommand AdicionaEnderecoCommand { get; set; }
        public ICommand AdicionaTelefoneCommand { get; set; }
        public ICommand ClienteFocusCommand { get; set; }


        public ClienteViewModel()
        {
            NavegarCommand = new DelegateCommand(Navegar, CanNavegar);
            NovoCommand = new DelegateCommand(Novo, CanNovo);
            EditarCommand = new DelegateCommand(Editar, CanEditar);
            SalvarCommand = new DelegateCommand(Salvar, CanSalvar);
            CancelarCommand = new DelegateCommand(Cancelar, CanCancelar);
            ExcluirCommand = new DelegateCommand(Excluir, CanExcluir);
            PesquisarCommand = new DelegateCommand(Pesquisar, CanPesquisar);
            LogCommand = new DelegateCommand(Log, CanLog);
            ClienteGrupoCommand = new DelegateCommand(ClienteGrupo);
            AdicionaEnderecoCommand = new DelegateCommand(AdicionaEndereco);
            AdicionaTelefoneCommand = new DelegateCommand(AdicionaTelefone);
            ClienteFocusCommand = new DelegateCommand(ClienteFocus);
            Pesquisar(0);
        }


        #region Propriedades

        private string _strCliCodigoPesquisa;
        public string strCliCodigoPesquisa
        {
            get { return _strCliCodigoPesquisa; }
            set
            {
                _strCliCodigoPesquisa = value;
            }
        }

        private string _strCliNomePesquisa;
        public string strCliNomePesquisa
        {
            get { return _strCliNomePesquisa; }
            set
            {
                _strCliNomePesquisa = value;
            }
        }

        private string _strCliTelefonePesquisa;
        public string strCliTelefonePesquisa
        {
            get { return _strCliTelefonePesquisa; }
            set
            {
                _strCliTelefonePesquisa = value;
            }
        }

        [Required(ErrorMessage = "Nome do cliente obrigatório")]
        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string cli_nome
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_nome; }
            set
            {
                if (objCliente.cli_nome != value)
                {
                    objCliente.cli_nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string cli_nomeFantasia
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_nomeFantasia; }
            set
            {
                if (objCliente.cli_nomeFantasia != value)
                {
                    objCliente.cli_nomeFantasia = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Tipo do cliente obrigatório")]
        [StringLength(1, ErrorMessage = "É permitido apenas 1 caracter")]
        public string cli_tipo
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_tipo; }
            set
            {
                if (objCliente.cli_tipo != value)
                {
                    objCliente.cli_tipo = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string cli_cpfCnpj
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_cpfCnpj; }
            set
            {
                if (objCliente.cli_cpfCnpj != value)
                {
                    objCliente.cli_cpfCnpj = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string cli_rgIe
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_rgIe; }
            set
            {
                if (objCliente.cli_rgIe != value)
                {
                    objCliente.cli_rgIe = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Sexo do cliente obrigatório")]
        [StringLength(1, ErrorMessage = "É permitido apenas 1 caracter")]
        public string cli_sexo
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_sexo; }
            set
            {
                if (objCliente.cli_sexo != value)
                {
                    objCliente.cli_sexo = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string cli_email
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_email; }
            set
            {
                if (objCliente.cli_email != value)
                {
                    objCliente.cli_email = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(250, ErrorMessage = "É permitido apenas 250 caracteres")]
        public string cli_observacao
        {
            get { return objCliente == null ? string.Empty : objCliente.cli_observacao; }
            set
            {
                if (objCliente.cli_observacao != value)
                {
                    objCliente.cli_observacao = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Range(0, 31, ErrorMessage = "Dia de fechamento inválido")]
        public int cli_fechamento
        {
            get { return objCliente == null ? 0 : objCliente.cli_fechamento; }
            set
            {
                if (objCliente.cli_fechamento != value)
                {
                    objCliente.cli_fechamento = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Range(0, 31, ErrorMessage = "Dia de vencimento inválido")]
        public int cli_vencimento
        {
            get { return objCliente == null ? 0 : objCliente.cli_vencimento; }
            set
            {
                if (objCliente.cli_vencimento != value)
                {
                    objCliente.cli_vencimento = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Grupo de cliente obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Grupo de cliente obrigatório")]
        public int? cgr_codigo
        {
            get
            {
                if (objCliente == null || objCliente.cgr_codigo == 0)
                    return null;
                else
                    return objCliente.cgr_codigo;
            }
            set
            {
                if (objCliente.cgr_codigo != value)
                {
                    objCliente.cgr_codigo = value == null ? 0 : (int)value;
                    ClienteGrupo(objCliente.cgr_codigo);
                }
            }
        }

        public string cgr_nome
        {
            get { return objCliente == null ? string.Empty : objCliente.tbClienteGrupo.cgr_nome; }
            set
            {
                if (objCliente.tbClienteGrupo.cgr_nome != value)
                {
                    objCliente.tbClienteGrupo.cgr_nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        private tbCliente _objCliente;
        public tbCliente objCliente
        {
            get { return _objCliente; }
            set
            {
                _objCliente = value;
                //Prepara a coleção ClienteEndereco
                if (_objCliente != null)
                {
                    ObservableCollection<ClienteEnderecoViewModel> arrClienteEnderecoViewModelAux = new ObservableCollection<ClienteEnderecoViewModel>();
                    foreach (tbClienteEndereco objClienteEndereco in _objCliente.tbClienteEndereco)
                    {
                        ClienteEnderecoViewModel objClienteEnderecoViewModel = new ClienteEnderecoViewModel(objClienteEndereco);
                        objClienteEnderecoViewModel.OnDispose += objClienteEnderecoViewModel_OnDispose;
                        arrClienteEnderecoViewModelAux.Add(objClienteEnderecoViewModel);
                    }
                    _arrClienteEnderecoViewModel = arrClienteEnderecoViewModelAux;

                    ObservableCollection<ClienteTelefoneViewModel> arrClienteTelefoneViewModelAux = new ObservableCollection<ClienteTelefoneViewModel>();
                    foreach (tbClienteTelefone objClienteTelefone in _objCliente.tbClienteTelefone)
                    {
                        ClienteTelefoneViewModel objClienteTelefoneViewModel = new ClienteTelefoneViewModel(objClienteTelefone);
                        objClienteTelefoneViewModel.OnDispose += objClienteTelefoneViewModel_OnDispose;
                        arrClienteTelefoneViewModelAux.Add(objClienteTelefoneViewModel);
                    }
                    _arrClienteTelefoneViewModel = arrClienteTelefoneViewModelAux;
                }
                else
                {
                    _arrClienteEnderecoViewModel = null;
                    _arrClienteTelefoneViewModel = null;
                }
                _intSelectedIndexTab = 0;
                RaisePropertyChanged(null);
            }
        }

        private ObservableCollection<ClienteEnderecoViewModel> _arrClienteEnderecoViewModel;
        public ObservableCollection<ClienteEnderecoViewModel> arrClienteEnderecoViewModel
        {
            get { return _arrClienteEnderecoViewModel; }
            set
            {
                _arrClienteEnderecoViewModel = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<ClienteTelefoneViewModel> _arrClienteTelefoneViewModel;
        public ObservableCollection<ClienteTelefoneViewModel> arrClienteTelefoneViewModel
        {
            get { return _arrClienteTelefoneViewModel; }
            set
            {
                _arrClienteTelefoneViewModel = value;
                RaisePropertyChanged();
            }
        }

        private List<tbCliente> _arrClientePesquisa;
        public List<tbCliente> arrClientePesquisa
        {
            get { return _arrClientePesquisa; }
            set
            {
                _arrClientePesquisa = value;
                RaisePropertyChanged();
                if (_arrClientePesquisa.Count > 0)
                    base.intSelectedIndexGrid = 0;
            }
        }

        private int _intSelectedIndexTab;
        public int intSelectedIndexTab
        {
            get { return _intSelectedIndexTab; }
            set
            {
                _intSelectedIndexTab = value;
                RaisePropertyChanged();
            }
        }

        private bool _blnEmailFocus;
        public bool blnEmailFocus
        {
            get { return _blnEmailFocus; }
            set
            {
                _blnEmailFocus = value;
                RaisePropertyChanged();
            }
        }

        public List<tbAuditoria> arrAuditoria { get; set; }

        #endregion Propriedades



        #region Comandos

        private bool CanNavegar(object objParam)
        {
            return (base.enStatusTelaAtual == enStatusTela.Navegacao);
        }
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
            return base.enStatusTelaAtual == enStatusTela.Navegacao && base.blnPermiteInclusaoRegistro;
        }
        private void Novo(object objParam)
        {
            tbCliente objClienteAux = new tbCliente();
            objClienteAux.cli_tipo = "F";
            objClienteAux.cli_sexo = "M";
            objClienteAux.tbClienteGrupo = new tbClienteGrupo();
            objClienteAux.tbClienteEndereco = new List<tbClienteEndereco>();
            objClienteAux.tbClienteTelefone = new List<tbClienteTelefone>();

            tbClienteEndereco objClienteEndereco = new tbClienteEndereco();
            objClienteEndereco.est_codigo = FrameworkUtil.objConfigStorage.objEmpresa.est_codigo;
            objClienteEndereco.cid_codigo = FrameworkUtil.objConfigStorage.objEmpresa.cid_codigo;
            objClienteEndereco.tbBairro = new tbBairro();
            objClienteAux.tbClienteEndereco.Add(objClienteEndereco);

            tbClienteTelefone objClienteTelefone = new tbClienteTelefone();
            objClienteAux.tbClienteTelefone.Add(objClienteTelefone);

            objCliente = objClienteAux;
            base.enStatusTelaAtual = enStatusTela.EmInclusaoOuAlteracao;
        }

        private bool CanEditar(object objParam)
        {
            return (base.enStatusTelaAtual == enStatusTela.Navegacao && base.blnPermiteAlteracaoRegistro);
        }
        private void Editar(object objParam)
        {
            if (objParam != null)
            {
                Retorno objRetorno;
                using (var objBLL = new Clientes())
                {
                    objRetorno = objBLL.RetornaCliente((int)objParam, null);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    objCliente = (tbCliente)objRetorno.objRetorno;
                    base.enStatusTelaAtual = enStatusTela.EmInclusaoOuAlteracao;
                }
                else
                {
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
            }
        }

        private bool CanSalvar(object objParam)
        {
            return base.enStatusTelaAtual == enStatusTela.EmInclusaoOuAlteracao;
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

            bool blnAchouErro = false;
            foreach (ClienteEnderecoViewModel objClienteEnderecoViewModel in arrClienteEnderecoViewModel)
            {
                objClienteEnderecoViewModel.Validate();
                blnAchouErro = objClienteEnderecoViewModel.HasErrors;
                if (blnAchouErro)
                    break;
            }
            if (!blnAchouErro)
            {
                foreach (ClienteTelefoneViewModel objClienteTelefoneViewModel in arrClienteTelefoneViewModel)
                {
                    objClienteTelefoneViewModel.Validate();
                    blnAchouErro = objClienteTelefoneViewModel.HasErrors;
                    if (blnAchouErro)
                        break;
                }
            }

            Validate();
            if (!HasErrors && !blnAchouErro)
            {
                objCliente.tbClienteEndereco.Clear();
                foreach (ClienteEnderecoViewModel objClienteEnderecoViewModel in arrClienteEnderecoViewModel)
                    objCliente.tbClienteEndereco.Add(objClienteEnderecoViewModel.objClienteEndereco);

                objCliente.tbClienteTelefone.Clear();
                foreach (ClienteTelefoneViewModel objClienteTelefoneViewModel in arrClienteTelefoneViewModel)
                    objCliente.tbClienteTelefone.Add(objClienteTelefoneViewModel.objClienteTelefone);

                Retorno objRetorno;
                using (var objBLL = new Clientes())
                {
                    objRetorno = objBLL.SalvarCliente(objCliente, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    objCliente = null;
                    ClearAllErrorsAsync();
                    base.enStatusTelaAtual = enStatusTela.Navegacao;
                    Pesquisar(null);
                }
                else
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
            }
        }

        private bool CanCancelar(object objParam)
        {
            return base.enStatusTelaAtual == enStatusTela.EmInclusaoOuAlteracao;
        }
        private void Cancelar(object objParam)
        {
            if (MessageBox.Show("Todas as alterações serão perdidas, deseja cancelar a edição?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                objCliente = null;
                ClearAllErrorsAsync();
                base.enStatusTelaAtual = enStatusTela.Navegacao;
                Pesquisar(null);
            }
        }

        private bool CanExcluir(object objParam)
        {
            return (base.enStatusTelaAtual == enStatusTela.Navegacao && base.blnPermiteExclusaoRegistro);
        }
        private void Excluir(object objParam)
        {
            if (objParam != null)
            {
                if (MessageBox.Show("Tem Certeza que deseja excluir o registro selecionado?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Retorno objRetorno;
                    using (var objBLL = new Clientes())
                    {
                        objRetorno = objBLL.ExcluirCliente((int)objParam);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        objCliente = null;
                        ClearAllErrorsAsync();
                        base.enStatusTelaAtual = enStatusTela.Navegacao;
                        Pesquisar(null);
                    }
                    else
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                }
            }
        }

        private bool CanPesquisar(object objParam)
        {
            return base.enStatusTelaAtual == enStatusTela.Navegacao;
        }
        public void Pesquisar(object objParam)
        {
            if (objParam != null && objParam.GetType() == typeof(tbCliente))
            {
                if (base.blnJanela)
                {
                    _objCliente = (tbCliente)objParam;
                    Dispose();
                }
            }
            else
            {
                int intSkip;
                if (objParam == null || !int.TryParse(objParam.ToString(), out intSkip))
                    intSkip = 0;

                Retorno objRetorno;
                using (var objBLL = new Clientes())
                {
                    objRetorno = objBLL.RetornaListaCliente(strCliCodigoPesquisa, strCliNomePesquisa, strCliTelefonePesquisa, intSkip, base.intQtdeRegPagina);
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
                    arrClientePesquisa = (List<tbCliente>)objRetorno.objRetorno;
                    if (arrClientePesquisa.Count() == 0)
                    {
                        base.intTotalPagina = 1;
                        base.intPaginaAtual = 1;
                        base.intQtdeReg = 0;
                    }
                }
                else
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
            }
        }

        private bool CanLog(object objParam)
        {
            return base.enStatusTelaAtual == enStatusTela.EmInclusaoOuAlteracao && objCliente != null && objCliente.cli_codigo > 0;
        }
        private void Log(object objParam)
        {
            if (objParam != null)
            {
                if (objParam.ToString() == "AbrirTela")
                {
                    Retorno objRetorno;
                    using (var objBll = new Auditoria())
                    {
                        objRetorno = objBll.RetornaListaAuditoria(objCliente.cli_codigo, objCliente);
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

        private void ClienteGrupo(object objParam)
        {
            int intCodigo;
            if (objParam != null)
            {
                blnEmailFocus = false;
                if (objParam.GetType() == typeof(tbClienteGrupo))
                {
                    if (((tbClienteGrupo)objParam).cgr_codigo > 0)
                    {
                        objCliente.cgr_codigo = ((tbClienteGrupo)objParam).cgr_codigo;
                        objCliente.tbClienteGrupo.cgr_nome = ((tbClienteGrupo)objParam).cgr_nome;
                        _blnEmailFocus = true;
                    }
                    else
                    {
                        objCliente.cgr_codigo = 0;
                        objCliente.tbClienteGrupo.cgr_nome = string.Empty;
                    }
                    RaisePropertyChanged("cgr_codigo");
                    RaisePropertyChanged("cgr_nome");
                    RaisePropertyChanged("blnEmailFocus");
                }
                else if (objParam.ToString() == "Pesquisar")
                {
                    winCadastro objTelaCadastro = new winCadastro();
                    ClienteGrupoViewModel objClienteGrupoViewModel = new ClienteGrupoViewModel();
                    objClienteGrupoViewModel.OnDispose += (sen1, eve1) => { objTelaCadastro.Close(); };
                    objClienteGrupoViewModel.blnJanela = true;
                    objTelaCadastro.Title = "Cadastro - " + objClienteGrupoViewModel.strNomeTela;
                    objTelaCadastro.DataContext = objClienteGrupoViewModel;
                    objTelaCadastro.Owner = (Window)Application.Current.MainWindow;
                    objTelaCadastro.Closed += (sen, eve) =>
                    {
                        ClienteGrupo(objClienteGrupoViewModel.objClienteGrupo);
                        objClienteGrupoViewModel = null;
                        objTelaCadastro = null;
                    };
                    objTelaCadastro.ShowDialog();
                }
                else if (int.TryParse(objParam.ToString(), out intCodigo))
                {
                    Retorno objRetorno;
                    using (var objBLL = new ClienteGrupos())
                    {
                        objRetorno = objBLL.RetornaClienteGrupo(intCodigo, null);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        ClienteGrupo((tbClienteGrupo)objRetorno.objRetorno);
                    }
                    else
                    {
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                        ClienteGrupo(new tbClienteGrupo());
                    }
                }
                else
                    ClienteGrupo("Pesquisar");
            }
        }

        private void AdicionaEndereco(object objParam)
        {
            tbClienteEndereco objClienteEndereco = new tbClienteEndereco();
            objClienteEndereco.est_codigo = FrameworkUtil.objConfigStorage.objEmpresa.est_codigo;
            objClienteEndereco.cid_codigo = FrameworkUtil.objConfigStorage.objEmpresa.cid_codigo;
            objClienteEndereco.tbBairro = new tbBairro();
            ClienteEnderecoViewModel objClienteEnderecoViewModel = new ClienteEnderecoViewModel(objClienteEndereco);
            objClienteEnderecoViewModel.blnCepFocus = true;
            objClienteEnderecoViewModel.OnDispose += objClienteEnderecoViewModel_OnDispose;
            arrClienteEnderecoViewModel.Add(objClienteEnderecoViewModel);
        }

        private void AdicionaTelefone(object objParam)
        {
            ClienteTelefoneViewModel objClienteTelefoneViewModel = new ClienteTelefoneViewModel(new tbClienteTelefone());
            objClienteTelefoneViewModel.blnTelefoneFocus = true;
            objClienteTelefoneViewModel.OnDispose += objClienteTelefoneViewModel_OnDispose;
            arrClienteTelefoneViewModel.Add(objClienteTelefoneViewModel);
        }

        public void ClienteFocus(object objParam)
        {
            if (objParam != null)
            {
                if (arrClienteEnderecoViewModel.Count > 0)
                    arrClienteEnderecoViewModel.FirstOrDefault().blnCepFocus = false;
                blnEmailFocus = false;

                if (objParam.ToString() == "Cep")
                {
                    if (arrClienteEnderecoViewModel.Count > 0)
                        arrClienteEnderecoViewModel.FirstOrDefault().blnCepFocus = true;
                }
                if (objParam.ToString() == "Email")
                    blnEmailFocus = true;
            }
        }

        #endregion Comandos



        #region Eventos

        void objClienteEnderecoViewModel_OnDispose(object sender, EventArgs e)
        {
            arrClienteEnderecoViewModel.Remove((ClienteEnderecoViewModel)sender);
        }

        void objClienteTelefoneViewModel_OnDispose(object sender, EventArgs e)
        {
            arrClienteTelefoneViewModel.Remove((ClienteTelefoneViewModel)sender);
        }

        #endregion Eventos



        #region Métodos



        #endregion Métodos
    }
}