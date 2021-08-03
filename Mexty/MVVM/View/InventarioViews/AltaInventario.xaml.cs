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

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for AltaInventario.xaml
    /// </summary>
    public partial class AltaInventario : Window {
        public AltaInventario() {
            InitializeComponent();
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
           Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void CantidadGUIChanges(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdatePiezas(object sender, TextChangedEventArgs e) {

        }

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {

        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {

        }

        private void RegistrarProducto(object sender, RoutedEventArgs e) {

        }
    }
}
