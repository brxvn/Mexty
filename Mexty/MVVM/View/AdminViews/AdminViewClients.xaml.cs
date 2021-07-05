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
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

            log.Info("Iniciado modulo clientes");
            InitializeComponent();
            FillData();

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
            Cliente cliente;
            try {
                cliente = (Cliente) DataClientes.SelectedItem; 
                // BUG: Si no hay nada en la tabla truena al darle clic probablemente pase en las demás pantallas.
            }
            catch (InvalidCastException exception) {
                // TODO agregar al log error
                Console.WriteLine(exception);
                return;
            }
            
            SelectedClient = cliente;
            txtNombreCliente.Text = cliente.Nombre;
            txtApPaternoCliente.Text = cliente.ApPaterno;
            txtApMaternoCliente.Text = cliente.ApMaterno;
            txtTelefono.Text = cliente.Telefono;
            txtDireccion.Text = cliente.Domicilio;
            txtComentario.Text = cliente.Comentario;
            txtDeuda.Text = cliente.Debe.ToString(CultureInfo.InvariantCulture);

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
                var noNull = new Predicate<object>(cliente => {
                    if (cliente == null) return false;
                    return ((Cliente) cliente).Activo == 1;
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
            var cliente = (Cliente) obj;
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
            var newClient = new Cliente {
                Nombre = txtNombreCliente.Text,
                ApPaterno = txtApPaternoCliente.Text,
                ApMaterno = txtApMaternoCliente.Text,
                Domicilio = txtDireccion.Text,
                Telefono = txtTelefono.Text == "" ? "0" : txtTelefono.Text,
                Debe = float.Parse(txtDeuda.Text),
                Comentario = txtComentario.Text
            };

            var validatorClient = new ClientValidation();
            var resultsClient = validatorClient.Validate(newClient);
            if (!resultsClient.IsValid) {
                foreach (var error in resultsClient.Errors) {
                    MessageBox.Show(error.ErrorMessage, "Error");
                }
                return;
            }

            if (SelectedClient != null && SelectedClient == newClient) {
                Database.UpdateData(newClient);
                
                var msg = $"Se ha actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre}.";
                MessageBox.Show(msg, "Cliente Actualizado");
            }
            else {
                MessageBox.Show("entra else");
                var alta = true;
                if (ListaClientes != null) {
                    for (var index = 0; index < ListaClientes.Count; index++) {
                        var cliente = ListaClientes[index];
                        
                        if (newClient != cliente || cliente.Activo != 0) continue;
                        //activamos y actualizamos
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
                if (alta) {
                    // Alta
                    Database.NewClient(newClient);
                    var msg = $"Se ha dado de alta el cliente {newClient.Nombre}.";
                    MessageBox.Show(msg, "Cliente Actualizado");
                }
            }
            FillData();
            ClearFields();
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
            //var regex = new Regex("^[+-]?([0-9]+([.][0-9]*)?|[.][0-9]+)$ ");
            //e.Handled = regex.IsMatch(e.Text);
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
        }


        private void txtUpdateNombre(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtNombreCliente.Text = textBox.Text;
        }

        private void txtUpdateApPaterno(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtApPaternoCliente.Text = textBox.Text;
        }

        private void txtUpdateApMaterno(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtApMaternoCliente.Text = textBox.Text;
        }

        private void txtUpdateTelefono(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtTelefono.Text = textBox.Text;
        }

        private void txtUpdateDireccion(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtDireccion.Text = textBox.Text;
        }

        private void txtUpdateComentario(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtComentario.Text = textBox.Text;
        }
        
        private void txtUpdateDeuda(object sender, TextChangedEventArgs e) {
            TextBox textBox = sender as TextBox;
            txtDeuda.Text = textBox.Text;
        }
    }
}
