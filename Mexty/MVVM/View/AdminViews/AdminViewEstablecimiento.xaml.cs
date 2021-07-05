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

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewEstablecimiento.xaml
    /// </summary>
    public partial class AdminViewEstablecimiento : UserControl {
        public AdminViewEstablecimiento() {
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

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }

        private void FilterSearch(object sender, TextChangedEventArgs e) {

        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {

        }

        private void GuardarEstablecimiento(object sender, RoutedEventArgs e) {

        }

        private void EliminarEstablecimiento(object sender, RoutedEventArgs e) {

        }

        private void txtUpdateNombre(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateTelefono(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateInstagram(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateFacebook(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateDescripcion(object sender, TextChangedEventArgs e) {

        }

    }
}
