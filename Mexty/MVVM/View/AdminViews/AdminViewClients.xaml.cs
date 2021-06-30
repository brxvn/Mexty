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
using FluentValidation;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;
using MySqlX.XDevAPI;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewClients.xaml
    /// </summary>
    public partial class AdminViewClients : UserControl {

        /// <summary>
        /// Lista de productos dada por la base de datos.
        /// </summary>
        private List<Cliente> ListaClientes { get; set; }

        /// <summary>
        /// Lista de deudas dada por la base de datos.
        /// </summary>
        private List<Deuda> ListaDeudas { get; set; }

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// El último cliente seleccionado de la datagrid.
        /// </summary>
        private Cliente SelectedClient { get; set; }

        public AdminViewClients() {

            InitializeComponent();

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

        // TODO: agregar eventos y llenarlos
        // TODO: En la historia de usuario dice que debe de haber una sección de comentarios para cada cliente, pero no está en la bd.

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

            var dataDeudas = Database.GetTablesFromDeudas();
            ListaDeudas = dataDeudas;
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

            if (DataClientes.SelectedItem == null) return;
            var cliente = (Cliente) DataClientes.SelectedItem;
            SelectedClient = cliente;
            txtNombreCliente.Text = cliente.Nombre;
            txtApPaternoCliente.Text = cliente.ApPaterno;
            txtApMaternoCliente.Text = cliente.ApMaterno;
            txtTelefono.Text = cliente.Telefono.ToString();
            txtDireccion.Text = cliente.Domicilio;
            //txtComentario.Text = cliente.
            //Eliminar.IsEnabled = true;

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
            //txtComentario.Text = cliente.
            
            txtNombreCliente.IsReadOnly = false;
            txtApPaternoCliente.IsReadOnly = false;
            txtApMaternoCliente.IsReadOnly = false;
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
            var newClient = new Cliente();
            var newDeuda = new Deuda();
            newClient.Nombre = txtNombreCliente.Text;
            newClient.ApPaterno = txtApPaternoCliente.Text;
            newClient.ApMaterno = txtApMaternoCliente.Text;
            newClient.Domicilio = txtDireccion.Text;
            //TODO: falta comentario.
            newClient.Telefono = txtTelefono.Text == "" ? 0 : int.Parse(txtTelefono.Text);
            //TODO: falta campo de deuda.
            //

            // TODO: También validar el objeto deuda.
            var validatorClient = new ClientValidation();
            var resultsClient = validatorClient.Validate(newClient);
            var validatorDeuda = new DeudaValidation();
            var resultsDeuda = validatorDeuda.Validate(newDeuda);
            if (!resultsClient.IsValid || !resultsDeuda.IsValid) {
                // No es valido
                return;
            }

            // TODO: armar operador de igual.
            if (SelectedClient != null && SelectedClient == newClient) {
                // Update
                
                Database.UpdateData(newClient);
                
                var msg = $"Se ha actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre}.";
                MessageBox.Show(msg, "Cliente Actualizado");
            }
            else {
                var alta = true;
                foreach (var cliente in ListaClientes) {
                    if (newClient == cliente && cliente.Activo == 0) {
                        //activamos y actualizamos
                        
                        Database.UpdateData(newClient);
                        alta = false;
                        var msg = $"Se ha activado y actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre}.";
                        MessageBox.Show(msg, "Cliente Actualizado");
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
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
    }
}
