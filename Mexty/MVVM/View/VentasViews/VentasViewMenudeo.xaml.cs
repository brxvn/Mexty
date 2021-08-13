using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for VentasViewMenudeo.xaml
    /// </summary>
    public partial class VentasViewMenudeo : UserControl {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaVenta = new();

        /// <summary>
        /// Venta actual en pantalla.
        /// </summary>
        private Venta VentaActual { get; set; }

        /// <summary>
        /// Producto seleccionado, viniendo de la pantalla de adm productos.
        /// </summary>
        private Producto SelectedProduct { get; set; }

        public VentasViewMenudeo() {
            try {
                InitializeComponent();
                FillData();
                NewVenta();
                ClearFields();

                Log.Debug("Se han inicializado los campos de ventas menudeo.");

                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += UpdateTimerTick;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de ventas menudeo.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que inicializa una nueva venta.
        /// </summary>
        private void NewVenta() {
            var venta = new Venta {
                Cambio = 0,
                Pago = 0,
                TotalVenta = 0,
                DetalleVentaList = new List<Producto>(),
                IdTienda = DatabaseInit.GetIdTienda(),
                UsuarioRegistra = DatabaseInit.GetUsername()
            };
            VentaActual = venta;
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
        /// Método que se encarga de inicializar los campos.
        /// </summary>
        private void FillData() {
            var data = QuerysProductos.GetTablesFromProductos();
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is Producto producto && producto.Activo != 0 // Solo productos activos en la tabla.
            };
            CollectionView = collectionView;
            DataProducts.ItemsSource = collectionView;
            Log.Debug("Se ha llendado el datagrid de ventas menudeo.");
        }

        /// <summary>
        /// Lógica para el campo de búsqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            TextBox tbx = sender as TextBox;
            var collection = CollectionView;
            if (tbx != null && tbx.Text != "") {
                var newText = tbx.Text;
                var customFilter = new Predicate<object>(o => FilterLogic(o, newText));

                collection.Filter = customFilter;
                DataProducts.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                var noNull = new Predicate<object>(producto =>
                {
                    if (producto == null) return false;
                    return ((Producto)producto).Activo == 1;
                });

                collection.Filter += noNull;
                DataProducts.ItemsSource = collection;
                CollectionView = collection;
            }

            SearchBox.Text = tbx.Text;
        }

        /// <summary>
        /// Lógica para el filtro del data grid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            text = text.ToLower();
            var producto = (Producto)obj;
            if (producto.NombreProducto.Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text)) {
                return producto.Activo == 1;
            }
            return false;
        }

        /// <summary>
        ///  Logica del evento item selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            // Bajar el campo de Especificación de producto y cuanto hay en inventario.
            // TODO: agregar el campo de especificicación e inventario.
        }

        /// <summary>
        /// Método que limpia los campos de texto y lo relacionado a la venta.
        /// </summary>
        private void ClearFields() {
            txtRecibido.Text = "";
            txtTotal.Text = "";
            ListaVenta.Clear();
            DataVenta.ItemsSource = null;
            VentaActual = new Venta();
            TotalVenta();
            CambioVenta();
        }

        /// <summary>
        /// Lógica del boton de Pagar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarVenta(object sender, RoutedEventArgs e) {
            Log.Info("Se ha precionado pagar en venta menudeo.");

            try {
                VentaActual.DetalleVentaList = ListaVenta;
                VentaActual.DetalleVenta = Venta.ListProductosToString(ListaVenta);

                if (txtRecibido.Text == "") {
                    MessageBox.Show("Error: necesitas asignar un valor de recibido a la venta.");
                }

                VentaActual.Pago = decimal.Parse(txtRecibido.Text);
                VentaActual.Cambio = decimal.Parse(txtCambio.Text.TrimStart('$'));

                var res = QuerysVentas.NewItem(VentaActual);
                if (res == 0) throw new Exception();
                MessageBox.Show("Se ha registrado la venta con exito.");
                ClearFields();
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al guardar la venta.");
                Log.Error($"Error: {exception.Message}");
                MessageBox.Show(
                    "Ha ocurrido un error al guardar la venta.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que mantiene actualizado el total de la venta.
        /// </summary>
        private void TotalVenta() {
            decimal total = 0;
            for (var index = 0; index < ListaVenta.Count; index++) {
                var producto = ListaVenta[index];
                total += producto.PrecioVenta;
            }

            VentaActual.TotalVenta = total;

            var totalFormated = string.Format("{0:C}", total);

            txtTotal.Text = totalFormated;
            txtRecibido.IsReadOnly = false;
        }

        /// <summary>
        /// Lógica detrás del boton de agregar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProduct(object sender, RoutedEventArgs e) {
            var producto = (Producto)((Button)e.Source).DataContext;

            if (!ListaVenta.Contains(producto)) ListaVenta.Add(producto);

            if (ListaVenta.Contains(producto)) {
                producto.CantidadDependencia += 1;
                producto.PrecioVenta = (producto.PrecioMenudeo * producto.CantidadDependencia);
            }
            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            Keyboard.Focus(txtRecibido);
            TotalVenta();
            CambioVenta();
            Log.Debug("Se ha agregado un producto a venta.");
        }

        /// <summary>
        /// Lógica detrás del boton de eliminar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelProduct(object sender, RoutedEventArgs e) {
            var producto = (Producto)((Button)e.Source).DataContext;

            if (ListaVenta.Contains(producto)) {
                producto.CantidadDependencia -= 1;
                producto.PrecioVenta = producto.PrecioMenudeo * producto.CantidadDependencia;
            }

            if (producto.CantidadDependencia == 0) ListaVenta.Remove(producto);

            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            TotalVenta();
            CambioVenta();
            Log.Debug("Se ha eliminado un producto de la venta.");
        }


        /// <summary>
        /// Validacion de solo letras y numeros para la dirección, así como el numeral.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsDigit(x) || '.'.Equals(x));
        }

        /// <summary>
        /// Método que actualiza el campo de total venta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUpdateVenta(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtTotal.Text = textBox.Text;
        }

        private void RecibidoUpdate(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtRecibido.Text = string.Format("{0:C}", textBox.Text);

            CambioVenta();
        }

        private void CambioVenta() {
            decimal total = txtTotal.Text == "" ? 0 : Convert.ToDecimal(txtTotal.Text.TrimStart('$'));
            decimal recibido = txtRecibido.Text == "" ? 0 : Convert.ToDecimal(txtRecibido.Text.Trim('$'));
            decimal cambio = Math.Max(0, recibido - total);
            string cambiFormated = string.Format("{0:C}", cambio);
            txtCambio.Text = cambiFormated; 
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e) {
            ClearFields();
        }
    }
}
