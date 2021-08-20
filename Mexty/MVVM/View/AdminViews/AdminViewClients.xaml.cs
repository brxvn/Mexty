using System;
using System.Collections.Generic;
using System.Globalization;
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
using FluentValidation;
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;
using MySqlX.XDevAPI;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewClients.xaml
    /// </summary>
    public partial class AdminViewClients : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Cliente> ListaClientes { get; set; }

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// El último cliente seleccionado de la datagrid.
        /// </summary>
        private Cliente SelectedClient { get; set; }

        public AdminViewClients() {

            Log.Info("Iniciado modulo clientes");

            try {
                InitializeComponent();
                FillData();
                ClearFields();
                Log.Debug("Se han inicializado los campos del modulo de clientes.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo de clientes.");
                Log.Error($"Error: {e.Message}");
            }

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
        /// Método que llena la datagrid con los Clientes.
        /// </summary>
        private void FillData() {
            var dataClientes = QuerysClientes.GetTablesFromClientes();
            ListaClientes = dataClientes;
            var collectionView = new ListCollectionView(dataClientes) {
                Filter = e => e is Cliente cliente
            };
            CollectionView = collectionView;
            DataClientes.ItemsSource = collectionView;
            Log.Debug("Se ha llenado datagrid de clientes.");
        }

        /// <summary>
        /// Lógica para el evento SelectedItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            ClearFields();

            txtNombreCliente.IsReadOnly = true;
            txtApPaternoCliente.IsReadOnly = true;
            txtApMaternoCliente.IsReadOnly = true;

            if (DataClientes.SelectedItem == null) return; // si no hay nada selecionado, bye
            Log.Debug("Cliente seleccionado.");
            var cliente = (Cliente)DataClientes.SelectedItem;

            SelectedClient = cliente;
            txtNombreCliente.Text = cliente.Nombre;
            txtApPaternoCliente.Text = cliente.ApPaterno;
            txtApMaternoCliente.Text = cliente.ApMaterno;
            txtTelefono.Text = cliente.Telefono;
            txtDireccion.Text = cliente.Domicilio;
            txtComentario.Text = cliente.Comentario;
            txtDeuda.Text = cliente.Debe.ToString(CultureInfo.InvariantCulture);
            SearchBox.Text = "";
            Eliminar.IsEnabled = true;
            Eliminar.ToolTip = "Eliminar Cliente.";
            Guardar.IsEnabled = true;
        }

        /// <summary>
        /// Método que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            Guardar.IsEnabled = false;
            Eliminar.IsEnabled = false;
            Eliminar.ToolTip = "Seleccione un cliente para eliminar.";
            txtNombreCliente.Text = "";
            txtApPaternoCliente.Text = "";
            txtApMaternoCliente.Text = "";
            txtTelefono.Text = "";
            txtDireccion.Text = "";
            txtComentario.Text = "";
            txtDeuda.Text = "";
            SearchBox.Text = "";
            txtNombreCliente.IsReadOnly = false;
            txtApPaternoCliente.IsReadOnly = false;
            txtApMaternoCliente.IsReadOnly = false;
            EnableGuardar();
            SelectedClient = null;
            Log.Debug("Se han limpiado los campos de texto.");
        }

        /// <summary>
        /// Logica para el campo de búsqueda
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            var tbx = sender as TextBox;
            var collection = CollectionView;
            if (tbx != null && tbx.Text != "") {
                var newText = tbx.Text;
                var customFilter = new Predicate<object>(o => FilterLogic(o, newText));

                collection.Filter = customFilter;
                DataClientes.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                // var noNull = new Predicate<object>(cliente => {
                //     return cliente != null;
                // });
                // collection.Filter += noNull;
                DataClientes.ItemsSource = collection;
                CollectionView = collection;
                ClearFields();
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
            var cliente = (Cliente)obj;
            return cliente.Nombre.Contains(text) ||
                   cliente.ApPaterno.Contains(text) ||
                   cliente.ApMaterno.Contains(text);
        }

        /// <summary>
        /// Lógica del boton de Guardar Cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarCliente(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha presionado el boton de guardar.");
            try {
                var newClient = new Cliente {
                    Nombre = txtNombreCliente.Text,
                    ApPaterno = txtApPaternoCliente.Text,
                    ApMaterno = txtApMaternoCliente.Text,
                    Domicilio = txtDireccion.Text,
                    Telefono = txtTelefono.Text == "" ? "0" : txtTelefono.Text,
                    Debe = float.Parse(txtDeuda.Text),
                    Comentario = txtComentario.Text
                };
                Log.Debug("Se ha creado el objeto el objeto Cliente con los campos de texto.");

                if (!Validar(newClient)) {
                    Log.Warn("El producto selecionado no ha pasado las validaciones.");
                    return;
                }
                Log.Debug("El producto selecionado ha pasado las validaciones.");

                if (SelectedClient != null && SelectedClient == newClient) {
                    Edit(newClient);
                }
                else {
                    Alta(newClient);
                }
                FillData();
                ClearFields();
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al hacer el proceso de guardar.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la alta de un nuevo cliente.
        /// </summary>
        /// <param name="newClient"></param>
        private static void Alta(Cliente newClient) {
            try {
                Log.Debug("Detectada alta de cliente.");

                var res = QuerysClientes.NewClient(newClient);
                if (res == 0) return;
                var name = char.ToUpper(newClient.Nombre[0]) + newClient.Nombre[1..] + " " + char.ToUpper(newClient.ApPaterno[0]) + newClient.ApPaterno[1..];
                var msg = $"Se ha dado de alta el cliente {name.ToUpper()}.";
                MessageBox.Show(msg, "Cliente Actualizado");
                Log.Debug("Se ha actualizado el cliente exitosamente.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al actualizar el cliente.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la edición de un producto.
        /// </summary>
        /// <param name="newClient"></param>
        private void Edit(Cliente newClient) {
            try {
                Log.Debug("Detectada edición de un cliente.");
                QuerysClientes.UpdateData(newClient);
                newClient.IdCliente = SelectedClient.IdCliente;

                var res = QuerysClientes.UpdateData(newClient);
                if (res == 0) return;

                var msg = $"Se ha actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre.ToUpper()}.";
                MessageBox.Show(msg, "Cliente Actualizado");
                Log.Debug("Se ha editado el cliente exitosamente.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al editar el cliente.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que valida a un cliente.
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        private static bool Validar(Cliente newClient) {
            try {
                var validatorClient = new ClientValidation();
                var resultsClient = validatorClient.Validate(newClient);
                if (!resultsClient.IsValid) {
                    foreach (var error in resultsClient.Errors) {
                        MessageBox.Show(error.ErrorMessage, "Error");
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

        /// <summary>
        /// Lógica boton eliminar cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarCliente(object sender, RoutedEventArgs e) {
            //
            var cliente = SelectedClient;

            if (cliente.Debe > 0) {
                MessageBox.Show("No puede eliminar un cliente cuya deuda no es 0!", "Error");
                return;
            }

            var mensaje =
                $"¿Seguro que quiere eliminar al cliente {cliente.Nombre} {cliente.ApPaterno.ToUpper()} {cliente.ApMaterno.ToUpper()}";

            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) != MessageBoxResult.OK) return;
            QuerysClientes.DelClient(cliente.IdCliente);
            SelectedClient = null;
            ClearFields();
            FillData();
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //TODO: agregarlo al evento de PreviewTextInput de la deuda.
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            // Patrón para floats
            e.Handled = !e.Text.Any(x => Char.IsDigit(x) || '.'.Equals(x));
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            DataClientes.SelectedItem = null;
            ClearFields();
        }

        private void txtUpdateNombre(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtNombreCliente.Text = textBox.Text;
            EnableGuardar();
        }

        private void txtUpdateApPaterno(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtApPaternoCliente.Text = textBox.Text;
            EnableGuardar();
        }

        private void txtUpdateApMaterno(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtApMaternoCliente.Text = textBox.Text;
            EnableGuardar();
        }

        private void txtUpdateTelefono(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtTelefono.Text = textBox.Text;
            EnableGuardar();
        }

        private void txtUpdateDireccion(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtDireccion.Text = textBox.Text;
            EnableGuardar();
        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtComentario.Text = textBox.Text;
            EnableGuardar();
        }

        private void txtUpdateDeuda(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtDeuda.Text = textbox.Text;
            //EnableGuardar();
            Regex r = new Regex(@"^-{0,1}\d+\.{0,1}\d*$"); // This is the main part, can be altered to match any desired form or limitations
            Match m = r.Match(txtDeuda.Text);
            if (m.Success) {
                txtDeuda.Text = textbox.Text;
                //EnableGuardar();
            }
            else {
                txtDeuda.Text = "";
                //Keyboard.Focus(textbox);
            }
        }

        //TODO: CHECAR POR QUE DEUDA LO LEE COMO CADENA VACÍA
        private void EnableGuardar() {
            if (txtNombreCliente.Text.Length >= 3 &&
                txtApPaternoCliente.Text.Length >= 3 &&
                txtApMaternoCliente.Text.Length >= 3) {

                Guardar.IsEnabled = true;
                Guardar.ToolTip = "Guardar Cambios";
            }

            else {
                Guardar.IsEnabled = false;
                Guardar.ToolTip = "Verificar los datos para guardar.\nEl cliente debe de tener al menos Nombre y Apellidos para poder guardar";
            }

        }

        /// <summary>
        /// Metodo para la validacion de solo Letras en el input
        /// </summary>
        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(c => char.IsLetter(c));
        }

        /// <summary>
        /// Validacion de solo letras y numeros para la dirección, así como el numeral.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyLettersAndNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }

        private void Historial_Click(object sender, RoutedEventArgs e) {
            if (SelectedClient is null) {
                MessageBox.Show("Seleccione un cliente para poder ver su historial");
                return;
            }
            AdminViewHistorialCliente historialCliente = new(SelectedClient);
            historialCliente.ShowDialog();

        }
    }
}
