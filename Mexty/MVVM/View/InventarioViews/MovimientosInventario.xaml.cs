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
using System.Windows.Shapes;

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for MovimientosInventario.xaml
    /// </summary>
    public partial class MovimientosInventario : Window {
        public MovimientosInventario() {
            InitializeComponent();
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }

        private void ImprimirTxt(object sender, RoutedEventArgs e) {

        }
    }
}