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
    /// Interaction logic for InventarioViewInvent.xaml
    /// </summary>
    public partial class InventarioViewInvent : UserControl {
        public InventarioViewInvent() {
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
        private void FilterSearch(object sender, TextChangedEventArgs e) {

        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }

        private void TextUpdateNombre(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateMenudeo(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateMayoreo(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateDisponible(object sender, TextChangedEventArgs e) {

        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {

        }

        private void RegistrarProducto(object sender, RoutedEventArgs e) {

        }

        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {

        }
    }
}
