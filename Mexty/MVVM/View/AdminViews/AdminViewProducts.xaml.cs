using System;
using System.Collections.Generic;
using System.Globalization;
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
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewProducts.xaml
    /// </summary>
    public partial class AdminViewProducts : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaProductos { get; set; }


        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// El último producto seleccionado de la datagrid.
        /// </summary>
        private Producto SelectedProduct { get; set; }

        public AdminViewProducts() {
            Log.Info("Iniciado modulo de productos.");

            try {
                InitializeComponent();
                FillData();
                ClearFields();

                Log.Debug("Se han inicializado los campos del modulo Productos exitosamente.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo producto.");
                Log.Error($"Error: {e.Message}");
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += UpdateTimerTick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            lblSucursal.Content = DatabaseInit.GetNombreTiendaIni();

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
        /// Método que llena la datadrid con los productos.
        /// </summary>
        private void FillData() {
            var data = QuerysProductos.GetTablesFromProductos();
            ListaProductos = data;
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is Producto producto
            };
            CollectionView = collectionView;
            DataProductos.ItemsSource = collectionView;
            Log.Debug("Se ha llenado la datagrid de productos.");

            //Datos ComboVenta
            ComboVenta.ItemsSource = Producto.TiposVentaTexto;
            Log.Debug("Se ha llenado el combo box de tipos de venta.");

            //Datos Combo tipos.
            ComboTipo.ItemsSource = Producto.GetTiposProducto();
            Log.Debug("Se ha llenado el combo box de tipos de producto.");

            //Datos Combo Medida.
            ComboMedida.ItemsSource = Producto.GetTiposMedida();
            Log.Debug("Se ha llenadao el comobo box de tipos de medida.");

            ComboCantidad.ItemsSource = Producto.GetTiposCantidades();
        }


        /// <summary>
        /// Método que reacciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e) {
            ClearFields(false);
            txtNombreProducto.IsReadOnly = true;
            ComboTipo.IsEnabled = false;
            if (DataProductos.SelectedItem == null) return;
            Log.Debug("Producto seleccionado.");
            var producto = (Producto)DataProductos.SelectedItem;

            SelectedProduct = producto;
            txtNombreProducto.Text = producto.NombreProducto;
            ComboVenta.SelectedIndex = producto.TipoVenta;
            ComboTipo.SelectedItem = producto.TipoProducto;

            txtPrecioMayoreo.Text = producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture);
            txtPrecioMenudeo.Text = producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture);
            txtDetalle.Text = producto.DetallesProducto;

            var strings = producto.MedidaProducto.Split(' ');
            ComboMedida.SelectedItem = strings[0];
            if (strings.Length == 2) ComboCantidad.SelectedItem = strings[1]; // si es litros y contiene medida.

            Eliminar.IsEnabled = true;
            Eliminar.ToolTip = "Eliminar Producto.";
            Guardar.IsEnabled = true;
        }

        /// <summary>
        /// Método que limpia los campos de texto
        /// </summary>
        /// <param name="botton">Si se le da false, no borra el campo de busqueda para que no se pierdan las busquedas.</param>
        private void ClearFields(bool botton=true) {
            if (botton) SearchBox.Text = "";
            Guardar.IsEnabled = false;
            Eliminar.IsEnabled = false;
            Eliminar.ToolTip = "Seleccione al menos un producto para eliminar.";
            txtNombreProducto.Text = "";
            ComboVenta.SelectedIndex = 0;
            ComboTipo.SelectedIndex = 0;
            txtPrecioMayoreo.Text = "";
            txtPrecioMenudeo.Text = "";

            txtDetalle.Text = "";
            ComboMedida.SelectedIndex = 0;
            txtNombreProducto.IsReadOnly = false;
            ComboTipo.IsEnabled = true;
            EnableGuardar();
            Log.Debug("Se han limpiado los campos del modulo de productos.");
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
                DataProductos.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                DataProductos.ItemsSource = collection;
                CollectionView = collection;
                ClearFields();
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

            if (text.Contains("00000")) {
                try {
                    int result = Int32.Parse(text);
                    if (producto.NombreProducto.ToLower().Contains(text) ||
                        producto.IdProducto.ToString().Contains(result.ToString()) ||
                        producto.TipoProducto.ToLower().Contains(text) ||
                        producto.TipoVentaNombre.ToLower().Contains(text) ||
                        producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture).Contains(text) ||
                        producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture).Contains(text)) {

                        return true;
                    }
                }
                catch (Exception e) {
                    Log.Warn(e.Message);
                }
            }
            else if (producto.NombreProducto.ToLower().Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text) ||
                producto.TipoVentaNombre.ToLower().Contains(text) ||
                producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture).Contains(text) ||
                producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture).Contains(text)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Lógica del boton de Guardar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarProducto(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado el boton de guardar.");
            try {
                var newProduct = new Producto {
                    NombreProducto = txtNombreProducto.Text,
                    MedidaProducto = ComboMedida.SelectedItem.ToString(),
                    TipoProducto = ComboTipo.SelectedItem.ToString(),
                    TipoVenta = ComboVenta.SelectedIndex
                };

                var tipo = ComboMedida.SelectedItem.ToString();
                newProduct.MedidaProducto = tipo == "litro" ? $"{tipo} {ComboCantidad.SelectedItem}" : ComboMedida.SelectedItem.ToString();

                newProduct.PrecioMayoreo = txtPrecioMayoreo.Text == "" ? 0 : decimal.Parse(txtPrecioMayoreo.Text);
                newProduct.PrecioMenudeo = txtPrecioMayoreo.Text == "" ? 0 : decimal.Parse(txtPrecioMenudeo.Text);

                // Validación de precio mayoreo y precio menudeo.
                if (newProduct.PrecioMayoreo > newProduct.PrecioMenudeo && ComboVenta.SelectedItem.ToString()?.ToLower() == "general") {
                    MessageBox.Show("No se permite guardar productos con un precio mayoreo mayor a el precio menudeo",
                        "Error: Precio mayoreo mayor que precio menudeo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }


                newProduct.DetallesProducto = txtDetalle.Text;
                Log.Debug("Se ha creado el objeto tipo Producto con los campos de texto.");

                if (!Validar(newProduct)) {
                    Log.Warn("El objeto tipo Producto no ha pasado las vaidaciones.");
                    return;
                }
                Log.Debug("El objeto tipo Producto ha pasado las validaciones.");

                if (SelectedProduct != null && SelectedProduct.NombreProducto == newProduct.NombreProducto && SelectedProduct.TipoProducto == newProduct.TipoProducto) {
                    Edit(newProduct);
                }
                else {
                    Alta(newProduct);
                }
                FillData();
                ClearFields();
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al hacer el proceso de guardar.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la alta de un producto.
        /// </summary>
        /// <param name="newProduct"></param>
        private static void Alta(Producto newProduct) {
            try {
                Log.Debug("Detectada alta de producto.");

                var res = QuerysProductos.NewProduct(newProduct);
                if (res == 0) return;

                var msg = $"Se agregó el producto {newProduct.TipoProducto} {newProduct.NombreProducto} correctamente.";
                MessageBox.Show(msg, "Producto Agregado");
                Log.Debug("Se ha dado de alta el producto de manera exitosa.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta el producto.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la edición de un producto.
        /// </summary>
        /// <param name="newProduct"></param>
        private void Edit(Producto newProduct) {
            try {
                Log.Debug("Detectada actualización de producto.");
                newProduct.IdProducto = SelectedProduct.IdProducto;
                var result = QuerysProductos.UpdateData(newProduct);

                if (result <= 0) return;
                var msg = $"Se actualizó el producto {newProduct.IdProducto.ToString()} {newProduct.TipoProducto} {newProduct.NombreProducto} correctamente.";
                MessageBox.Show(msg, "Producto Actualizado");
                Log.Debug("Se ha editado el producto de manera exitosa.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al editar el producto.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que valida a un objeto tipo Producto
        /// </summary>
        /// <param name="newProduct"> Producto a validar.</param>
        /// <returns></returns>
        private static bool Validar(Producto newProduct) {
            try {
                var validator = new ProductValidation();
                var results = validator.Validate(newProduct);
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

        /// <summary>
        /// Elimina (hace inactivo) el producto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarProducto(object sender, RoutedEventArgs e) {
            Log.Debug("Presionado eliminar producto.");
            var producto = SelectedProduct;
            var mensaje = $"¿Seguro quiere eliminar el producto {producto.TipoProducto} {producto.NombreProducto}?";
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) != MessageBoxResult.OK) return;
            QuerysProductos.DelProduct(producto.IdProducto);
            Log.Debug("Producto eliminado.");
            SelectedProduct = null;
            ClearFields();
            FillData();
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsDigit(x) || '.'.Equals(x));
        }

        private void OnlyNumbersValidationCantidad(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsDigit(x));
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            DataProductos.SelectedItem = null;
            ClearFields();
        }

        // --- Eventos Text-Update--

        private void TextUpdateNombre(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtNombreProducto.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdatePrecioMenudeo(object sender, TextChangedEventArgs a) {
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

        private void TextUpdatePrecioMayoreo(object sender, TextChangedEventArgs a) {
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

        private void TextUpdateDetalle(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtDetalle.Text = textbox.Text;
            EnableGuardar();
        }

        /// <summary>
        /// Método que habilita el botón de "GUARDAR"
        /// </summary>
        private void EnableGuardar() {
            if (txtNombreProducto.Text.Length > 3) {
                Guardar.IsEnabled = true;
                Guardar.ToolTip = "Guardar Cambios.";
            }
            else {
                Guardar.IsEnabled = false;
                Guardar.ToolTip = "Verificar los datos para guardar.\nDebe de tener al menos el nombre para dar de alta el producto.";

            }
        }

        /// <summary>
        /// Metodo para la validacion de solo Letras en el input
        /// </summary>
        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(c => char.IsLetter(c));
        }

        /// <summary>
        /// Validacion de solo letras y numeros para la dirección, así como el numeral.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x) || ','.Equals(x) || '/'.Equals(x) || '('.Equals(x) || ')'.Equals(x) || '-'.Equals(x));
        }


        /// <summary>
        /// Habilita los campos para dar de alta el precio del producto.
        /// </summary>
        private void HabilitarInputPrecios(object sender, SelectionChangedEventArgs e) {
            switch (ComboVenta.SelectedIndex) {
                case 0:
                    txtPrecioMayoreo.Text = "";
                    txtPrecioMenudeo.Text = "";
                    txtPrecioMayoreo.IsReadOnly = false;
                    txtPrecioMenudeo.IsReadOnly = false;

                    break;
                case 1:
                    txtPrecioMayoreo.IsReadOnly = false;
                    txtPrecioMayoreo.Text = "";
                    txtPrecioMenudeo.Text = "0";
                    txtPrecioMenudeo.IsReadOnly = true;
                    break;
                default:
                    txtPrecioMayoreo.IsReadOnly = true;
                    txtPrecioMenudeo.IsReadOnly = false;
                    txtPrecioMayoreo.Text = "0";
                    txtPrecioMenudeo.Text = "";
                    break;
            }

        }

        /// <summary>
        /// Mostrar solo litros para Agua y Helado
        /// </summary>
        private void MonstrarLitros(object sender, SelectionChangedEventArgs e) {
            switch (ComboTipo.SelectedItem.ToString()) {    
                case "Paleta Agua":
                case "Paleta Leche":
                case "Paleta Fruta":
                    ComboMedida.ItemsSource = null;
                    ComboMedida.ItemsSource = Producto.GetTiposMedida(cant: 1);
                    ComboMedida.IsEnabled = false;
                    ComboMedida.SelectedIndex = 0;
                    txtPrecioMayoreo.IsReadOnly = false;
                    txtPrecioMenudeo.IsReadOnly = false;
                    break;
                case "Agua":
                    ComboMedida.ItemsSource = null;
                    ComboMedida.ItemsSource = Producto.GetTiposMedida(salto: 1, cant: 1);
                    ComboMedida.SelectedIndex = 0;
                    ComboMedida.IsEnabled = true;
                    txtPrecioMayoreo.IsReadOnly = false;
                    txtPrecioMenudeo.IsReadOnly = false;
                    break;
                case "Helado":
                    ComboMedida.ItemsSource = null;
                    ComboMedida.ItemsSource = Producto.GetTiposMedida();
                    ComboMedida.SelectedIndex = 0;
                    ComboMedida.IsEnabled = true;
                    txtPrecioMayoreo.IsReadOnly = false;
                    txtPrecioMenudeo.IsReadOnly = false;
                    break;
                default:
                    ComboMedida.ItemsSource = null;
                    ComboMedida.ItemsSource = Producto.GetTiposMedida();
                    ComboMedida.SelectedIndex = 0;
                    txtPrecioMayoreo.IsReadOnly = false;
                    txtPrecioMenudeo.IsReadOnly = false;
                    ComboMedida.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Método para filtar las medidas dependiendo del tipo de producto seleccionado.
        /// </summary>
        private void DependenciaProductos(object sender, RoutedEventArgs e) {
            if (SelectedProduct == null) {
                MessageBox.Show("Seleccione un producto de la tabla primero.");
                return;
            }
            AdminViewProductDependency dependency = new(SelectedProduct);
            dependency.ShowDialog();
        }

        private void generarQR_Click(object sender, RoutedEventArgs e) {
            BarCodes barCodes = new();
        }

        private void MostrarCantidades(object sender, SelectionChangedEventArgs e) {

            if (ComboMedida.SelectedIndex == 1 && ComboTipo.SelectedIndex == 3) {
                ComboCantidad.Visibility = Visibility.Visible;
                ColCantidad.Width = new GridLength(.2, GridUnitType.Star);
            }
            else if (ComboMedida.SelectedIndex == 0 && ComboTipo.SelectedIndex == 4) {
                ComboCantidad.Visibility = Visibility.Visible;
                ColCantidad.Width = new GridLength(.2, GridUnitType.Star);
            }
            else {
                ComboCantidad.Visibility = Visibility.Collapsed;
                ColCantidad.Width = new GridLength(0, GridUnitType.Star);
            }
        }
    }
}
