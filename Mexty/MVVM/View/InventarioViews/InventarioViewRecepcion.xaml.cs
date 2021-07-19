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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for InventarioViewRecepcion.xaml
    /// </summary>
    public partial class InventarioViewRecepcion : UserControl {
        public InventarioViewRecepcion() {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        /// <summary>
        /// Actualiza la hora.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {

        }

        private void RegistrarProducto(object sender, RoutedEventArgs e) {

        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {
        }

        private void txtUpdatePiezas(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {

        }

        private void FilterSearch(object sender, TextChangedEventArgs e) {

        }

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}
