using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
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
        /// Producto seleccionado, viniendo de la pantalla de adm productos.
        /// </summary>
        private Producto SelectedProduct { get; set; }
        public VentasViewMenudeo() {
            int TotalVenta = 0;
            InitializeComponent();
            FillData();

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
        /// Método que se encarga de inicializar los campos.
        /// </summary>
        private void FillData() {
            var data = QuerysProductos.GetTablesFromProductos();
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is Producto producto && producto.Activo != 0 // Solo productos activos en la tabla.
            };
            CollectionView = collectionView;
            DataProducts.ItemsSource = collectionView;
            Log.Debug("Se ha llendado el datagrid de productos.");
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {

        }

        private void txtUpdateCantidad(object sender, TextChangedEventArgs e) {

        }

        private void SeleccionarProducto(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// Lógica detrás del boton de agregar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProduct(object sender, RoutedEventArgs e) {
            var producto = (Producto)((Button)e.Source).DataContext; //contiene todo lo del producto

            if (!ListaVenta.Contains(producto)) ListaVenta.Add(producto);

            if (ListaVenta.Contains(producto)) {
                producto.CantidadDependencia += 1;
                //producto.PrecioMenudeo += producto.PrecioMenudeo;
            }


            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;

            Log.Debug("Se ha agregado un producto a venta.");

        }

        /// <summary>
        /// Lógica detrás del boton de eliminar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelProduct(object sender, RoutedEventArgs e) {
            var producto = (Producto)((Button)e.Source).DataContext; //contiene todo lo del producto

            if (ListaVenta.Contains(producto)) {
                producto.CantidadDependencia -= 1;
                //producto.PrecioMenudeo -= producto.PrecioMenudeo;
            }


            if (producto.CantidadDependencia == 0) ListaVenta.Remove(producto);

            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            Log.Debug("Se ha eliminado una dependecia al producto.");
        }
    }
}
