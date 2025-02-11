﻿using BSFood.Apoio;
using BSFoodFramework.Apoio;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BSFood.ViewModel
{
    public class TelaViewModel : ViewModelBase
    {
        public ICommand FecharCommand { get; set; }

        public TelaViewModel()
        {
            //Recupero definições da tela, como nome, titulo, imagem que a representa e permissões
            if (FrameworkUtil.objConfigStorage != null && FrameworkUtil.objConfigStorage.objPerfilAcesso != null)
            {
                strNomeControle = GetType().Namespace + "." + GetType().Name;
                var objPerfilAcessoMenuAux = FrameworkUtil.objConfigStorage.objPerfilAcesso.tbPerfilAcessoMenu
                    .FirstOrDefault(pam => pam.tbMenu.men_nomeControle == strNomeControle);
                if (objPerfilAcessoMenuAux != null)
                {
                    strImagemTela = objPerfilAcessoMenuAux.tbMenu.men_imagem;
                    strNomeTela = objPerfilAcessoMenuAux.tbMenu.men_cabecalho;
                    blnPermiteInclusaoRegistro = objPerfilAcessoMenuAux.pam_permiteInclusao;
                    blnPermiteAlteracaoRegistro = objPerfilAcessoMenuAux.pam_permiteAlteracao;
                    blnPermiteExclusaoRegistro = objPerfilAcessoMenuAux.pam_permiteExclusao;
                }
            }
            FecharCommand = new DelegateCommand(Fechar);
            _enStatusTelaAtual = enStatusTela.Navegacao;
            blnJanela = false;

            _intQtdeRegPagina = 20;
            _intTotalPagina = 1;
            _intPaginaAtual = 1;
            _intQtdeReg = 0;
        }



        #region Propriedades

        public string strNomeControle { get; set; }

        public string strImagemTela { get; set; }

        public string Header { get; set; }

        public string strNomeTela
        {
            get { return Header; }
            set
            {
                Header = value;
            }
        }

        private enStatusTela _enStatusTelaAtual;
        public enStatusTela enStatusTelaAtual
        {
            get { return _enStatusTelaAtual; }
            set
            {
                _enStatusTelaAtual = value;
                if (_enStatusTelaAtual == enStatusTela.EmInclusaoOuAlteracao)
                    intSelectedIndexTabPrincipal = 1;
                if (_enStatusTelaAtual == enStatusTela.Navegacao)
                    intSelectedIndexTabPrincipal = 0;
            }
        }

        public bool blnPermiteInclusaoRegistro { get; set; }

        public bool blnPermiteAlteracaoRegistro { get; set; }

        public bool blnPermiteExclusaoRegistro { get; set; }

        private int _intSelectedIndexGrid;
        public int intSelectedIndexGrid
        {
            get { return _intSelectedIndexGrid; }
            set
            {
                _intSelectedIndexGrid = value;
                RaisePropertyChanged();
            }
        }

        private int _intSelectedIndexTabPrincipal;
        public int intSelectedIndexTabPrincipal
        {
            get { return _intSelectedIndexTabPrincipal; }
            set
            {
                _intSelectedIndexTabPrincipal = value;
                RaisePropertyChanged();
            }
        }

        public bool blnJanela { get; set; }

        private int _intQtdeReg;
        public int intQtdeReg
        {
            get { return _intQtdeReg; }
            set
            {
                _intQtdeReg = value;
                RaisePropertyChanged();
            }
        }

        private int _intQtdeRegPagina;
        public int intQtdeRegPagina
        {
            get { return _intQtdeRegPagina; }
            set
            {
                _intQtdeRegPagina = value;
                RaisePropertyChanged();
            }
        }

        private int _intTotalPagina;
        public int intTotalPagina
        {
            get { return _intTotalPagina; }
            set
            {
                _intTotalPagina = value;
                RaisePropertyChanged();
            }
        }

        private int _intPaginaAtual;
        public int intPaginaAtual
        {
            get { return _intPaginaAtual; }
            set
            {
                _intPaginaAtual = value;
                RaisePropertyChanged();
            }
        }

        #endregion Propriedades



        #region Comandos

        private void Fechar(object objParam)
        {
            if (enStatusTelaAtual == enStatusTela.EmInclusaoOuAlteracao)
            {
                if (MessageBox.Show("Todas as alterações serão perdidas, deseja fechar a tela?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    FecharCommand = null;
                    Dispose();
                }
            }
            else
            {
                FecharCommand = null;
                Dispose();
            }
        }

        #endregion Comandos
    }
}
