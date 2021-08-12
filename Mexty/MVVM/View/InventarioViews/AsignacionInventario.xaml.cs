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
            foreach (var sucursal in sucursales) {
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
            var piezas = ListaItems[index].Piezas.ToString();

            txtMedida.Text = ListaItems[index].Medida;
            txtTipo.Text = ListaItems[index].TipoProducto;
                    
            txtCantidadActual.Text = cantidad;
            txtCantidadPosterior.Text = cantidad;
            txtPiezasActuales.Text = piezas;
            txtPiezasPosterior.Text = piezas;

            txtPiezas.Text = "";
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
                case "pieza":
                    txtCantidad.Visibility = Visibility.Collapsed;
                    txtPiezas.Visibility = Visibility.Visible;

                    txtCantidadActual.Visibility = Visibility.Collapsed;
                    txtCantidadPosterior.Visibility = Visibility.Collapsed;
                    lblCActual.Visibility = Visibility.Collapsed;
                    lblCRestante.Visibility = Visibility.Collapsed;

                    txtPiezasActuales.Visibility = Visibility.Visible;
                    txtPiezasPosterior.Visibility = Visibility.Visible;
                    lblPActual.Visibility = Visibility.Visible;
                    lblPRestante.Visibility = Visibility.Visible;

                    rowCantidad.Height = new GridLength(0, GridUnitType.Star);
                    GridCantidad.Width = new GridLength(0, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(1, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(1, GridUnitType.Star);

                    break;
                case "0.5 litros":
                case "3 litros":
                case "12 litros":
                    txtCantidad.Visibility = Visibility.Visible;
                    txtPiezas.Visibility = Visibility.Collapsed;

                    txtCantidadActual.Visibility = Visibility.Visible;
                    txtCantidadPosterior.Visibility = Visibility.Visible;
                    lblCActual.Visibility = Visibility.Visible;
                    lblCRestante.Visibility = Visibility.Visible;

                    txtPiezasActuales.Visibility = Visibility.Collapsed;
                    txtPiezasPosterior.Visibility = Visibility.Collapsed;
                    lblPActual.Visibility = Visibility.Collapsed;
                    lblPRestante.Visibility = Visibility.Collapsed;

                    GridCantidad.Width = new GridLength(0, GridUnitType.Star);
                    rowCantidad.Height = new GridLength(0, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(0, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(0, GridUnitType.Star);
                    break;
                case "litro":
                    txtCantidad.Visibility = Visibility.Visible;
                    txtPiezas.Visibility = Visibility.Collapsed;

                    txtCantidadActual.Visibility = Visibility.Visible;
                    txtCantidadPosterior.Visibility = Visibility.Visible;
                    lblCActual.Visibility = Visibility.Visible;
                    lblCRestante.Visibility = Visibility.Visible;

                    txtPiezasActuales.Visibility = Visibility.Collapsed;
                    txtPiezasPosterior.Visibility = Visibility.Collapsed;
                    lblPActual.Visibility = Visibility.Collapsed;
                    lblPRestante.Visibility = Visibility.Collapsed;

                    GridCantidad.Width = new GridLength(1, GridUnitType.Star);
                    rowCantidad.Height = new GridLength(1, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(0, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(0, GridUnitType.Star);
                    break;
                default:
                    txtCantidad.Visibility = Visibility.Visible;
                    txtPiezas.Visibility = Visibility.Visible;

                    txtCantidadActual.Visibility = Visibility.Visible;
                    txtCantidadPosterior.Visibility = Visibility.Visible;
                    lblCActual.Visibility = Visibility.Visible;
                    lblCRestante.Visibility = Visibility.Visible;

                    txtPiezasActuales.Visibility = Visibility.Visible;
                    txtPiezasPosterior.Visibility = Visibility.Visible;
                    lblPActual.Visibility = Visibility.Visible;
                    lblPRestante.Visibility = Visibility.Visible;

                    GridCantidad.Width = new GridLength(1, GridUnitType.Star);
                    rowCantidad.Height = new GridLength(0, GridUnitType.Star);

                    GridPiezas.Width = new GridLength(1, GridUnitType.Star);
                    rowPiezas.Height = new GridLength(0, GridUnitType.Star);
                    break;
            }
        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {
            var index = ComboNombre.SelectedIndex;
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
            int cantidadActual = ListaItems[index].Cantidad;
            int cantidadRestar = txtCantidad.Text == "" ? 0 : int.Parse(txtCantidad.Text);
            int resultado = cantidadActual - cantidadRestar;
            txtCantidadPosterior.Text = Math.Max(0, resultado).ToString();
        }

        private void txtUpdatePiezas(object sender, TextChangedEventArgs e) {
            var index = ComboNombre.SelectedIndex;
            TextBox textbox = sender as TextBox;
            txtPiezas.Text = textbox.Text;
            int piezasActual = ListaItems[index].Piezas;
            int piezasRestantes = txtPiezas.Text == "" ? 0 : int.Parse(txtPiezas.Text);
            int resultado = piezasActual - piezasRestantes;
            txtPiezasPosterior.Text = Math.Max(0, resultado).ToString();
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            txtCantidad.Text = "";
            txtPiezas.Text = "";
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
                var piezas = txtPiezas.Text == "" ? 0 : int.Parse(txtPiezas.Text);

                if (!Validar(cantiadad, piezas, out var cantiadadR, out var piezasR)) {
                    Log.Debug("Se ha dado una cantidad no válida.");
                    MessageBox.Show("Cantidad no valida."); // provicional.
                    return;
                }

                for (var index = 0; index < ListaItems.Count; index++) {
                    var producto = ListaItems[index];

                    if (producto.IdProducto != int.Parse(ComboNombre.SelectedItem.ToString().Split(" ")[0])) continue;
                    producto.Cantidad = cantiadadR;
                    producto.Piezas = piezasR;

                    var res = QuerysInventario.UpdateData(producto, true);
                    if (res == 0) throw new Exception();
                    Log.Debug("Se ha actualizado la cantidad/piezas en el inventario.");
                }

                // Escribir en moviemientos inventario.
                var newLog = new LogInventario() {
                    Mensaje = $"Asignada Cantidad: {cantiadad.ToString()} Piezas: {piezas.ToString()} de {ComboNombre.SelectedItem} a {ComboSucursal.SelectedItem}",
                    UsuarioRegistra = DatabaseInit.GetUsername(),
                    FechaRegistro = Convert.ToDateTime(DatabaseHelper.GetCurrentTimeNDate()),
                };

                var res1 = QuerysMovInvent.NewLogInventario(newLog);
                if (res1 == 0) throw new Exception();
                Log.Debug("Se ha actualizado el movimiento en movimientos inventario.");
                MessageBox.Show("Se ha asignado producto de manera exitosa.");

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
        private bool Validar(int cantidad, int piezas, out int cantidatR, out int piezasR) {
            var id = int.Parse(ComboNombre.SelectedItem.ToString().Split(" ")[0]);
            cantidatR = -1;
            piezasR = -1;

            for (var index = 0; index < ListaItems.Count; index++) {
                var item = ListaItems[index];
                if (item.IdProducto == id) {
                    cantidatR = item.Cantidad - cantidad;
                    piezasR = item.Piezas - piezas;
                    return cantidatR >= 0 && piezasR >= 0;
                }
            }

            return false;
        }

        private void SucursalSelected(object sender, SelectionChangedEventArgs e) {
            Guardar.IsEnabled = true; 
        }
    }
}
