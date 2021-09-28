using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.View.VentasViews {
    /// <summary>
    /// Interaction logic for VentasViewMayoreo.xaml
    /// </summary>
    public partial class VentasViewMayoreo : UserControl {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// Lista de productos en la venta.
        /// </summary>
        public List<ItemInventario> ListaVenta = new();

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<ItemInventario> ListaProductos = new();

        /// <summary>
        /// Lista de clientes dada por la bd
        /// </summary>
        private List<Cliente> ListaClientes { get; set; }

        /// <summary>
        /// Venta actual en pantalla.
        /// </summary>
        private Venta VentaActual { get; set; }
        string barCode;
        private bool _blockHandlers;

        public VentasViewMayoreo() {
            try {
                InitializeComponent();
                FillData();
                NewVenta();

                Log.Debug("Se han inicializado los campos de ventas mayoreo.");

                var timer = new DispatcherTimer();
                timer.Tick += UpdateTimerTick;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
                lblSucursal.Content = DatabaseInit.GetNombreTiendaIni();

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de ventas mayoreo.");
                Log.Error($"Error: {e.Message}");
            }
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
                IdTienda = DatabaseInit.GetIdTiendaIni(),
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
            var data = QuerysVentas.GetListaInventarioVentas(true);
            ListaProductos = data;
            var collectionView = new ListCollectionView(data) {
                Filter = (e) => e is ItemInventario producto && producto.Cantidad > 0
            };
            CollectionView = collectionView;
            DataProducts.ItemsSource = collectionView;
            SortDataGrid(DataProducts, 0);

            //Keyboard.Focus(txtID); // TODO: checar esto

            Log.Debug("Se ha llendado el datagrid de ventas mayoreo.");

            ComboCliente.Items.Clear();
            var dataClientes = QuerysClientes.GetTablesFromClientes();
            ListaClientes = dataClientes;
            foreach (var client in dataClientes) {
                ComboCliente.Items.Add($"{client.IdCliente.ToString()} {client.Nombre} Deuda: {client.Debe.ToString()}");
            }

            //ComboCliente.SelectedIndex = 0;
            Log.Debug("Se ha llendado el combobox de clientes.");
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
        /// Lógica para el campo de búsqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            if (_blockHandlers) {
                return;
            }
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

                var collectionView = new ListCollectionView(ListaProductos) {
                    Filter = (e => e is ItemInventario producto && producto.Cantidad > 0)
                };

                DataProducts.ItemsSource = collectionView;
                CollectionView = collection;
            }

        }

        /// <summary>
        /// Lógica para el filtro del data grid.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool FilterLogic(object obj, string text) {
            var producto = (ItemInventario)obj;
            text = text.ToLower();
            if (text.StartsWith("000")) {
                try {
                    int result = Int32.Parse(text);
                    if (producto.IdProducto.ToString() == result.ToString()) {
                        if (!ListaVenta.Contains(producto)) ListaVenta.Add(producto);

                        if (ListaVenta.Contains(producto)) {
                            producto.CantidadDependencias += 1;
                            producto.PrecioVenta = producto.PrecioMayoreo * producto.CantidadDependencias;
                        }
                        DataVenta.ItemsSource = null;
                        DataVenta.ItemsSource = ListaVenta;

                        TotalVenta();
                        CambioVenta();
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

        // TODO: terminar este y el otro.
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
        }

        /// <summary>
        /// Método que limpia los campos de texto y lo relacionado a la venta.
        /// </summary>
        private void ClearFields() {
            txtRecibido.Text = "";
            txtTotal.Text = "";
            txtDescripcion.Text = "";
            txtComentario.Text = "";
            ListaVenta.Clear();
            DataVenta.ItemsSource = null;
            VentaActual = new Venta();
            TotalVenta();
            CambioVenta();
            Keyboard.Focus(txtTotal);
        }

        /// <summary>
        /// Lógica del boton de Pagar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarVenta(object sender, RoutedEventArgs e) {
            Log.Info("Se ha precionado pagar en venta mayoreo.");
            ProcesarVenta();
            NewVenta();
        }

        private void ProcesarVenta() {
            try {
                TotalVenta();
                VentaActual.DetalleVentaList = ListaVenta;
                VentaActual.DetalleVenta = Venta.ListProductosToString(ListaVenta, true);
                VentaActual.IdTienda = DatabaseInit.GetIdTiendaIni();

                if (ListaVenta.Count == 0) {
                    MessageBox.Show("Error: No hay elementos en la cuenta.");
                    return;
                }

                if (ComboCliente.SelectedItem == null) {
                    MessageBox.Show("Necesita seleccionar un cliente primero.");
                    return;
                }
                VentaActual.IdCliente = int.Parse(ComboCliente.SelectedItem.ToString()?.Split(' ')[0] ?? throw new Exception());


                if (txtRecibido.Text == "") {
                    MessageBox.Show("Error: El pago está vacío.");
                    return;
                }

                VentaActual.Pago = decimal.Parse(txtRecibido.Text);
                VentaActual.Cambio = decimal.Parse(txtCambio.Text.TrimStart('$'));
                VentaActual.Comentarios = txtComentario.Text;

                if (VentaActual.TotalVenta > VentaActual.Pago) {
                    var buttons = MessageBoxButton.YesNo;
                    var resYesNo = MessageBox.Show("El pago dado no alcanza para cubrir la venta! ¿Desea agregarlo a la deuda del cliente?", "Pago Insuficiente", buttons);
                    var selectedClientId = int.Parse(ComboCliente.SelectedItem.ToString().Split(' ')[0]);
                    if (resYesNo == MessageBoxResult.Yes) {
                        ActualizaDeuda(selectedClientId);
                    }
                    else {
                        return;
                    }
                }

                if (!ValidaExistencias()) {
                    MessageBox.Show("No tienes suficientes elementos en tu inventario para la venta!");
                    return;
                }
                Log.Debug("Existencias validadas.");


                var res = QuerysVentas.NewItem(VentaActual, true);

                if (res == 0) throw new Exception();
                MessageBox.Show("Se ha registrado la venta con éxito.");

                ActualizaInventario();

                FillData();
                Ticket ticket = new(txtTotal.Text, txtRecibido.Text, txtCambio.Text, ListaVenta, VentaActual);
                ticket.ImprimirTicketVenta(false);
                ClearFields();
                txtTotal.Focus();
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
        /// Método que maneja el popUp de venta a credito
        /// </summary>
        /// <returns>True si el cargo fue exitoso.</returns>
        private bool VentaCredito() {
            try {
                if (VentaActual.TotalVenta > VentaActual.Pago) {
                    const MessageBoxButton buttons = MessageBoxButton.YesNo;
                    var resYesNo = MessageBox.Show("El pago dado no alcanza para cubrir la venta! ¿Desea agregarlo a la deuda del cliente?", "Pago Insuficiente", buttons);
                    if (resYesNo == MessageBoxResult.Yes) {
                        var selectedClientId = int.Parse(ComboCliente.SelectedItem.ToString().Split(' ')[0]);
                        ActualizaDeuda(selectedClientId);
                    }
                    else {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al hacer el cargo a la cuenta del cliente.");
                Log.Error($"Error: {e.Message}");
                throw;
            }

        }

        /// <summary>
        /// Método que se encarga de actualizar la deuda.
        /// </summary>
        private void ActualizaDeuda(int selectedClientId) {
            // Iteramos entre la lista de clientes.
            for (var index = 0; index < ListaClientes.Count; index++) {
                var cliente = ListaClientes[index];

                if (cliente.IdCliente == selectedClientId) {
                    // Encontramos el cliente en la lista por id.

                    var deudaAnterior = cliente.Debe;
                    cliente.Debe += VentaActual.TotalVenta - VentaActual.Pago;

                    // Creamos el historial.
                    var log = new LogCliente() {
                        IdCliente = selectedClientId,
                        Mensaje = $"Deuda aumentada por venta de ${deudaAnterior.ToString(CultureInfo.InvariantCulture)} a ${cliente.Debe.ToString(CultureInfo.InvariantCulture)}",
                        UsuarioRegistra = DatabaseInit.GetUsername(),
                        //FechaRegistro = DatabaseHelper.GetCurrentTimeNDate()
                    };

                    var resDeud = QuerysMovClientes.NewLogCliente(log);
                    if (resDeud == 0)
                        throw new Exception("No se ha alterado ninguna columna al guardar el movimiento de cliente");

                    // Acualizamos la deuda.
                    var resQ = QuerysClientes.UpdateData(cliente);
                    if (resQ == 0) throw new Exception("No se actualizó la deuda del cliente.");
                }
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
                        item.CantidadDependencias, true); // descontamos el producto compuesto

                    // convertimos el string a una lista de objetos con las dependencias del producto compuesto
                    var dependenciasToList = Producto.DependenciasToList(dependencias);
                    for (var i = 0; i < dependenciasToList.Count; i++) {
                        var dependencia = dependenciasToList[i];
                        QuerysVentas.UpdateInventario(dependencia.IdProducto, // ID de producto
                                                                              // la cantidad que se descuenta dada desde la definicion de producto x la cantidad de ese producto que se vendio.
                            dependencia.CantidadDependencia * item.CantidadDependencias, true);
                    }
                }
                else {
                    // No es producto compuesto
                    var res1 = QuerysVentas.UpdateInventario(item.IdProducto, item.CantidadDependencias, true);
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
                producto.PrecioVenta = producto.PrecioMayoreo * producto.CantidadDependencias;
            }
            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            txtDescripcion.Text = producto.Comentario;
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
                producto.PrecioVenta = producto.PrecioMayoreo * producto.CantidadDependencias;
            }

            if (producto.CantidadDependencias == 0) ListaVenta.Remove(producto);

            DataVenta.ItemsSource = null;
            DataVenta.ItemsSource = ListaVenta;
            TotalVenta();
            CambioVenta();
            Log.Debug("Se ha eliminado un producto de la venta.");
        }

        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsDigit(x) || '.'.Equals(x));
        }

        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x) || '.'.Equals(x) || '/'.Equals(x) || ','.Equals(x) || '('.Equals(x) || ')'.Equals(x));
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

        private void Filtrar() {
            var id = 300;

            var query =
                from item in ListaProductos.AsParallel()
                where item.IdProducto == id && item.Cantidad > 0
                select item;

            if (query.Any()) {
                var itemInventarios = query.ToArray();
                MessageBox.Show($"{itemInventarios.First().IdProducto.ToString()} {itemInventarios.First().NombreProducto}");
            }
            else {
                MessageBox.Show("no se encontro el item");
            }
        }

        private void AddFromScannerToGrid(string id) {
            var finded = false;
            var nombreProducto = "";
            try {
                id.Trim('\r');
                var idProdutco = id == "" ? 0 : int.Parse(id);

                for (var index = 0; index < ListaProductos.Count; index++) {
                    var item = ListaProductos[index];

                    if (item.IdProducto == idProdutco) {
                        nombreProducto = item.NombreProducto;

                        finded = true;
                        if (!ListaVenta.Contains(item)) {
                            ListaVenta.Add(item);
                        }

                        if (ListaVenta.Contains(item)) {
                            item.CantidadDependencias += 1;
                            item.PrecioVenta = item.PrecioMayoreo * item.CantidadDependencias;
                        }

                        DataVenta.ItemsSource = null;
                        DataVenta.ItemsSource = ListaVenta;

                        TotalVenta();
                        CambioVenta();
                        break;
                    }
                }
                if (!finded) {
                    MessageBox.Show($"Error: El producto no esta en tu inventaio o no tiene existencias.");
                }

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al agregar el producto con el scanner.");
                Log.Error($"Error: {e.Message}");
            }
        }

        private void DataVenta_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                ChangeCantidad();
            }
            else if (e.Key == Key.F1) {
                SetFocus(sender, e);
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
                        item.PrecioVenta = item.PrecioMayoreo * item.CantidadDependencias;
                    }


                    DataVenta.ItemsSource = null;
                    DataVenta.ItemsSource = ListaVenta;

                    TotalVenta();
                    CambioVenta();
                    txtTotal.Focus();
                }
            }
            txtTotal.Focus();
            if (txtTotal.IsFocused) {
                MessageBox.Show("Focus ventana recibido");
            }
        }


        private void txtRecibido_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                Log.Debug("Enter en el recibido, se procede a procesar la venta.");
                ProcesarVenta();
                NewVenta();
            }
        }

        ///// <summary>
        ///// Método que se encarga de manejar el imput del scanner.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private async void UserControl_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            //await Task.Delay(250);
            //txtTotal.Focus();
            //barCode += e.Text;

            //if (barCode.Length == 9) {
            //    await Task.Delay(250);
            //    AddFromScannerToGrid(barCode);
            //    barCode = "";
            //}

        }

        private void SetFocus(object sender, RoutedEventArgs e) {
            txtTotal.Focus();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.F1) {
                txtTotal.Focus();
                if (txttotal.isfocused) {
                    messagebox.show("f1 presionado.\nfocus ventana recibido");
                }
            }
        }

        private async void UserControl_TextInput(object sender, TextCompositionEventArgs e) {

            barCode += e.Text;

            if (barCode.Length == 9) {
                await Task.Delay(250);
                AddFromScannerToGrid(barCode);
                barCode = "";
            }
        }
    }
}
