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

namespace Mexty.MVVM.View.VentasViews {
    /// <summary>
    /// Interaction logic for VentasViewMayoreo.xaml
    /// </summary>
    public partial class VentasViewMayoreo : UserControl {
        public VentasViewMayoreo() {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += UpdateTimerTick;
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

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void FilterSearch(object sender, TextChangedEventArgs e) {

        }

        private void AddProduct(object sender, RoutedEventArgs e) {

        }

        private void DelProduct(object sender, RoutedEventArgs e) {

        }

        private void GuardarVenta(object sender, RoutedEventArgs e) {

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e) {

        }

        private void RecibidoUpdate(object sender, TextChangedEventArgs e) {

        }
    }
}
