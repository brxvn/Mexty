using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Mexty.MVVM.View.InventarioViews {

    /// <summary>
    /// Interaction logic for InventarioViewInvent.xaml
    /// </summary>
    public partial class InventarioViewInventMatriz : UserControl {
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

        public InventarioViewInventMatriz() {

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

            lblSucursal.Content = DatabaseInit.GetNombreTiendaIni();

            if (DatabaseInit.GetIdRol().Equals(3)) {
                AltInventario.Visibility = Visibility.Collapsed;
                AsignInventario.Visibility = Visibility.Collapsed;
                MovInventario.Visibility = Visibility.Collapsed;
            }
            else {
                AltInventario.Visibility = Visibility.Visible;
                AsignInventario.Visibility = Visibility.Visible;
                MovInventario.Visibility = Visibility.Visible;
            }
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
            var data = QuerysInventario.GetTablesFromInventarioMatrix();
            ListaItems = data;

            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is ItemInventario producto //&& producto.Activo != 0 // Solo productos activos en la tabla.
            };

            CollectionView = collectionView;
            DataProducts.ItemsSource = collectionView;
            SortDataGrid(DataProducts, 4);
            Log.Debug("Se ha llenado la datagrid de manera exitosa.");
        }

        /// <summary>
        /// Ordenar por piezas de manera ascendente 
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="columnIndex"></param>
        /// <param name="sortDirection"></param>
        void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending) {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns) {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }

        /// <summary>
        /// Método que reaciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            ClearFields();
            txtComentario.IsReadOnly = false;
            txtCantidad.IsReadOnly = false;
            if (DataProducts.SelectedItem == null) return;
            Log.Debug("Item seleccionado.");
            var item = (ItemInventario) DataProducts.SelectedItem;

            SelectedItem = item;
            txtComentario.Text = item.Comentario;
            txtCantidad.Text = item.Cantidad.ToString();

            Guardar.IsEnabled = true;
        }

        /// <summary>
        /// Método que limpia los campos de texto.
        /// </summary>
        private void ClearFields() {
            Guardar.IsEnabled = false;
            txtComentario.IsReadOnly = true;
            txtCantidad.IsReadOnly = true;
            txtComentario.Text = "";
            txtCantidad.Text = "";
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
                    if (producto == null) return false;
                    return true;
                    //return ((ItemInventario)producto).Activo == 1;
                    //return false;
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
            var producto = (ItemInventario)obj;
            text = text.ToLower();
            if (text.StartsWith("00000")) {
                try {
                    int result = Int32.Parse(text);
                    if (producto.NombreProducto.ToLower().Contains(text) ||
                        producto.IdProducto.ToString().Contains(result.ToString()) ||
                        producto.TipoProducto.ToLower().Contains(text)) {
                        return true;
                    }
                }
                catch (Exception e) {
                    Log.Warn(e.Message);
                }
            }
            else if (producto.NombreProducto.ToLower().Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text)) {
                return true;
            }

            return false;

        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
            InitializeComponent();
        }

        private void RegistrarProducto(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado el boton de guardar.");

            var newItem = new ItemInventario {
                Comentario = txtComentario.Text,
                Cantidad = int.Parse(txtCantidad.Text),
            };

            if (!Validar(newItem)) {
                Log.Warn("El objeto tipo ItemInventario no ha pasado las vaidaciones.");
                return;
            }
            Log.Debug("El objeto tipo ItemInventario ha pasado las validaciones.");

            newItem.NombreProducto = SelectedItem.NombreProducto;
            newItem.TipoProducto = SelectedItem.TipoProducto;
            newItem.IdProducto = SelectedItem.IdProducto;
            newItem.IdRegistro = SelectedItem.IdRegistro;
            var res = QuerysInventario.UpdateData(newItem, true);
            if (res > 0) {
                Log.Debug("Se ha editado un producto.");
                MessageBox.Show($"Se ha editado el producto {newItem.IdProducto.ToString()} {newItem.TipoProducto} {newItem.NombreProducto}");
            }
            //DataProducts.ItemsSource = null;
            FillData();
        }

        /// <summary>
        /// Valida un objeto tipo ItemInventario.
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        private static bool Validar(ItemInventario newItem) {
            try {
                var validator = new ItemValidation();
                var results = validator.Validate(newItem);
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

        private void txtUpdateDisponible(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtCantidad.Text = textbox.Text;
        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtComentario.Text = textbox.Text;
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

        /// <summary>
        /// Lógica detrás del boton de Reporte inventario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReporteInventario(object sender, RoutedEventArgs e) {
            ReportesInventario reports = new();
            reports.ReporteInventario();
        }

        /// <summary>
        /// Lógica detrás del boton de alta inventario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AltaInventario(object sender, RoutedEventArgs e) {
            AltaInventario altaInventario = new AltaInventario();
            altaInventario.ShowDialog();
            FillData();
        }

        /// <summary>
        /// Lógica detrás del boton de Asignar inventario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AsingInventario(object sender, RoutedEventArgs e) {
            AsignacionInventario asignacionInventario = new AsignacionInventario();
            asignacionInventario.ShowDialog();
            FillData();
        }

        /// <summary>
        /// Lógica detrás del boton de Movimientos de inventario.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovimInventario(object sender, RoutedEventArgs e) {
            MovimientosInventario movimientosInventario = new MovimientosInventario();
            movimientosInventario.ShowDialog();
        }
    }
}
