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
        /// Método que llena la datadrid con los usuarios.
        /// </summary>
        private void FillData() {
            var data = Database.GetTablesFromProductos();
            ListaProductos = data;
            DataProductos.ItemsSource = data; // provicional
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

        }

        /// <summary>
        /// Método que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            
        }

        /// <summary>
        /// Lógica para el campo de búsqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO agregarlo como funcion al evento TxtChangedEvent.
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            
        }

        /// <summary>
        /// Lógica para el filtro del data grid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            var usuario = (Usuario) obj;
            // if (usuario.Username.Contains(text) ||
            //     usuario.ApPaterno.Contains(text) ||
            //     usuario.Nombre.Contains(text)) {
            //     return usuario.Activo == 1;
            // }
            return false;
        }

        /// <summary>
        /// Método para editar el producto seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: agregar esta función al evento clic del boton editar.
        private void EditarProducto(object sender, RoutedEventArgs e) {
            
        }

        /// <summary>
        /// Lógica del boton de registrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // TODO: agregar esta función al evento clic del boton registar.
        private void RegistrarUsuario(object sender, RoutedEventArgs e) {
            
        }

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
    }
}
