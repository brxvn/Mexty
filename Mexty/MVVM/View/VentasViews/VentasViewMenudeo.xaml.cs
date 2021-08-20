using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mexty.MVVM.View.VentasViews {

    /// <summary>
    /// Interaction logic for VentasViewMenudeo.xaml
    /// </summary>
    public partial class VentasViewMenudeo : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// Lista de productos en la venta.
        /// </summary>
        private List<ItemInventario> ListaVenta = new();

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<ItemInventario> ListaProductos = new();

        /// <summary>
        /// Venta actual en pantalla.
        /// </summary>
        private Venta VentaActual { get; set; }


        string barCode = null;

        /// <summary>
        /// El último producto seleccionado de la datagrid.
        /// </summary>
        private ItemInventario SelectedItem { get; set; }

        public VentasViewMenudeo() {
            try {
                InitializeComponent();
                FillData();
                NewVenta();
                //ClearFields();
                Log.Debug("Se han inicializado los campos de ventas menudeo.");

                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += UpdateTimerTick;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de ventas menudeo.");
                Log.Error($"Error: {e.Message}");
            }
            //ClearFields();
        }

        /// <summary>
        /// Método que inicializa una nueva venta.
        /// </summary>
        private void NewVenta() {
            var venta = new Venta {
                Cambio = 0,
                Pago = 0,
                TotalVenta = 0,
                DetalleVentaList = new List<ItemInventario>(),
                IdTienda = DatabaseInit.GetIdTienda(),
                UsuarioRegistra = DatabaseInit.GetUsername()
            };
            VentaActual = venta;
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
            var data = QuerysVentas.GetListaInventarioVentas();
            ListaProductos = data;
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is ItemInventario producto && producto.IdTienda == DatabaseInit.GetIdTienda()
            };
            CollectionView = collectionView;
            DataProducts.ItemsSource = collectionView;

            Log.Debug("Se ha llendado el datagrid de ventas menudeo.");
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
                DataProducts.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                // var noNull = new Predicate<object>(producto =>
                // {
                //     if (producto == null) return false;
                //     return ((Producto)producto).Activo == 1;
                // });
                //
                // collection.Filter += noNull;
                DataProducts.ItemsSource = collection;
                CollectionView = collection;
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
            var producto = (ItemInventario)obj;
            if (producto.NombreProducto.Contains(text) ||
                producto.IdProducto.ToString().Contains(text) ||
                producto.TipoProducto.ToLower().Contains(text)) {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Logica del evento item selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            //ClearFields();
            //if (DataProducts.SelectedItem == null) return;
            //Log.Debug("Item seleccionado.");
            //var item = (ItemInventario)DataProducts.SelectedItem;

            //SelectedItem = item;
            //txtDescripcion.Text = item.Comentario;
        }

        /// <summary>
        /// Método que limpia los campos de texto y lo relacionado a la venta.
        /// </summary>
        private void ClearFields() {
            txtRecibido.Text = "";
            txtDescripcion.Text = "";
            txtTotal.Text = "";
            Keyboard.Focus(txtTotal);
            ListaVenta.Clear();
            DataVenta.ItemsSource = null;
            VentaActual = new Venta();
            TotalVenta();
            CambioVenta();
        }

        /// <summary>
        /// Lógica del boton de Pagar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarVenta(object sender, RoutedEventArgs e) {
            Log.Info("Se ha precionado pagar en venta menudeo.");
            ProcesarVenta();
        }

        private void ProcesarVenta() {
            try {
                VentaActual.DetalleVentaList = ListaVenta;
                VentaActual.DetalleVenta = Venta.ListProductosToString(ListaVenta);

                if (txtRecibido.Text == "") {
                    MessageBox.Show("Error: El pago está vacío.");
                    return;
                }

                VentaActual.Pago = decimal.Parse(txtRecibido.Text);
                VentaActual.Cambio = decimal.Parse(txtCambio.Text.TrimStart('$'));

                if (VentaActual.TotalVenta > VentaActual.Pago) {
                    MessageBox.Show("El pago dado no alcanza para cubrir la venta!");
                    return;
                }

                if (!ValidaExistencias()) {
                    MessageBox.Show("No tienes suficientes elementos en tu inventario para la venta!");
                    return;
                }

                var res = QuerysVentas.NewItem(VentaActual);
                if (res == 0) throw new Exception();
                MessageBox.Show("Se ha registrado la venta con exito.");

                ActualizaInventario();


                FillData();
                Ticket ticket = new(txtTotal.Text, txtRecibido.Text, txtCambio.Text, ListaVenta, VentaActual);
                ticket.ImprimirTicketVenta();
                ClearFields();
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al guardar la venta.");
                Log.Error($"Error: {exception.Message}");
                MessageBox.Show(
                    "Ha ocurrido un error al guardar la venta.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que actualiza la existencia en el inventario.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ActualizaInventario() {
            // por cada producto en la cuenta.
            for (var index = 0; index < VentaActual.DetalleVentaList.Count; index++) {
                var item = VentaActual.DetalleVentaList[index];

                var dependencias = item.Dependencias;
                if (dependencias != "") {
                    // Si es producto compuesto.
                    QuerysVentas.UpdateInventario(item.IdProducto,
                        item.CantidadDependencias); // descontamos el producto compuesto

                    // convertimos el string a una lista de objetos con las dependencias del producto compuesto
                    var dependenciasToList = Producto.DependenciasToList(dependencias);
                    for (var i = 0; i < dependenciasToList.Count; i++) {
                        var dependencia = dependenciasToList[i];
                        QuerysVentas.UpdateInventario(dependencia.IdProducto, // ID de producto
                                                                              // la cantidad que se descuenta dada desde la definicion de producto x la cantidad de ese producto que se vendio.
                            dependencia.CantidadDependencia * item.CantidadDependencias);
                    }
                }
                else {
                    // No es producto compuesto
                    var res1 = QuerysVentas.UpdateInventario(item.IdProducto, item.CantidadDependencias);
                    if (res1 == 0) throw new Exception();
                }
            }
        }

        /// <summary>
        /// Método que valida las exitencias antes de que se haga la venta.
        /// </summary>
        private bool ValidaExistencias() {
            foreach (var itemVenta in ListaVenta) { // por cada item en el carrito de la venta.
                if (itemVenta.Dependencias != "") { // SI es un produco compuesto.
                    var dep = Producto.DependenciasToList(itemVenta.Dependencias); // Obtenemos la lista de dependencias.
                    foreach (var dependencia in dep) { // Por cada dependencia.
                        var alcanza =
                            ListaProductos.Where(producto => dependencia.IdProducto == producto.IdProducto) //donde el Id del producto de la dependencia coincida con el del inventario
                            .All(x => x.Cantidad >= dependencia.CantidadDependencia * itemVenta.CantidadDependencias); // la cantidad en existencia tiene que ser menor o igual a la que se va a vender

                        if (!alcanza) return false;
                    }
                }
                else {
                    var alcanza = ListaProductos.Where(producto => producto.IdProducto == itemVenta.IdProducto)
                        .All(x => x.Cantidad >= itemVenta.CantidadDependencias);

                    if (!alcanza) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Método que mantiene actualizado el total de la venta.
        /// </summary>
        private void TotalVenta() {
            decimal total = 0;
            for (var index = 0; index < ListaVenta.Count; index++) {
                var producto = ListaVenta[index];
                total += producto.PrecioVenta;
            }

            VentaActual.TotalVenta = total;

            var totalFormated = string.Format("{0:C}", total);

            txtTotal.Text = totalFormated;
            txtRecibido.IsReadOnly = false;
        }

        /// <summary>
        /// Lógica detrás del boton de agregar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddProduct(object sender, RoutedEventArgs e) {
            var producto = (ItemInventario)((Button)e.Source).DataContext;

            if (!ListaVenta.Contains(producto)) ListaVenta.Add(producto);

            if (ListaVenta.Contains(producto)) {
                producto.CantidadDependencias += 1;
                producto.PrecioVenta = (producto.PrecioMenudeo * producto.CantidadDependencias);
                txtDescripcion.Text = producto.Comentario;
            }

            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            Keyboard.Focus(txtRecibido);
            TotalVenta();
            CambioVenta();
            Log.Debug("Se ha agregado un producto a venta.");
        }

        /// <summary>
        /// Lógica detrás del boton de eliminar producto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelProduct(object sender, RoutedEventArgs e) {
            var producto = (ItemInventario)((Button)e.Source).DataContext;

            if (ListaVenta.Contains(producto)) {
                producto.CantidadDependencias -= 1;
                producto.PrecioVenta = producto.PrecioMenudeo * producto.CantidadDependencias;
            }

            if (producto.CantidadDependencias == 0) ListaVenta.Remove(producto);
            txtDescripcion.Text = producto.Comentario;

            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            TotalVenta();
            CambioVenta();
            Log.Debug("Se ha eliminado un producto de la venta.");
        }


        /// <summary>
        /// Validacion de solo letras y numeros para la dirección, así como el numeral.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsDigit(x) || '.'.Equals(x));
        }

        /// <summary>
        /// Método que actualiza el campo de total venta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUpdateVenta(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtTotal.Text = textBox.Text;
        }

        private void RecibidoUpdate(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtRecibido.Text = string.Format("{0:C}", textBox.Text);

            CambioVenta();
        }

        private void CambioVenta() {
            decimal total = txtTotal.Text == "" ? 0 : Convert.ToDecimal(txtTotal.Text.TrimStart('$'));
            decimal recibido = txtRecibido.Text == "" ? 0 : Convert.ToDecimal(txtRecibido.Text.Trim('$'));
            decimal cambio = Math.Max(0, recibido - total);
            string cambiFormated = string.Format("{0:C}", cambio);
            txtCambio.Text = cambiFormated;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e) {
            foreach (var item in ListaVenta) {
                item.CantidadDependencias = 0;
            }

            ClearFields();
            SetFocus(sender, e);
        }

        private void SetFocus(object sender, RoutedEventArgs e) {
            txtTotal.Focus();
        }

        private void AddFromScannerToGrid(string id) {
            try {
                id.Trim('\r');
                var idProdutco = id == "" ? 0 : int.Parse(id);

                foreach (var item in ListaProductos) {
                    if (item.IdProducto == idProdutco) {

                        if (!ListaVenta.Contains(item)) {
                            ListaVenta.Add(item);
                        }

                        if (ListaVenta.Contains(item)) {
                            item.CantidadDependencias += 1;
                            item.PrecioVenta = item.PrecioMenudeo * item.CantidadDependencias;
                        }

                        DataVenta.ItemsSource = null;
                        DataVenta.ItemsSource = ListaVenta;

                        TotalVenta();
                        CambioVenta();
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e.ToString());
            }

        }

        private void DataVenta_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                ChangeCantidad();
            }
        }

        private void ChangeCantidad() {
            var cellInfo = DataVenta.SelectedCells[0];
            var content = cellInfo.Column.GetCellContent(cellInfo.Item);
            var item2 = cellInfo.Item as ItemInventario;
            var nuevaCantidad = int.Parse((content as TextBox).Text.Trim('x'));

            foreach (var item in ListaProductos) {
                if (item.IdProducto == item2.IdProducto) {

                    if (ListaVenta.Contains(item)) {
                        if (nuevaCantidad != 0) {
                            item.CantidadDependencias = nuevaCantidad;
                        }
                        else if (nuevaCantidad == 0) ListaVenta.Remove(item);
                        else {
                            item.CantidadDependencias += 1;
                        }

                        item.PrecioVenta = item.PrecioMenudeo * item.CantidadDependencias;
                    }

                    DataVenta.ItemsSource = null;
                    DataVenta.ItemsSource = ListaVenta;

                    TotalVenta();
                    CambioVenta();
                }
            }
        }

        private void txtRecibido_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                Log.Debug("Enter en el recibido, se procede a procesar la venta.");
                ProcesarVenta();
            }
        }

        private void UserControl_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            barCode += e.Text;

            if (barCode.Length == 9) {
                AddFromScannerToGrid(barCode);
                barCode = null;
            }
        }
    }
}

