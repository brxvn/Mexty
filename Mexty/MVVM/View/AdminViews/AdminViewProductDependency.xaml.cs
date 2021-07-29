using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Data;
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
using Common.Logging;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewProductDependency.xaml
    /// </summary>
    public partial class AdminViewProductDependency : Window {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Producto> ListaProductos = new();

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }
        public AdminViewProductDependency() {
            try {
                InitializeComponent();
                FillData();
                Log.Debug("Se han inicializado los campos de Dependecia de productos.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de Dependencia de productos.");
                Log.Error($"Error {e.Message}");
            }

        }

        /// <summary>
        /// Método que se encarga de inicializar los campos.
        /// </summary>
        private void FillData() {
            var data = Database.GetTablesFromProductos();
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is Producto producto && producto.Activo != 0 // Solo productos activos en la tabla.
            };
            CollectionView = collectionView;
            DataProductos.ItemsSource = collectionView;
            Log.Debug("Se ha llendado el datagrid de productos.");
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha precionado el boton de cerrar ventana.");
            Close();
        }

        /// <summary>
        /// Método que valida el imput solo de letras.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(c => char.IsLetter(c));
        }

        private void TextUpdateNombre(object sender, TextChangedEventArgs e) {

        }

        private void SaveProduct(object sender, RoutedEventArgs e) {
            Close();
        }

        /// <summary>
        /// Lógica detrás del boton de agregar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProduct(object sender, RoutedEventArgs e) {
            Producto producto = (Producto)((Button)e.Source).DataContext; //contiene todo lo del producto
            ListaProductos.Add(producto);
            DataActual.ItemsSource = null;
            DataActual.ItemsSource = ListaProductos;
            Log.Debug("Se ha agregado una dependecia al producto.");
        }

        /// <summary>
        /// Lógica detrás del boton de eliminar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelProduct(object sender, RoutedEventArgs e) {
            var index = DataActual.Items.IndexOf(DataActual.CurrentItem);
            Producto producto = (Producto)((Button)e.Source).DataContext; //contiene todo lo del producto
            
            ListaProductos.RemoveAt(index);
            DataActual.ItemsSource = null;
            DataActual.ItemsSource = ListaProductos;
            Log.Debug("Se ha eliminado una dependecia al producto.");
        }
    }
}
