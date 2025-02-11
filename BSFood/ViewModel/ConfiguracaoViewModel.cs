﻿using BSFood.View;
using BSFood.Apoio;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;
using BSFoodFramework.Apoio;
using BSFoodFramework.Models;
using BSFoodFramework.BusinessLogic;
using BSFoodFramework.DataTransfer;

namespace BSFood.ViewModel
{
    public class ConfiguracaoViewModel : TelaViewModel
    {
        public ICommand SalvarCommand { get; set; }
        public ICommand LogCommand { get; set; }

        public ConfiguracaoViewModel()
        {
            SalvarCommand = new DelegateCommand(Salvar, CanSalvar);
            LogCommand = new DelegateCommand(Log, CanLog);
            this.objConfiguracao = FrameworkUtil.objConfigStorage.objConfiguracao;
        }


        #region Propriedades

        [Required(ErrorMessage = "Cnpj da Software House obrigatório")]
        [StringLength(20, ErrorMessage = "É permitido apenas 20 caracteres")]
        public string cfg_cnpjSoftwareHouse
        {
            get { return this.objConfiguracao == null ? string.Empty : this.objConfiguracao.cfg_cnpjSoftwareHouse; }
            set
            {
                if (this.objConfiguracao.cfg_cnpjSoftwareHouse != value)
                {
                    this.objConfiguracao.cfg_cnpjSoftwareHouse = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Impressora para entrega obrigatória")]
        [StringLength(150, ErrorMessage = "É permitido apenas 150 caracteres")]
        public string cfg_impressoraEntrega
        {
            get { return this.objConfiguracao == null ? string.Empty : this.objConfiguracao.cfg_impressoraEntrega; }
            set
            {
                if (this.objConfiguracao.cfg_impressoraEntrega != value)
                {
                    this.objConfiguracao.cfg_impressoraEntrega = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Impressora para comanda obrigatória")]
        [StringLength(150, ErrorMessage = "É permitido apenas 150 caracteres")]
        public string cfg_impressoraComanda
        {
            get { return this.objConfiguracao == null ? string.Empty : this.objConfiguracao.cfg_impressoraComanda; }
            set
            {
                if (this.objConfiguracao.cfg_impressoraComanda != value)
                {
                    this.objConfiguracao.cfg_impressoraComanda = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Impressora para bebidas obrigatória")]
        [StringLength(150, ErrorMessage = "É permitido apenas 150 caracteres")]
        public string cfg_impressoraBebida
        {
            get { return this.objConfiguracao == null ? string.Empty : this.objConfiguracao.cfg_impressoraBebida; }
            set
            {
                if (this.objConfiguracao.cfg_impressoraBebida != value)
                {
                    this.objConfiguracao.cfg_impressoraBebida = value;
                    RaisePropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Impressora para balcão obrigatória")]
        [StringLength(150, ErrorMessage = "É permitido apenas 150 caracteres")]
        public string cfg_impressoraBalcao
        {
            get { return this.objConfiguracao == null ? string.Empty : this.objConfiguracao.cfg_impressoraBalcao; }
            set
            {
                if (this.objConfiguracao.cfg_impressoraBalcao != value)
                {
                    this.objConfiguracao.cfg_impressoraBalcao = value;
                    RaisePropertyChanged();
                }
            }
        }

        private tbConfiguracao _objConfiguracao;
        public tbConfiguracao objConfiguracao
        {
            get { return this._objConfiguracao; }
            set
            {
                this._objConfiguracao = value;
                RaisePropertyChanged(null);
            }
        }

        public List<tbAuditoria> arrAuditoria { get; set; }

        #endregion Propriedades



        #region Comandos

        private bool CanSalvar(object objParam)
        {
            return true;
        }
        private void Salvar(object objParam)
        {
            this.Validate();
            if (!this.HasErrors)
            {
                Retorno objRetorno;
                using (var objBLL = new Configuracao())
                {
                    objRetorno = objBLL.SalvarConfiguracao(this.objConfiguracao, FrameworkUtil.objConfigStorage.objFuncionario.fun_codigo);
                }
                if (objRetorno.intCodigoErro == 0)
                {
                    MessageBox.Show("A configuracao do sistema foi alterada, será necessário fechar o sistema!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Util.FecharSistema();
                }
                else
                    MessageBox.Show(objRetorno.strMsgErro, "Atenção", MessageBoxButton.OK, Util.GetMessageImage(objRetorno.intCodigoErro));
            }
        }

        private bool CanLog(object objParam)
        {
            return true;
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
                        objRetorno = objBLL.RetornaListaAuditoria(this.objConfiguracao.cfg_codigo, this.objConfiguracao);
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