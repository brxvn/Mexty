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
                Log.Debug("Se han inicializado los campos del modulo de clientes.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo de clientes.");
                Log.Error($"Error: {e.Message}");
            }
            ClearFields();
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
            var dataClientes = Database.GetTablesFromClientes();
            ListaClientes = dataClientes;
            var collectionView = new ListCollectionView(dataClientes) {
                Filter = e => e is Cliente cliente && cliente.Activo != 0
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
            Eliminar.IsEnabled = true;

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
        }

        /// <summary>
        /// Método que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            txtNombreCliente.Text = "";
            txtApPaternoCliente.Text = "";
            txtApMaternoCliente.Text = "";
            txtTelefono.Text = "";
            txtDireccion.Text = "";
            txtComentario.Text = "";
            txtDeuda.Text = "";

            txtNombreCliente.IsReadOnly = false;
            txtApPaternoCliente.IsReadOnly = false;
            txtApMaternoCliente.IsReadOnly = false;
            Eliminar.IsEnabled = false;
            Limpiar.IsEnabled = true;
            Guardar.IsEnabled = false;
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
                var noNull = new Predicate<object>(cliente =>
                {
                    if (cliente == null) return false;
                    return ((Cliente)cliente).Activo == 1;
                });
                collection.Filter += noNull;
                DataClientes.ItemsSource = collection;
                CollectionView = collection;
            }
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
            if (cliente.Nombre.Contains(text) ||
                cliente.ApPaterno.Contains(text) ||
                cliente.ApMaterno.Contains(text)) {
                return cliente.Activo == 1;
            }
            return false;
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
                    var alta = true;
                    if (ListaClientes != null) {
                        Activar(newClient, ref alta);
                    }

                    if (alta) {
                        Alta(newClient);
                    }
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
            Log.Debug("Detectada alta de cliente.");
            Database.NewClient(newClient);
            var msg = $"Se ha dado de alta el cliente {newClient.Nombre}.";
            MessageBox.Show(msg, "Cliente Actualizado");
        }

        /// <summary>
        /// Método que se encarga de la edición de un producto.
        /// </summary>
        /// <param name="newClient"></param>
        private void Edit(Cliente newClient) {
            Log.Debug("Detectada edición de un cliente.");
            Database.UpdateData(newClient);

            var msg = $"Se ha actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre}.";
            MessageBox.Show(msg, "Cliente Actualizado");
        }

        /// <summary>
        /// Método que se encarga de la activación de un producto.
        /// </summary>
        /// <param name="newClient"></param>
        private void Activar(Cliente newClient, ref bool alta) {
            for (var index = 0; index < ListaClientes.Count; index++) {
                var cliente = ListaClientes[index];

                if (newClient != cliente || cliente.Activo != 0) continue;
                Log.Debug("Detectado cliente equivalente no activo, actualizando y activando.");
                newClient.IdCliente = cliente.IdCliente;
                newClient.Activo = 1;
                Database.UpdateData(newClient);
                alta = false;
                var msg =
                    $"Se ha activado y actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre}.";
                MessageBox.Show(msg, "Cliente Actualizado");
                break;
            }
        }

        /// <summary>
        /// Método que valida a un cliente.
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        private static bool Validar(Cliente newClient) {
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

        /// <summary>
        /// Lógica boton eliminar cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarCliente(object sender, RoutedEventArgs e) {
            var cliente = SelectedClient;
            var mensaje =
                $"¿Seguro que quiere eliminar al cliente {cliente.Nombre} {cliente.ApPaterno} {cliente.ApMaterno}";

            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) != MessageBoxResult.OK) return;
            cliente.Activo = 0;
            Database.UpdateData(cliente);
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
                EnableGuardar();
            }
            else {
                txtDeuda.Text = "";
                Keyboard.Focus(textbox);
            }
        }

        public void EnableGuardar() {
            if (txtNombreCliente.Text.Length > 3 &&
                txtApPaternoCliente.Text.Length > 3 &&
                txtApMaternoCliente.Text.Length > 3 &&
                txtTelefono.Text.Length == 10 &&
                txtDireccion.Text.Length > 3 &&
                txtComentario.Text.Length > 3 &&
                txtDeuda.Text != "") {
                
                Guardar.IsEnabled = true;
            }

            else Guardar.IsEnabled = false;
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
    }
}
