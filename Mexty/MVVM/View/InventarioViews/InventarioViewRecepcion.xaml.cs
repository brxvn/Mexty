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
using Mexty.MVVM.Model.Validations;

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

        private List<ItemInventario> ListaFromInventario { get; set; }

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

            var enInventario = Database.GetTablesFromInventario();
            ListaFromInventario = enInventario;
            Log.Debug("Se han obtenido los prodcutos de manera exitosa.");
        }

        /// <summary>
        /// Metodo que actua cuando se cambia la selección en ComboNombre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e) {
            var index = ComboNombre.SelectedIndex;
            txtMedida.Text = ListaProductos[index].MedidaProducto;
            txtTipo.Text = ListaProductos[index].TipoProducto;
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// Método que maneja la lógica del boton de guardar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarProducto(object sender, RoutedEventArgs e) {
            var newItem = new ItemInventario {
                IdProducto = ComboNombre.SelectedIndex + 1,
                Comentario = txtComentario.Text,
                Cantidad = int.Parse(txtCantidad.Text),
                Piezas = int.Parse(txtPiezas.Text)
            };

            if (!Validar(newItem)) {
                Log.Warn("El objeto tipo ItemInventario no ha pasado las vaidaciones.");
                return;
            }
            Log.Debug("El objeto tipo ItemInventario ha pasado las validaciones.");

            for (var index = 0; index < ListaFromInventario.Count; index++) {
                var item = ListaFromInventario[index];
                if (item.IdProducto != newItem.IdProducto) continue;
                MessageBox.Show(
                    "Error: Estas dando de alta un producto que ya tienes en inventario, si quieres editarlo debes ir a la pantalla de Inventario.",
                    "Producto duplicado");
                return;
            }

            try {
                var row = Database.NewItem(newItem);
                if (row > 0) {
                    MessageBox.Show($"Se ha dado de alta en el inventario el producto {ComboNombre.SelectedItem}");
                    Log.Debug("Se ha dado de alta un producto en el inventario.");
                }
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al dar de alta un producto en el inventario.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Valida un objeto tipo ItemInventario.
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        private static bool Validar(ItemInventario newItem) {
            try {
                var validator = new ItemValidation();
                var results = validator.Validate(newItem);
                if (!results.IsValid) {
                    foreach (var error in results.Errors) {
                        MessageBox.Show(error.ErrorMessage);
                        Log.Warn(error.ErrorMessage);
                    }

                    return false;
                }

                return true;

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al hacer la validación.");
                Log.Error($"Error: {e.Message}");
                return false;
            }
        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
        }

        private void txtUpdatePiezas(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtPiezas.Text = textbox.Text;

        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtComentario.Text = textbox.Text;
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
