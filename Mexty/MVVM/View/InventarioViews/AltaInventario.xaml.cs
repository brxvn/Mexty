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
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for AltaInventario.xaml
    /// </summary>
    public partial class AltaInventario : Window {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaProductos { get; set; }


        /// <summary>
        /// Lista de items en el inventario.
        /// </summary>
        private List<ItemInventario> ListaFromInventario { get; set; }

        public AltaInventario() {
            try {
                InitializeComponent();
                FillData();
                Log.Debug("Se han inicializado los campos de Alta Inventario Matriz.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de Alta Inventario Matriz.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que llena el combobox de producto.
        /// </summary>
        // TODO: hacer que los campos de cantidad y piezas aparezcan y desaparezcan de la ui dependiendo del tipo.
        private void FillData() {
            var data = Database.GetTablesFromProductos();
            ListaProductos = data;
            for (var index = 0; index < data.Count; index++) {
                var producto = data[index];
                ComboNombre.Items.Add(
                    $"{producto.IdProducto.ToString()} {producto.TipoProducto} {producto.NombreProducto}");
            }
            Log.Debug("Se ha llenado el combo box de producto.");

            var dataInventario = Database.GetTablesFromInventarioMatrix();
            ListaFromInventario = dataInventario;
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha pulsado en cerrar ventana.");
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            var index = ComboNombre.SelectedIndex;
            txtMedida.Text = ListaProductos[index].MedidaProducto;
            txtTipo.Text = ListaProductos[index].TipoProducto;
        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(char.IsDigit);
        }

        private void CantidadGUIChanges(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtMedida.Text = textBox.Text;
            switch (textBox.Text) {
                case "pieza":
                    txtCantidad.Visibility = Visibility.Collapsed;
                    txtPiezas.Visibility = Visibility.Visible;
                    GridCantidad.Width = new GridLength(0, GridUnitType.Star);
                    GridPiezas.Width = new GridLength(1, GridUnitType.Star);
                    break;
                case "0.5 litros":
                case "3 litros":
                case "12 litros":
                    txtCantidad.Visibility = Visibility.Collapsed;
                    txtPiezas.Visibility = Visibility.Collapsed;
                    GridCantidad.Width = new GridLength(0, GridUnitType.Star);
                    GridPiezas.Width = new GridLength(0, GridUnitType.Star);
                    break;
                case "litro":
                    txtCantidad.Visibility = Visibility.Visible;
                    txtPiezas.Visibility = Visibility.Collapsed;
                    GridCantidad.Width = new GridLength(1, GridUnitType.Star);
                    GridPiezas.Width = new GridLength(0, GridUnitType.Star);
                    break;
                default:
                    txtCantidad.Visibility = Visibility.Visible;
                    txtPiezas.Visibility = Visibility.Visible;
                    GridCantidad.Width = new GridLength(1, GridUnitType.Star);
                    GridPiezas.Width = new GridLength(1, GridUnitType.Star);
                    break;
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

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x) || '#'.Equals(x) || '/'.Equals(x));
        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtComentario.Text = textbox.Text;
        }

        /// <summary>
        /// Lógica detrás del boton de Limpiar campos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            txtCantidad.Text = "";
            txtPiezas.Text = "";
            txtComentario.Text = "";
            Log.Debug("Se han limpiado los campos.");
        }

        /// <summary>
        /// Lógica detrás del boton de guardar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarProducto(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado el boton de guardar.");

            var newProduct = new ItemInventario() {
                Comentario = txtComentario.Text,
                Cantidad = txtCantidad.Text == "" ? 0 : int.Parse(txtCantidad.Text),
                Piezas = txtPiezas.Text == "" ? 0 : int.Parse(txtPiezas.Text)
            };

            newProduct.IdProducto = int.Parse(ComboNombre.SelectedItem.ToString().Split(" ")[0]);

            if (!Validar(newProduct)) {
                Log.Warn("El objeto tipo ItemInventario no ha pasado las vaidaciones.");
                return;
            }
            Log.Debug("El objeto tipo ItemInventario ha pasado las validaciones.");


            for (var index = 0; index < ListaFromInventario.Count; index++) {
                var item = ListaFromInventario[index];
                if (item.IdProducto != newProduct.IdProducto) continue;
                MessageBox.Show(
                    "Error: Estas dando de alta un producto que ya tienes en inventario, si quieres editarlo debes ir a la pantalla de Inventario.",
                    "Producto duplicado");
                return;
            }

            try {
                var row = Database.NewItem(newProduct, true);
                if (row > 0) {
                    MessageBox.Show($"Se ha dado de alta en el inventario el producto {ComboNombre.SelectedItem}");
                    Log.Debug("Se ha dado de alta un producto en el inventario.");
                }

            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al dar de alta el producto en el inventario.");
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
    }
}
