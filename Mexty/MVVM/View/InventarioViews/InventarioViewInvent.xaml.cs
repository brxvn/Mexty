using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            TextBox textbox = sender as TextBox;
            txtNombreProducto.Text = textbox.Text;
        }

        private void txtUpdateMenudeo(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtPrecioMenudeo.Text = textbox.Text;
            Regex r = new Regex(@"^-{0,1}\d+\.{0,1}\d*$"); // This is the main part, can be altered to match any desired form or limitations
            Match m = r.Match(txtPrecioMenudeo.Text);
            if (m.Success) {
                txtPrecioMenudeo.Text = textbox.Text;
            }
            else {
                txtPrecioMenudeo.Text = "";
            }
        }

        private void txtUpdateMayoreo(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtPrecioMayoreo.Text = textbox.Text;
            Regex r = new Regex(@"^-{0,1}\d+\.{0,1}\d*$"); // This is the main part, can be altered to match any desired form or limitations
            Match m = r.Match(txtPrecioMayoreo.Text);
            if (m.Success) {
                txtPrecioMayoreo.Text = textbox.Text;
            }
            else {
                txtPrecioMayoreo.Text = "";
            }
        }

        private void txtUpdateDisponible(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {

        }

        private void RegistrarProducto(object sender, RoutedEventArgs e) {

        }

        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(c => char.IsLetter(c));
        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => Char.IsDigit(x) || '.'.Equals(x));
        }

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }
    }
}
