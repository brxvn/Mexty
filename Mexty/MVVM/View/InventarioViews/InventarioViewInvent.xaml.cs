using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private int idTienda = DatabaseInit.GetIdTienda();


        private List<Sucursal> dataSucursal = QuerysSucursales.GetTablesFromSucursales();

        public InventarioViewInvent() {

            try {
                InitializeComponent();
                FillData();

                FillSucursales();
                ClearFields();

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
        public void FillData() {
            var data = QuerysInventario.GetItemsFromInventario();
            //ListaItems = data;

            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is ItemInventario producto //&& producto.ac == idSucursal // Solo productos activos en la tabla.
            };
            //foreach (var item in collectionView) {
            //    List.Add((ItemInventario)item);
            //}
            CollectionView = collectionView;
            //DataProducts.ItemsSource = List;
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
        /// Método que llena el datagrid y los combobox.
        /// </summary>
        private void FillData(int idSucursal) {
            var data = QuerysInventario.GetItemsFromInventarioById(idSucursal);
            ListaItems = data;

            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is ItemInventario producto // Solo productos activos en la tabla.
            };

            CollectionView = collectionView;
            DataProducts.ItemsSource = null;
            DataProducts.ItemsSource = collectionView;
            Log.Debug("Se ha llenado la datagrid de manera exitosa.");
        }

        /// <summary>
        /// Método para filtar la lista inventario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SucursalSeleccionada(object sender, SelectionChangedEventArgs e) {
            var newList = ComboSucursal.SelectedIndex + 1;
            if (newList == idTienda) {
                FillData();
            }
            else {
                FillData(newList);
            }
        }

        private void FillSucursales() {
            foreach (var sucu in dataSucursal) {
                ComboSucursal.Items.Add(sucu.NombreTienda);
                if (sucu.IdTienda == idTienda) {
                    ComboSucursal.SelectedIndex = idTienda - 1;
                }
            }

            Log.Debug("Se ha llenado el combo de sucursal");
        }

        /// <summary>
        /// Método que reaciona al evento SelectionChanged del datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e) {

            ClearFields();
            if (DataProducts.SelectedItem == null) return;
            Log.Debug("Item seleccionado.");
            var item = (ItemInventario)DataProducts.SelectedItem;

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
            DataProducts.SelectedItem = null;
            ClearFields();

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
            var res = QuerysInventario.UpdateData(newItem);
            if (res > 0) {
                Log.Debug("Se ha editado un producto.");
                MessageBox.Show($"Se ha editado el producto {newItem.IdProducto.ToString()} {newItem.TipoProducto} {newItem.NombreProducto}");
            }

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

        private void ReporteInventario(object sender, RoutedEventArgs e) {
            ReportesInventario reports = new();
            reports.ReporteInventario();
        }

        public void ActualizarData(object sender, RoutedEventArgs e) {
            FillData();
        }
    }
}
