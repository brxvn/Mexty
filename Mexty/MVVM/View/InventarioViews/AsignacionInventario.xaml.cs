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
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for AsignacionInventario.xaml
    /// </summary>
    public partial class AsignacionInventario : Window {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista de productos en inventario dada por la base de datos.
        /// </summary>
        private List<ItemInventario> ListaItems { get; set; }

        public AsignacionInventario() {
            try {
                InitializeComponent();
                FillData();
                Log.Debug("Se han inicializado los campos de Asignación de inventario.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de Asignación de inventario.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que llena los combobox.
        /// </summary>
        private void FillData() {
            var data = QuerysInventario.GetTablesFromInventarioMatrix();
            ListaItems = data;
            foreach (var item in data) {
                ComboNombre.Items.Add($"{item.IdProducto.ToString()} {item.TipoProducto} {item.NombreProducto}");
            }

            ComboNombre.SelectedIndex = 0;
            Log.Debug("Se ha llenado el combo de item inventario.");


            var sucursales = QuerysSucursales.GetTablesFromSucursales();
            var sucActual = DatabaseInit.GetIdTiendaIni();
            foreach (var sucursal in sucursales) {
                if (sucActual == sucursal.IdTienda) continue; // excluye la sucursal actual.
                ComboSucursal.Items.Add($"{sucursal.IdTienda} {sucursal.NombreTienda}");
            }

            ComboSucursal.SelectedIndex = 0;
            Log.Debug("Se ha llenado el combo de sucursales.");
        }


        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha pulsado cerrar ventana.");
            Close();
        }

        private void ProductoSelected(object sender, SelectionChangedEventArgs e) {
            var index = ComboNombre.SelectedIndex;
            var cantidad = ListaItems[index].Cantidad.ToString();

            txtMedida.Text = ListaItems[index].Medida;
            txtTipo.Text = ListaItems[index].TipoProducto;
                    
            txtCantidadActual.Text = cantidad;
            txtCantidadPosterior.Text = cantidad;

            txtCantidad.Text = "";
            Guardar.IsEnabled = true;
        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(char.IsDigit);
        }

        private void CantidadGUIChanges(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtMedida.Text = textBox.Text;
            switch (textBox.Text) {
                case "0.5 litros":
                case "3 litros":
                case "12 litros":
                    txtCantidad.Visibility = Visibility.Visible;

                    txtCantidadActual.Visibility = Visibility.Visible;
                    txtCantidadPosterior.Visibility = Visibility.Visible;
                    lblCActual.Visibility = Visibility.Visible;
                    lblCRestante.Visibility = Visibility.Visible;

                    GridCantidad.Width = new GridLength(0, GridUnitType.Star);
                    rowCantidad.Height = new GridLength(0, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(0, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(0, GridUnitType.Star);
                    break;
                case "litro":
                    txtCantidad.Visibility = Visibility.Visible;

                    txtCantidadActual.Visibility = Visibility.Visible;
                    txtCantidadPosterior.Visibility = Visibility.Visible;
                    lblCActual.Visibility = Visibility.Visible;
                    lblCRestante.Visibility = Visibility.Visible;

                    GridCantidad.Width = new GridLength(1, GridUnitType.Star);
                    rowCantidad.Height = new GridLength(1, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(0, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(0, GridUnitType.Star);
                    break;
                default:
                    txtCantidad.Visibility = Visibility.Visible;

                    txtCantidadActual.Visibility = Visibility.Visible;
                    txtCantidadPosterior.Visibility = Visibility.Visible;
                    lblCActual.Visibility = Visibility.Visible;
                    lblCRestante.Visibility = Visibility.Visible;

                    GridCantidad.Width = new GridLength(1, GridUnitType.Star);
                    rowCantidad.Height = new GridLength(0, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(1, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(0, GridUnitType.Star);
                    break;
            }
        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
            var index = ComboNombre.SelectedIndex;
            var cantidadActual = ListaItems[index].Cantidad;
            var cantidadRestar = txtCantidad.Text == "" ? 0 : int.Parse(txtCantidad.Text);
            var resultado = cantidadActual - cantidadRestar;
            txtCantidadPosterior.Text = Math.Max(0, resultado).ToString();
        }

        /// <summary>
        /// Función que actualiza la cantidad restante mostrada.
        /// </summary>
        private void UpdateCantidad() {
            var index = ComboNombre.SelectedIndex;
            txtCantidadActual.Text = ListaItems[index].Cantidad.ToString();
            txtCantidadPosterior.Text = txtCantidadActual.Text;
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            txtCantidad.Text = "";
        }

        /// <summary>
        /// Lógica detrás del boton de guardar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarProducto(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado guardar.");

            try {
                // Validar que la cantidad asignada <= cantidad actual.
                var cantiadad = txtCantidad.Text == "" ? 0 : int.Parse(txtCantidad.Text);

                if (!Validar(cantiadad, out var cantiadadR)) {
                    Log.Debug("Se ha dado una cantidad no válida.");
                    MessageBox.Show("Cantidad no valida."); // provicional.
                    return;
                }

                for (var index = 0; index < ListaItems.Count; index++) {
                    var producto = ListaItems[index];

                    if (producto.IdProducto != int.Parse(ComboNombre.SelectedItem.ToString().Split(" ")[0])) continue;
                    producto.Cantidad = cantiadadR;

                    var res = QuerysInventario.UpdateData(producto, true);
                    if (res == 0) throw new Exception();
                    Log.Debug("Se ha actualizado la cantidad/piezas en el inventario.");
                }

                // Escribir en moviemientos inventario.
                var newLog = new LogInventario() {
                    Mensaje = $"Asignada Cantidad: {cantiadad.ToString()} de {ComboNombre.SelectedItem} a {ComboSucursal.SelectedItem}",
                    UsuarioRegistra = DatabaseInit.GetUsername(),
                    FechaRegistro = Convert.ToDateTime(DatabaseHelper.GetCurrentTimeNDate()),
                };

                var res1 = QuerysMovInvent.NewLogInventario(newLog);
                if (res1 == 0) throw new Exception();

                Log.Debug("Se ha actualizado el movimiento en movimientos inventario.");
                MessageBox.Show("Se ha asignado producto de manera exitosa.");
                FillData();
                txtCantidad.Text = "";
                UpdateCantidad();

            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al hacer la asignación de producto.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Método que valida que las cantidades y piezas sean validas.
        /// </summary>
        /// <returns><c>true</c> si las cantidades son validas, <c>false</c> si no.</returns>
        private bool Validar(int cantidad, out int cantidatR) {
            var id = int.Parse(ComboNombre.SelectedItem.ToString().Split(" ")[0]);
            cantidatR = -1;

            for (var index = 0; index < ListaItems.Count; index++) {
                var item = ListaItems[index];
                if (item.IdProducto == id) {
                    cantidatR = item.Cantidad - cantidad;
                    return cantidatR >= 0;
                }
            }

            return false;
        }

        private void SucursalSelected(object sender, SelectionChangedEventArgs e) {
            Guardar.IsEnabled = true; 
        }
    }
}
