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
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for InventarioViewRecepcion.xaml
    /// </summary>
    public partial class InventarioViewRecepcion : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);


        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaProductos { get; set; }

        public InventarioViewRecepcion() {

            try {
                InitializeComponent();
                FillData();

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de Recepción de inventario.");
                Log.Error($"Error: {e.Message}");
            }


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

        /// <summary>
        /// Método que llena los campos con la información necesaria.
        /// </summary>
        private void FillData() {
            var productosDisponibles = Database.GetTablesFromProductos();
            ListaProductos = productosDisponibles;
            var listaProductos = new List<string>();
            for (var index = 0; index < productosDisponibles.Count; index++) {
                var producto = productosDisponibles[index];
                listaProductos.Add(
                    $"{producto.IdProducto.ToString()} {producto.TipoProducto} {producto.NombreProducto}");
            }

            ComboNombre.ItemsSource = listaProductos;
            Log.Debug("Se ha llenado el combo box con los productos de manera exitosa.");
        }

        /// <summary>
        /// Metodo que actua cuando se cambia la selección en ComboNombre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e) {

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
