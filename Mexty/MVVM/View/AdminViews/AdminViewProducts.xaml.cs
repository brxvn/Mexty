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
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.View.AdminViews{
    /// <summary>
    /// Interaction logic for AdminViewProducts.xaml
    /// </summary>
    public partial class AdminViewProducts : UserControl {

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
            
            InitializeComponent();
            FillData();
            
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
        /// Método que llena la datadrid con los productos.
        /// </summary>
        private void FillData() {
            var data = Database.GetTablesFromProductos();
            ListaProductos = data;
            DataProductos.ItemsSource = data; // provicional
            // TODO: ver que onda con los productos activos.
            // var collectionView = new ListCollectionView(query) {
            //     Filter = (e) => e is Usuario emp && emp.Activo != 0 // Solo usuarios activos en la tabla.
            // };
            // CollectionView = collectionView;
            // DataUsuarios.ItemsSource = collectionView;
        }

        /// <summary>
        /// Método que reacciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            ClearFields();
            var producto = (Producto) DataProductos.SelectedItem;
            if (producto == null) return;
            SelectedProduct = producto;
            txtNombreProducto.Text = producto.NombreProducto;
            ComboVenta.SelectedIndex = producto.TipoVenta; // Implementar ComboBox 
            //ComboTipo.SelectedIndex = producto.TipoProducto; TODO: inconsistencia de tipos int a string.
            txtPrecioMayoreo.Text = producto.PrecioMayoreo.ToString();
            txtPrecioMenudeo.Text = producto.PrecioMenudeo.ToString();
            txtDetalle.Text = producto.DetallesProducto;
            txtMedida.Text = producto.MedidaProducto;
        }

        /// <summary>
        /// Método que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            txtNombreProducto.Text = "";
            ComboVenta.SelectedIndex = 0;
            ComboTipo.SelectedIndex = 0;
            txtPrecioMayoreo.Text = "";
            txtPrecioMenudeo.Text = "";
            txtDetalle.Text = "";
            txtMedida.Text = "";
        }

        /// <summary>
        /// Lógica para el campo de búsqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO agregarlo como funcion al evento TxtChangedEvent.
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            TextBox tbx = sender as TextBox;
            var collection = CollectionView;
            if (tbx != null && tbx.Text != "") {
                
            }
            else {
                
            }
        }

        /// <summary>
        /// Lógica para el filtro del data grid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            var producto = (Producto) obj;
            if (producto.NombreProducto.Contains(text) ||
                //producto.TipoVenta.Contains(text) || TODO: implementar Tipo de venta no numerico
                producto.TipoProducto.Contains(text)) {
                //return usuario.Activo == 1; Ver que onda con los productos activos.
            }
            return false;
        }

        /// <summary>
        /// Método para editar el producto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: agregar esta función al evento clic del boton editar.
        private void EditarProducto(object sender, RoutedEventArgs e) {
            if (StrPrep(txtNombreProducto.Text) != StrPrep(SelectedProduct.NombreProducto)) {
                SelectedProduct.NombreProducto = txtNombreProducto.Text;
            }

            // if (ComboTipo.SelectedIndex != SelectedProduct.TipoProducto) {
            //     SelectedProduct.TipoProducto = ComboTipo.SelectedIndex;
            // } TODO: Inconsitencias tipo de productos.
            if (ComboVenta.SelectedIndex != SelectedProduct.TipoVenta) {
                SelectedProduct.TipoVenta = ComboTipo.SelectedIndex;
            }
            if (txtPrecioMayoreo.Text != SelectedProduct.PrecioMayoreo.ToString()) {
                SelectedProduct.PrecioMayoreo = int.Parse(txtPrecioMayoreo.Text);
            }
            if (txtPrecioMenudeo.Text != SelectedProduct.PrecioMenudeo.ToString()) {
                SelectedProduct.PrecioMenudeo = int.Parse(txtPrecioMenudeo.Text);
            }
            if (StrPrep(txtDetalle.Text) != StrPrep(SelectedProduct.DetallesProducto)) {
                SelectedProduct.DetallesProducto = txtDetalle.Text;
            }
            if (StrPrep(txtMedida.Text) != StrPrep(SelectedProduct.MedidaProducto)) {
                SelectedProduct.MedidaProducto = txtMedida.Text;
            }
            
            var mensaje = "Está a punto de editar el producto: \n" + SelectedProduct.NombreProducto + "\n"
               + "¿Desea continuar?";
            var titulo = "Confirmación de Edicionde Usuario";
            if (MessageBox.Show(mensaje, titulo, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                Database.UpdateData(SelectedProduct);
                FillData();
                ClearFields();
            }
        }

        /// <summary>
        /// Lógica del boton de registrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: agregar esta función al evento clic del boton registar.
        private void RegistrarUsuario(object sender, RoutedEventArgs e) {
            var newProduct = new Producto();
            newProduct.NombreProducto = txtNombreProducto.Text;
            newProduct.MedidaProducto = txtMedida.Text;
            newProduct.TipoProducto = txtMedida.Text;
            newProduct.TipoVenta = ComboVenta.SelectedIndex;
            //newProduct.TipoProducto = ComboTipo.SelectedIndex; TODO inconsistencia de datos de arriba.
            newProduct.PrecioMayoreo = int.Parse(txtPrecioMayoreo.Text);
            newProduct.PrecioMenudeo = int.Parse(txtPrecioMenudeo.Text);
            newProduct.DetallesProducto = txtDetalle.Text;
            foreach (var producto in ListaProductos) {
                // TODO: Checar por repetidos
            }
            // TODO: mostrar cuadro de dialogo
            
            // if (!repetido) {
            //     Database.NewProduct(newUsuario);
            // }
            
            ClearFields();
            FillData();
        }

        //x
        /// <summary>
        /// Elimina (hace inactivo) el producto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: Preguntar si debemos agregar el campo activo a tabla de productos.
        // TODO: agregar esta función al evento clicl del boton eliminar.
        private void EliminarProducto(object sender, RoutedEventArgs e) {
            
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: agregarlo al evento PreviewTextInput de los campos númericos.
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Regresa la cadena dada en Mayusculas y sin espeacios.
        /// </summary>
        /// <param name="text">Texto a Preparar.</param>
        /// <returns></returns>
        private static string StrPrep(string text) {
            return text.ToUpper().Replace(" ", "");
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: agregarlo al evento click del boton Limpiar.
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
            //string menssage = "¡Desea limpiar los campos?";
            //string titulo = "Confirmación";

            //if (MessageBox.Show("Do you want to close this window?",
            //"Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                
            //}

        }

        private void txtIdProducto_TextChanged(object sender)
        {

        }
    }
}
