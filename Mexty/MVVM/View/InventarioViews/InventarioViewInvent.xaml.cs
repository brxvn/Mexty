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

namespace Mexty.MVVM.View.InventarioViews {

    /// <summary>
    /// Interaction logic for InventarioViewInvent.xaml
    /// </summary>
    public partial class InventarioViewInvent : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista que contiene la cantidad y el comentario de cada item del modulo de inventario.
        /// </summary>
        private List<ItemInventario> ListaItems { get; set; }

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// El último producto seleccionado de la datagrid.
        /// </summary>
        private ItemInventario SelectedItem { get; set; }

        public InventarioViewInvent() {

            try {
                InitializeComponent();
                FillData();

                Log.Debug("Se han inicializado los campos del modulo de inventario exitosamente.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo de inventario.");
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
        /// Método que llena el datagrid y los combobox.
        /// </summary>
        private void FillData() {
            var data = Database.GetItemsFromInventario();
            ListaItems = data;
            DataProducts.ItemsSource = data;
            // TODO: ver que onda con la collectionView.
            Log.Debug("Se ha llenado la datagrid de manera exitosa.");

        }

        /// <summary>
        /// Método que reaciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

            ClearFields();
            if (DataProducts.SelectedItem == null) return;
            Log.Debug("Producto seleccionado.");
            var item = (ItemInventario) DataProducts.SelectedItem;

            SelectedItem = item;
            txtComentario.Text = item.Comentario;
            txtCantidad.Text = item.Cantidad.ToString();
            txtPiezas.Text = item.Piezas.ToString();

            Guardar.IsEnabled = true;
        }

        /// <summary>
        /// Método que limpia los campos de texto.
        /// </summary>
        private void ClearFields() {
            Guardar.IsEnabled = false;
            txtComentario.Text = "";
            txtCantidad.Text = "";
            txtPiezas.Text = "";
            Log.Debug("Se han limpiado los campos de inventario.");
        }

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
                    //if (producto == null) return false;
                    //return ((ItemInventario)producto).Activo == 1;
                    return false;
                });

                collection.Filter += noNull;
                DataProducts.ItemsSource = collection;
                CollectionView = collection;
                ClearFields();
            }

            SearchBox.Text = tbx.Text;

        }

        /// <summary>
        /// Lógica de la busqueda.
        /// </summary>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            text = text.ToLower();
            var producto = (ItemInventario)obj;
            if (producto.NombreProducto.Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text) ||
                producto.Medida.ToLower().Contains(text) //||
                ) {
                //return producto.Activo == 1;
                return true;
            }
            return false;

        }


        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
        }

        private void RegistrarProducto(object sender, RoutedEventArgs e) {

        }

        private void txtUpdateDisponible(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtComentario.Text = textbox.Text;
        }

        private void txtUpdatePiezas(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtPiezas.Text = textbox.Text;
        }

        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(c => char.IsLetter(c));
        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => Char.IsDigit(x) || '.'.Equals(x));
        }

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }

    }
}
