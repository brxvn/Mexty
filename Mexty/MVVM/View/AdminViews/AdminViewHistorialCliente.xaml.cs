using Mexty.MVVM.Model.DataTypes;
using System;
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

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewHistorialCliente.xaml
    /// </summary>
    public partial class AdminViewHistorialCliente : Window {
        public AdminViewHistorialCliente(Cliente cliente) {
            InitializeComponent();
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }
    }
}
