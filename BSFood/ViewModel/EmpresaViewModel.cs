﻿using BSFood.View;
using BSFood.Apoio;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using Microsoft.Win32;
using System.IO;
using BSFoodFramework.Models;
using BSFoodFramework.DataTransfer;
using BSFoodFramework.BusinessLogic;
using BSFoodFramework.Apoio;

namespace BSFood.ViewModel
{
    public class EmpresaViewModel : TelaViewModel
    {
        public ICommand SalvarCommand { get; set; }
        public ICommand LogCommand { get; set; }
        public ICommand CidadeCommand { get; set; }
        public ICommand NovaLogoCommand { get; set; }
        public ICommand NenhumaLogoCommand { get; set; }

        public EmpresaViewModel()
        {
            SalvarCommand = new DelegateCommand(Salvar);
            LogCommand = new DelegateCommand(Log);
            CidadeCommand = new DelegateCommand(Cidade);
            NovaLogoCommand = new DelegateCommand(NovaLogo);
            NenhumaLogoCommand = new DelegateCommand(NenhumaLogo);
            CarregaComboEstado();
            objEmpresa = FrameworkUtil.objConfigStorage.objEmpresa;
        }


        #region Propriedades

        [Required(ErrorMessage = "Nome da Empresa obrigatório")]
        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string emp_nome
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_nome; }
            set
            {
                if (objEmpresa.emp_nome != value)
                {
                    objEmpresa.emp_nome = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Nome fantasia da Empresa obrigatório")]
        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string emp_nomeFantasia
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_nomeFantasia; }
            set
            {
                if (objEmpresa.emp_nomeFantasia != value)
                {
                    objEmpresa.emp_nomeFantasia = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string emp_cnpj
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_cnpj; }
            set
            {
                if (objEmpresa.emp_cnpj != value)
                {
                    objEmpresa.emp_cnpj = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string emp_ie
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_ie; }
            set
            {
                if (objEmpresa.emp_ie != value)
                {
                    objEmpresa.emp_ie = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string emp_im
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_im; }
            set
            {
                if (objEmpresa.emp_im != value)
                {
                    objEmpresa.emp_im = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(256, ErrorMessage = "É permitido apenas 256 caracteres")]
        public string emp_assinaturaSat
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_assinaturaSat; }
            set
            {
                if (objEmpresa.emp_assinaturaSat != value)
                {
                    objEmpresa.emp_assinaturaSat = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(200, ErrorMessage = "É permitido apenas 200 caracteres")]
        public string emp_logradouro
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_logradouro; }
            set
            {
                if (objEmpresa.emp_logradouro != value)
                {
                    objEmpresa.emp_logradouro = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(10, ErrorMessage = "É permitido apenas 10 caracteres")]
        public string emp_numero
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_numero; }
            set
            {
                if (objEmpresa.emp_numero != value)
                {
                    objEmpresa.emp_numero = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(100, ErrorMessage = "É permitido apenas 100 caracteres")]
        public string emp_bairro
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_bairro; }
            set
            {
                if (objEmpresa.emp_bairro != value)
                {
                    objEmpresa.emp_bairro = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(9, ErrorMessage = "É permitido apenas 9 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string emp_cep
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_cep; }
            set
            {
                if (objEmpresa.emp_cep != value)
                {
                    objEmpresa.emp_cep = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(150, ErrorMessage = "É permitido apenas 150 caracteres")]
        public string emp_complemento
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_complemento; }
            set
            {
                if (objEmpresa.emp_complemento != value)
                {
                    objEmpresa.emp_complemento = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string emp_telefone
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_telefone; }
            set
            {
                if (objEmpresa.emp_telefone != value)
                {
                    objEmpresa.emp_telefone = value;
                    RaisePropertyChanged();
                }
            }
        }

        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe apenas números")]
        public string emp_fax
        {
            get { return objEmpresa == null ? string.Empty : objEmpresa.emp_fax; }
            set
            {
                if (objEmpresa.emp_fax != value)
                {
                    objEmpresa.emp_fax = value;
                    RaisePropertyChanged();
                }
            }
        }

        public byte[] emp_logo
        {
            get { return objEmpresa == null ? null : objEmpresa.emp_logo; }
            set
            {
                if (objEmpresa.emp_logo != value)
                {
                    objEmpresa.emp_logo = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Estado obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Estado obrigatório")]
        public int est_codigo
        {
            get { return objEmpresa == null ? 0 : objEmpresa.est_codigo; }
            set
            {
                if (objEmpresa.est_codigo != value)
                {
                    objEmpresa.est_codigo = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Cidade obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Cidade obrigatório")]
        public int cid_codigo
        {
            get
            {
                if (objEmpresa == null)
                    return 0;
                else
                {
                    RaisePropertyChanged("est_codigo", false);
                    return objEmpresa.cid_codigo;
                }
            }
            set
            {
                if (objEmpresa.cid_codigo != value)
                {
                    objEmpresa.cid_codigo = value;
                    RaisePropertyChanged();
                }
            }
        }

        private tbEmpresa _objEmpresa;
        public tbEmpresa objEmpresa
        {
            get { return _objEmpresa; }
            set
            {
                _objEmpresa = value;
                RaisePropertyChanged(null);
            }
        }

        private List<tbEstado> _arrEstado;
        public List<tbEstado> arrEstado
        {
            get { return _arrEstado; }
            set
            {
                _arrEstado = value;
                RaisePropertyChanged("arrEstado", false);
            }
        }

        public List<tbAuditoria> arrAuditoria { get; set; }

        #endregion Propriedades



        #region Comandos

        private void Salvar(object objParam)
        {
            Validate();
            if (!HasErrors)
            {
                Retorno objRetorno;
                using (var objBLL = new Empresa())
                {
                    objRetorno = objBLL.SalvarEmpresa(objEmpresa, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    MessageBox.Show("A empresa do sistema foi alterada, será necessário fechar o sistema!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Util.FecharSistema();
                }
                else
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
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
                        objRetorno = objBLL.RetornaListaAuditoria(objEmpresa.emp_codigo, objEmpresa);
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

        private void Cidade(object objParam)
        {
            winCadastro objTelaCadastro = new winCadastro();
            CidadeViewModel objCidadeViewModel = new CidadeViewModel();
            objCidadeViewModel.OnDispose += (sen1, eve1) => { objTelaCadastro.Close(); };
            objCidadeViewModel.blnJanela = true;
            objTelaCadastro.Title = "Cadastro - " + objCidadeViewModel.strNomeTela;
            objTelaCadastro.DataContext = objCidadeViewModel;
            objTelaCadastro.Owner = (Window)Application.Current.MainWindow;
            objTelaCadastro.Closed += (sen, eve) =>
            {
                if (objCidadeViewModel.objCidade != null)
                {
                    CarregaComboEstado();
                    objEmpresa.est_codigo = (int)objCidadeViewModel.objCidade.est_codigo;
                    RaisePropertyChanged("est_codigo");
                    objEmpresa.cid_codigo = (int)objCidadeViewModel.objCidade.cid_codigo;
                    RaisePropertyChanged("cid_codigo");
                }
                objCidadeViewModel = null;
                objTelaCadastro = null;
            };
            objTelaCadastro.ShowDialog();
            //if (objParam != null)
            //{
            //    if (objParam.ToString() == "Novo")
            //    {
            //        winCadastro objTelaCadastro = new winCadastro();
            //        CidadeViewModel objCidadeViewModel = new CidadeViewModel();
            //        if (objEmpresa.cid_codigo > 0)
            //            objCidadeViewModel.cid_codigo = objEmpresa.cid_codigo;
            //        objCidadeViewModel.OnDispose += (sen1, eve1) => { objTelaCadastro.Close(); };
            //        objTelaCadastro.Title = "Cadastro - " + objCidadeViewModel.strNomeTela;
            //        objTelaCadastro.DataContext = objCidadeViewModel;
            //        objTelaCadastro.Owner = (Window)Application.Current.MainWindow;
            //        objTelaCadastro.Closed += (sen, eve) =>
            //        {
            //            if (objCidadeViewModel.cid_codigo != null)
            //            {
            //                CarregaComboEstado();
            //                objEmpresa.est_codigo = (int)objCidadeViewModel.est_codigo;
            //                RaisePropertyChanged("est_codigo");
            //                objEmpresa.cid_codigo = (int)objCidadeViewModel.cid_codigo;
            //                RaisePropertyChanged("cid_codigo");
            //            }
            //            objCidadeViewModel = null;
            //            objTelaCadastro = null;
            //        };
            //        objTelaCadastro.ShowDialog();
            //    }
            //}
        }

        private void NovaLogo(object objParam)
        {
            var ofdEscolha = new OpenFileDialog();
            ofdEscolha.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
            var res = ofdEscolha.ShowDialog();
            if (res.HasValue)
            {
                objEmpresa.emp_logoFormato = Path.GetExtension(ofdEscolha.FileName).ToLower();
                emp_logo = Util.ConvertBitmapSourceToByteArray(ofdEscolha.FileName);
                if (emp_logo.Length > 7000)
                {
                    int intScala = 170;
                    while(emp_logo.Length > 7000)
                    {
                        emp_logo = Util.ResizeScala(ofdEscolha.FileName, intScala);
                        intScala -= 5;
                    }
                }
            }
        }

        private void NenhumaLogo(object objParam)
        {
            emp_logo = null;
        }

        #endregion Comandos



        #region Eventos



        #endregion Eventos



        #region Métodos

        private void CarregaComboEstado()
        {
            Retorno objRetorno;
            using (var objBLL = new Cidades())
            {
                objRetorno = objBLL.RetornaListaEstado();
            }
            if (objRetorno.intCodigoErro == 0)
                arrEstado = (List<tbEstado>)objRetorno.objRetorno;
            else
                MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
        }

        #endregion Métodos
    }
}