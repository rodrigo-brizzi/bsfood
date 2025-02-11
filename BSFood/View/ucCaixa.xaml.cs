﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BSFood.View
{
    /// <summary>
    /// Interaction logic for ucCaixa.xaml
    /// </summary>
    public partial class ucCaixa : UserControl
    {
        public ucCaixa()
        {
            InitializeComponent();
        }

        private void dgPesquisa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = e.Source as DataGrid;
            if (dg.SelectedIndex > -1)
            {
                dg.Focus();
                dg.CurrentCell = new DataGridCellInfo(dg.Items[dg.SelectedIndex], dg.Columns[0]);
            }
        }
    }
}
