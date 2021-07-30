using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections;
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
        private List<Producto> ListaDependecias = new();

        /// <summary>
        /// Producto seleccionado, viniendo de la pantalla de adm productos.
        /// </summary>
        private Producto SelectedProduct { get; set; }

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }
        public AdminViewProductDependency(Producto selectedProduct) {
            try {
                InitializeComponent();
                FillData();
                SelectedProduct = selectedProduct;
                if (selectedProduct.DependenciasText == "") {
                    selectedProduct.Dependencias ??= new List<Producto>();
                    ListaDependecias = selectedProduct.Dependencias;
                    Log.Debug("No se han encontrado dependencias, generando lista.");
                }
                else {
                    ProcessData(SelectedProduct);
                }

                txtNombreProducto.Text = $"{selectedProduct.IdProducto.ToString()} {selectedProduct.TipoProducto} {selectedProduct.NombreProducto}";
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

        /// <summary>
        /// Método que prosesa la lista de dependencias.
        /// </summary>
        /// <param name="selectedProduct"></param>
        // TODO: Ver que onda cuando un producto se elimina.
        private void ProcessData(Producto selectedProduct) {
            ListaDependecias = Producto.DependenciasToList(SelectedProduct.DependenciasText);

            var data = DataProductos.ItemsSource;
            for (var index = 0; index < ListaDependecias.Count; index++) {
                var dependecia = ListaDependecias[index];
                foreach (Producto producto in data) {
                    if (dependecia.IdProducto == producto.IdProducto) {
                        dependecia.NombreProducto = producto.NombreProducto;
                        dependecia.TipoProducto = producto.TipoProducto;
                    }
                }
            }
            DataActual.ItemsSource = ListaDependecias;

            Log.Debug("Se han leido las dependencias de manera exitosa.");
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
            try {
                Log.Debug("Se ha precionado el boton de guardar dependencias.");
                SelectedProduct.Dependencias = ListaDependecias;
                SelectedProduct.DependenciasText = Producto.DependenciasToString(SelectedProduct.Dependencias);
                Log.Debug("Se ha convertido el producto a texto de manera exitosa.");
                Database.UpdateData(SelectedProduct);
                Log.Debug("Se ha actualizado el producto en la base de datos.");

                Close();

            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al guardar las dependencias.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Lógica detrás del boton de agregar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProduct(object sender, RoutedEventArgs e) {
            var producto = (Producto)((Button)e.Source).DataContext; //contiene todo lo del producto

            if (!ListaDependecias.Contains(producto)) ListaDependecias.Add(producto);

            if (ListaDependecias.Contains(producto)) producto.CantidadDependencia += 1;

            DataActual.ItemsSource = null;
            DataActual.ItemsSource = ListaDependecias;

            Log.Debug("Se ha agregado una dependecia al producto.");
        }

        /// <summary>
        /// Lógica detrás del boton de eliminar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelProduct(object sender, RoutedEventArgs e) {
            var producto = (Producto)((Button)e.Source).DataContext; //contiene todo lo del producto

            if (ListaDependecias.Contains(producto)) producto.CantidadDependencia -= 1;

            if (producto.CantidadDependencia == 0) ListaDependecias.Remove(producto);

            DataActual.ItemsSource = null;
            DataActual.ItemsSource = ListaDependecias;
            Log.Debug("Se ha eliminado una dependecia al producto.");
        }
    }
}
