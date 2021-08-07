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
            Log.Debug("Se ha llenado el combo de item inventario.");

            var sucursales = QuerysSucursales.GetTablesFromSucursales();
            foreach (var sucursal in sucursales) {
                ComboSucursal.Items.Add($"{sucursal.IdTienda.ToString()} {sucursal.NombreTienda}");
            }
            Log.Debug("Se ha llenado el combo de sucursales.");
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha pulsado cerrar ventana.");
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            var index = ComboNombre.SelectedIndex;
            txtMedida.Text = ListaItems[index].Medida;
            txtTipo.Text = ListaItems[index].TipoProducto;
        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(char.IsDigit);
        }

        private void CantidadGUIChanges(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
        }

        private void txtUpdatePiezas(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtPiezas.Text = textbox.Text;
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

            // Validar que la cantidad asignada <= cantidad actual.
            Validar(int.Parse(txtCantidad.Text), int.Parse(txtPiezas.Text));

            // hacerl el update.

            // Escribir en moviemientos inventario.
            var newLog = new LogInventario() {
                Mensaje = $"Asignada Cantidad: {txtCantidad.Text} Piezas: {txtPiezas.Text} de {ComboNombre.SelectedItem} a {ComboSucursal.SelectedItem}",
                UsuarioRegistra = DatabaseInit.GetUsername(),
                FechaRegistro = Convert.ToDateTime(DatabaseHelper.GetCurrentTimeNDate()),
            };

        }

        /// <summary>
        /// Método que valida que las cantidades y piezas sean validas.
        /// </summary>
        /// <returns><c>true</c> si las cantidades son validas, <c>false</c> si no.</returns>
        private bool Validar(int cantidad, int piezas) {
            var id = int.Parse(ComboNombre.SelectedItem.ToString().Split(" ")[0]);

            for (var index = 0; index < ListaItems.Count; index++) {
                var item = ListaItems[index];
                if (item.IdProducto != id) {
                    return item.Cantidad - cantidad >= 0 && item.Piezas - piezas >= 0;
                }
            }

            return false;
        }
    }
}
