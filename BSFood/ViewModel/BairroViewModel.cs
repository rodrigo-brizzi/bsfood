﻿using BSFood.View;
using BSFood.Apoio;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using BSFoodFramework.Models;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.BusinessLogic;
using BSFoodFramework.Apoio;

namespace BSFood.ViewModel
{
    public class BairroViewModel : TelaViewModel
    {
        public ICommand NavegarCommand { get; set; }
        public ICommand NovoCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand SalvarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        public ICommand ExcluirCommand { get; set; }
        public ICommand PesquisarCommand { get; set; }
        public ICommand LogCommand { get; set; }

        public BairroViewModel()
        {
            NavegarCommand = new DelegateCommand(Navegar, CanNavegar);
            NovoCommand = new DelegateCommand(Novo, CanNovo);
            EditarCommand = new DelegateCommand(Editar, CanEditar);
            SalvarCommand = new DelegateCommand(Salvar, CanSalvar);
            CancelarCommand = new DelegateCommand(Cancelar, CanCancelar);
            ExcluirCommand = new DelegateCommand(Excluir, CanExcluir);
            PesquisarCommand = new DelegateCommand(Pesquisar, CanPesquisar);
            LogCommand = new DelegateCommand(Log, CanLog);
            Pesquisar(0);
        }


        #region Propriedades

        private string _strBaiCodigoPesquisa;
        public string strBaiCodigoPesquisa
        {
            get { return _strBaiCodigoPesquisa; }
            set
            {
                _strBaiCodigoPesquisa = value;
            }
        }

        private string _strBaiNomePesquisa;
        public string strBaiNomePesquisa
        {
            get { return _strBaiNomePesquisa; }
            set
            {
                _strBaiNomePesquisa = value;
            }
        }

        [Required(ErrorMessage = "Nome do Bairro obrigatório")]
        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string bai_nome
        {
            get { return objBairro == null ? string.Empty : objBairro.bai_nome; }
            set
            {
                if (objBairro.bai_nome != value)
                {
                    objBairro.bai_nome = value;
                    RaisePropertyChanged();
                }
            }
        }      

        private tbBairro _objBairro;
        public tbBairro objBairro
        {
            get { return _objBairro; }
            set
            {
                _objBairro = value;
                RaisePropertyChanged(null);
            }
        }

        private List<tbBairro> _arrBairroPesquisa;
        public List<tbBairro> arrBairroPesquisa
        {
            get { return _arrBairroPesquisa; }
            set
            {
                _arrBairroPesquisa = value;
                RaisePropertyChanged();
                if (_arrBairroPesquisa.Count > 0)
                    base.intSelectedIndexGrid = 0;
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
            if(objParam != null)
            {
                if(objParam.ToString() == "Proximo")
                {
                    if(base.intPaginaAtual < base.intTotalPagina)
                    {
                        Pesquisar(base.intPaginaAtual * base.intQtdeRegPagina);
                        base.intPaginaAtual++;
                    }
                }
                else if(objParam.ToString() == "Anterior")
                {
                    if(base.intPaginaAtual > 1)
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
            tbBairro objBairroAux = new tbBairro();
            objBairro = objBairroAux;
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
                using (var objBLL = new Bairros())
                {
                    objRetorno = objBLL.RetornaBairro((int)objParam, null);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    objBairro = (tbBairro)objRetorno.objRetorno;
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

            Validate();
            if (!HasErrors)
            {
                Retorno objRetorno;
                using (var objBLL = new Bairros())
                {
                    objRetorno = objBLL.SalvarBairro(objBairro, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    objBairro = null;
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
                objBairro = null;
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
                    using (var objBLL = new Bairros())
                    {
                        objRetorno = objBLL.ExcluirBairro((int)objParam);
                    }
                    if (objRetorno.intCodigoErro == 0)
                    {
                        objBairro = null;
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
            if(objParam != null && objParam.GetType() == typeof(tbBairro))
            {
                if (base.blnJanela)
                {
                    _objBairro = (tbBairro)objParam;
                    Dispose();
                }
            }
            else
            {
                int intSkip;
                if (objParam == null || !int.TryParse(objParam.ToString(), out intSkip))
                    intSkip = 0;

                Retorno objRetorno;
                using (var objBLL = new Bairros())
                {
                    objRetorno = objBLL.RetornaListaBairro(strBaiCodigoPesquisa, strBaiNomePesquisa, intSkip, base.intQtdeRegPagina);
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
                    arrBairroPesquisa = (List<tbBairro>)objRetorno.objRetorno;
                    if(arrBairroPesquisa.Count() == 0)
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
            return base.enStatusTelaAtual == enStatusTela.EmInclusaoOuAlteracao && objBairro != null && objBairro.bai_codigo > 0;
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
                        objRetorno = objBll.RetornaListaAuditoria(objBairro.bai_codigo, objBairro);
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

        #endregion Comandos



        #region Eventos



        #endregion Eventos



        #region Métodos



        #endregion Métodos
    }
}