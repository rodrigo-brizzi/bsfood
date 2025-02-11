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
    public class CaixaAberturaViewModel : ViewModelBase
    {
        public ICommand SalvarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand FuncionarioCommand { get; set; }
        public ICommand AdicionaCaixaMovimentoCommand { get; set; }
        public ICommand CaixaAberturaFocusCommand { get; set; }

        public CaixaAberturaViewModel()
        {
            SalvarCommand = new DelegateCommand(Salvar, CanSalvar);
            CancelarCommand = new DelegateCommand(Cancelar);
            FuncionarioCommand = new DelegateCommand(Funcionario);
            AdicionaCaixaMovimentoCommand = new DelegateCommand(AdicionaCaixaMovimento);
            CaixaAberturaFocusCommand = new DelegateCommand(CaixaAberturaFocus);
        }


        #region Propriedades

        [Required(ErrorMessage = "Funcionário obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Funcionário obrigatório")]
        public int? fun_codigo
        {
            get
            {
                if (objCaixa == null || objCaixa.fun_codigo == 0)
                    return null;
                else
                    return objCaixa.fun_codigo;
            }
            set
            {
                if (objCaixa.fun_codigo != value)
                {
                    objCaixa.fun_codigo = value == null ? 0 : (int)value;
                    Funcionario(objCaixa.fun_codigo);
                }
            }
        }

        public string fun_nome
        {
            get { return objCaixa == null ? string.Empty : objCaixa.tbFuncionario.fun_nome; }
            set
            {
                if (objCaixa.tbFuncionario.fun_nome != value)
                {
                    objCaixa.tbFuncionario.fun_nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(250, ErrorMessage = "É permitido apenas 250 caracteres")]
        public string cai_observacao
        {
            get { return objCaixa == null ? string.Empty : objCaixa.cai_observacao; }
            set
            {
                if (objCaixa.cai_observacao != value)
                {
                    objCaixa.cai_observacao = value;
                    RaisePropertyChanged();
                }
            }
        }

        private decimal _cai_valorTotal;
        public decimal cai_valorTotal
        {
            get { return _cai_valorTotal; }
            set
            {
                _cai_valorTotal = value;
                RaisePropertyChanged();
            }
        }

        private tbCaixa _objCaixa;
        public tbCaixa objCaixa
        {
            get { return _objCaixa; }
            set
            {
                _objCaixa = value;
                //Prepara a coleção CaixaMovimento
                if (_objCaixa != null)
                {
                    ObservableCollection<CaixaMovimentoViewModel> arrCaixaMovimentoViewModelAux = new ObservableCollection<CaixaMovimentoViewModel>();
                    foreach (tbCaixaMovimento objCaixaMovimento in _objCaixa.tbCaixaMovimento)
                    {
                        CaixaMovimentoViewModel objCaixaMovimentoViewModel = new CaixaMovimentoViewModel(objCaixaMovimento);
                        objCaixaMovimentoViewModel.OnDispose += objCaixaMovimentoViewModel_OnDispose;
                        objCaixaMovimentoViewModel.PropertyChanged += ObjCaixaMovimentoViewModel_PropertyChanged;
                        arrCaixaMovimentoViewModelAux.Add(objCaixaMovimentoViewModel);
                    }
                    _arrCaixaMovimentoViewModel = arrCaixaMovimentoViewModelAux;
                }
                else
                    _arrCaixaMovimentoViewModel = null;

                //Prepara propriedades da viewmodel
                RaisePropertyChanged(null);
            }
        }

        private ObservableCollection<CaixaMovimentoViewModel> _arrCaixaMovimentoViewModel;
        public ObservableCollection<CaixaMovimentoViewModel> arrCaixaMovimentoViewModel
        {
            get { return _arrCaixaMovimentoViewModel; }
            set
            {
                _arrCaixaMovimentoViewModel = value;
                RaisePropertyChanged();
            }
        }

        private bool _blnObservacaoFocus;
        public bool blnObservacaoFocus
        {
            get { return _blnObservacaoFocus; }
            set
            {
                _blnObservacaoFocus = value;
                RaisePropertyChanged();
            }
        }


        #endregion Propriedades



        #region Comandos

        private bool CanSalvar(object objParam)
        {
            return _objCaixa != null;
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

            Validate();
            if (!HasErrors)
            {
                Retorno objRetorno;
                using (var objBLL = new Caixa())
                {
                    objRetorno = objBLL.AbrirCaixa(objCaixa, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    objCaixa = null;
                    ClearAllErrorsAsync();
                    Dispose();
                }
                else
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
            }
        }

        private void Cancelar(object objParam)
        {
            if (MessageBox.Show("Todas as alterações serão perdidas, deseja cancelar a edição?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                objCaixa = null;
                ClearAllErrorsAsync();
                Dispose();
            }
        }

        private void Funcionario(object objParam)
        {
            int intCodigo;
            if (objParam != null)
            {
                blnObservacaoFocus = false;
                if (objParam.GetType() == typeof(tbFuncionario))
                {
                    if (((tbFuncionario)objParam).fun_codigo > 0)
                    {
                        objCaixa.fun_codigo = ((tbFuncionario)objParam).fun_codigo;
                        objCaixa.tbFuncionario.fun_nome = ((tbFuncionario)objParam).fun_nome;
                        _blnObservacaoFocus = true;
                    }
                    else
                    {
                        objCaixa.fun_codigo = 0;
                        objCaixa.tbFuncionario.fun_nome = string.Empty;
                    }
                    RaisePropertyChanged("fun_codigo");
                    RaisePropertyChanged("fun_nome");
                    RaisePropertyChanged("blnObservacaoFocus");
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
                        Funcionario(objFuncionarioViewModel.objFuncionario);
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
                        Funcionario((tbFuncionario)objRetorno.objRetorno);
                    }
                    else
                    {
                        MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
                        Funcionario(new tbFuncionario());
                    }
                }
                else
                    Funcionario("Pesquisar");
            }
        }

        private void AdicionaCaixaMovimento(object objParam)
        {
            tbCaixaMovimento objCaixaMovimento = new tbCaixaMovimento();
            objCaixaMovimento.tbFormaPagamento = new tbFormaPagamento();
            CaixaMovimentoViewModel objCaixaMovimentoViewModel = new CaixaMovimentoViewModel(objCaixaMovimento);
            objCaixaMovimentoViewModel.blnCodigoFocus = true;
            objCaixaMovimentoViewModel.OnDispose += objCaixaMovimentoViewModel_OnDispose;
            objCaixaMovimentoViewModel.PropertyChanged += ObjCaixaMovimentoViewModel_PropertyChanged;
            arrCaixaMovimentoViewModel.Add(objCaixaMovimentoViewModel);
        }

        public void CaixaAberturaFocus(object objParam)
        {
            if (objParam != null)
            {
                if (arrCaixaMovimentoViewModel.Count > 0)
                    arrCaixaMovimentoViewModel.FirstOrDefault().blnCodigoFocus = false;

                if (objParam.ToString() == "CodigoForma")
                {
                    if (arrCaixaMovimentoViewModel.Count > 0)
                        arrCaixaMovimentoViewModel.FirstOrDefault().blnCodigoFocus = true;
                }
            }
        }

        #endregion Comandos



        #region Eventos

        void objCaixaMovimentoViewModel_OnDispose(object sender, EventArgs e)
        {
            arrCaixaMovimentoViewModel.Remove((CaixaMovimentoViewModel)sender);
            cai_valorTotal = arrCaixaMovimentoViewModel.Sum(caim => caim.objCaixaMovimento.caim_valor);
        }

        private void ObjCaixaMovimentoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "caim_valor")
                cai_valorTotal = arrCaixaMovimentoViewModel.Sum(caim => caim.objCaixaMovimento.caim_valor);
        }

        #endregion Eventos



        #region Métodos

        public void CarregaFormaPagamento()
        {
            Retorno objRetorno;
            _objCaixa = new tbCaixa();
            _objCaixa.tbFuncionario = new tbFuncionario();
            _objCaixa.tbCaixaMovimento = new List<tbCaixaMovimento>();
            using (var objBLL = new FormaPagamento())
            {
                objRetorno = objBLL.RetornaListaFormaPagamento();
            }
            if (objRetorno.intCodigoErro == 0)
            {
                _arrCaixaMovimentoViewModel = new ObservableCollection<CaixaMovimentoViewModel>();
                List<tbFormaPagamento> arrFormaPagamento = (List<tbFormaPagamento>)objRetorno.objRetorno;
                if (arrFormaPagamento.Count > 0)
                {
                    foreach (tbFormaPagamento objFormaPagamento in arrFormaPagamento)
                    {
                        tbCaixaMovimento objCaixaMovimento = new tbCaixaMovimento();
                        objCaixaMovimento.tbFormaPagamento = objFormaPagamento;
                        objCaixaMovimento.fpg_codigo = objFormaPagamento.fpg_codigo;
                        _objCaixa.tbCaixaMovimento.Add(objCaixaMovimento);

                        CaixaMovimentoViewModel objCaixaMovimentoViewModel = new CaixaMovimentoViewModel(objCaixaMovimento);
                        objCaixaMovimentoViewModel.OnDispose += objCaixaMovimentoViewModel_OnDispose;
                        objCaixaMovimentoViewModel.PropertyChanged += ObjCaixaMovimentoViewModel_PropertyChanged;
                        _arrCaixaMovimentoViewModel.Add(objCaixaMovimentoViewModel);
                    }
                }
                RaisePropertyChanged(null);
            }
            else
                MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
        }

        #endregion Métodos
    }
}