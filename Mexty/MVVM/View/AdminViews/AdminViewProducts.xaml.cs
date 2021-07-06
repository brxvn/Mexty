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
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;

namespace Mexty.MVVM.View.AdminViews{
    /// <summary>
    /// Interaction logic for AdminViewProducts.xaml
    /// </summary>
    public partial class AdminViewProducts : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaProductos { get; set; }

        /// <summary>
        /// Lista de todas las sucursales dada por la Base de datos
        /// </summary>
        private List<Sucursal> ListaSucursales { get; set; }

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
                FillSucursales();
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
            var data = Database.GetTablesFromProductos();
            ListaProductos = data;
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is Producto producto && producto.Activo != 0 // Solo productos activos en la tabla.
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
        }

        /// <summary>
        /// Método que llena la lista de sucursales.
        /// </summary>
        private void FillSucursales() {
            var sucursales = Database.GetTablesFromSucursales();
            ListaSucursales = sucursales;
            foreach (var sucursal in sucursales) {
                ComboSucursal.Items.Add(sucursal.NombreTienda);
            }
            Log.Debug("Se ha llenado el combo box de sucursales.");
        }

        /// <summary>
        /// Método que reacciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            ClearFields();
            txtNombreProducto.IsReadOnly = true;
            ComboTipo.IsEnabled = false;
            
            if (DataProductos.SelectedItem == null) return;
            Log.Debug("Producto seleccionado.");
            var producto = (Producto) DataProductos.SelectedItem;
            
            SelectedProduct = producto;
            txtNombreProducto.Text = producto.NombreProducto;
            ComboVenta.SelectedIndex = producto.TipoVenta;
            ComboTipo.SelectedItem = producto.TipoProducto;
            txtPrecioMayoreo.Text = producto.PrecioMayoreo.ToString();
            txtPrecioMenudeo.Text = producto.PrecioMenudeo.ToString();
            txtDetalle.Text = producto.DetallesProducto;
            ComboMedida.SelectedItem = producto.MedidaProducto;
            ComboSucursal.SelectedIndex = producto.IdSucursal - 1;
            Eliminar.IsEnabled = true;
            Guardar.IsEnabled = true;
        }

        /// <summary>
        /// Método que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            Guardar.IsEnabled = false;
            Eliminar.IsEnabled = false;
            txtNombreProducto.Text = "";
            ComboVenta.SelectedIndex = 0;
            ComboTipo.SelectedIndex = 0;
            txtPrecioMayoreo.Text = "";
            txtPrecioMenudeo.Text = "";
            txtDetalle.Text = "";
            ComboMedida.SelectedIndex = 0;
            ComboSucursal.SelectedIndex = 0;
            txtNombreProducto.IsReadOnly = false;
            ComboTipo.IsEnabled = true;
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
                var noNull = new Predicate<object>(producto => {
                    if (producto == null) return false;
                    return ((Producto) producto).Activo == 1;
                });
                collection.Filter += noNull;
                DataProductos.ItemsSource = collection;
                CollectionView = collection;
            }
        }

        /// <summary>
        /// Lógica para el filtro del data grid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            text = text.ToLower();
            var producto = (Producto) obj;
            if (producto.NombreProducto.Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text) ||
                producto.TipoVentaNombre.ToLower().Contains(text) ||
                producto.GetSucursalNombre.ToLower().Contains(text)) {
                return producto.Activo == 1;
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
                var newProduct = new Producto();
                newProduct.NombreProducto = txtNombreProducto.Text;
                newProduct.MedidaProducto = ComboMedida.SelectedItem.ToString();
                newProduct.TipoProducto = ComboTipo.SelectedItem.ToString();
                newProduct.TipoVenta = ComboVenta.SelectedIndex;
                newProduct.TipoProducto = ComboTipo.SelectedItem.ToString();
                newProduct.PrecioMayoreo = txtPrecioMayoreo.Text == "" ? 0 : int.Parse(txtPrecioMayoreo.Text);
                newProduct.PrecioMenudeo = txtPrecioMayoreo.Text == "" ? 0 : int.Parse(txtPrecioMenudeo.Text);
                newProduct.DetallesProducto = txtDetalle.Text;
                newProduct.IdSucursal = ComboSucursal.SelectedIndex + 1;
                Log.Debug("Se ha creado el objeto tipo Producto con los campos de texto.");

                if (!Validar(newProduct)) {
                    Log.Warn("El objeto tipo Producto no ha pasado las vaidaciones.");
                    return;
                }
                Log.Debug("El objeto tipo Producto ha pasado las validaciones.");

                if (SelectedProduct != null && SelectedProduct.NombreProducto == newProduct.NombreProducto) {
                    Edit(newProduct);
                }
                else {
                    var alta = true;
                    if (ListaProductos != null) {
                        Activar(newProduct, ref alta);
                    }
                    if (alta) {
                        Alta(newProduct);
                    }
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
            Log.Debug("Detectada alta de producto.");
            Database.NewProduct(newProduct);
            var msg = $"Se ha dado de alta el producto {newProduct.NombreProducto}.";
            MessageBox.Show(msg, "Producto Actualizado");
        }

        /// <summary>
        /// Método que se encarga de la edición de un producto.
        /// </summary>
        /// <param name="newProduct"></param>
        private void Edit(Producto newProduct) {
            Log.Debug("Detectada actualización de producto.");
            newProduct.IdProducto = SelectedProduct.IdProducto;
            newProduct.Activo = SelectedProduct.Activo;
            Database.UpdateData(newProduct);

            var msg = $"Se ha actualizado el producto {newProduct.IdProducto.ToString()} {newProduct.NombreProducto}.";
            MessageBox.Show(msg, "Producto Actualizado");
        }

        /// <summary>
        /// Métdodo que se encarga de la activación de un usuario.
        /// </summary>
        /// <param name="newProduct"></param>
        /// <param name="alta"></param>
        private void Activar(Producto newProduct, ref bool alta) {
            for (var index = 0; index < ListaProductos.Count; index++) {
                var producto = ListaProductos[index];
                if (newProduct.NombreProducto != producto.NombreProducto || producto.Activo != 0) continue;
                Log.Debug("Detectado producto equivalente no activo, actualizando y activando.");
                // actualizamos y activamos.
                newProduct.IdProducto = producto.IdProducto;
                newProduct.Activo = 1;
                Database.UpdateData(newProduct);
                alta = false;
                var msg =
                    $"Se ha activado y actualizado el producto {newProduct.IdProducto.ToString()} {newProduct.NombreProducto}.";
                MessageBox.Show(msg, "Producto Actualizado");
            }
        }

        /// <summary>
        /// Método que valida a un objeto tipo Producto
        /// </summary>
        /// <param name="newProduct"> Producto a validar.</param>
        /// <returns></returns>
        private static bool Validar(Producto newProduct) {
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

        /// <summary>
        /// Elimina (hace inactivo) el producto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarProducto(object sender, RoutedEventArgs e) {
            Log.Debug("Presionado eliminar producto.");
            var producto = SelectedProduct;
            var mensaje = $"¿Seguro quiere eliminar el producto {producto.NombreProducto}?";
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) != MessageBoxResult.OK) return;
            producto.Activo = 0;
            Database.UpdateData(producto);
            Log.Debug("Producto eliminado.");
            ClearFields();
            FillData();
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => Char.IsDigit(x) || '.'.Equals(x));
         }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
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
            EnableGuardar();
        }    

        private void TextUpdatePrecioMayoreo(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtPrecioMayoreo.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdateDetalle(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtDetalle.Text = textbox.Text;
            EnableGuardar();
        }

        private void EnableGuardar() {
            if (txtNombreProducto.Text != "" && txtPrecioMenudeo.Text != "" && txtPrecioMayoreo.Text != "" && txtDetalle.Text != "") {
                Guardar.IsEnabled = true;
            }
            else Guardar.IsEnabled = false;
        }

        /// <summary>
        /// Metodo para la validacion de solo Letras en el input
        /// </summary>
        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetter(x));
            // if (!Regex.IsMatch(e.Text, "^[a-zñáéíóúüA-ZÑÁÉÍÓÚÜ]")) {
            //     e.Handled = true;
            // }
        }

    }
}
