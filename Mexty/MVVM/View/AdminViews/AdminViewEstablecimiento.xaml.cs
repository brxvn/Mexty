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
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewEstablecimiento.xaml
    /// </summary>
    public partial class AdminViewEstablecimiento : UserControl {
        private static readonly ILog Log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Lista de Sucursales de la BD.
        /// </summary>
        private List<Sucursal> ListaSucursales { get; set; }

        /// <summary>
        /// Collection view actual de la datagrid.
        /// </summary>
        private CollectionView CollectionView { get; set; }

        /// <summary>
        /// La última Sucursal seleccionado de la datagrid.
        /// </summary>
        private Sucursal SelectedSucursal { get; set; }

        public AdminViewEstablecimiento() {
            Log.Info("Iniciado el modulo de Establecimiento.");

            try {
                InitializeComponent();
                FillData();

                Log.Debug("Se han inicializado los campos del modulo de establecimiento.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo de establecimiento.");
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
        /// Método que llena el data grid de clientes.
        /// </summary>
        private void FillData() {
            var data = Database.GetTablesFromSucursales();
            ListaSucursales = data;
            // TODO: preguntar si va a haber sucursales inacitvas o activas.
            // var collectionView = new ListCollectionView(data) {
            //     Filter = e => e is Sucursal sucursal && sucursal.Activo != 0
            // };
            // CollectionView = collectionView;
            // DataEstablecimientos.ItemsSource = collectionView;
            DataEstablecimientos.ItemsSource = data;
            Log.Debug("Se ha llenado datagrid de sucursales.");
            
            var tipos = new[] {"Matriz", "Sucursal"};
            ComboTipo.ItemsSource = tipos;
            Log.Debug("Se ha llenado el campo de tipo de sucursal.");
        }

        /// <summary>
        /// Lógica para el evento SelectedItem.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, SelectionChangedEventArgs e) {
            ClearFields();
            txtNombreEstablecimiento.IsReadOnly = true;
            Eliminar.IsEnabled = true;
            
            if (DataEstablecimientos.SelectedItem == null) return; // si no hay nada selecionado, bye
            Log.Debug("Se ha selecionado un establecimiento.");
            var sucursal = (Sucursal) DataEstablecimientos.SelectedItem;

            sucursal.NombreTienda = txtNombreEstablecimiento.Text;
            sucursal.Rfc = txtRFC.Text;
            sucursal.Dirección = txtDirección.Text;
            sucursal.Instagram = txtInstagram.Text;
            sucursal.Facebook = txtFacebook.Text;
            sucursal.Telefono = txtTelefono.Text;
            sucursal.TipoTienda = ComboTipo.SelectedItem.ToString();

        }

        /// <summary>
        /// Método que limpia los campos de texto.
        /// </summary>
        private void ClearFields() {
            txtNombreEstablecimiento.Text = "";
            txtRFC.Text = "";
            txtDirección.Text = "";
            txtInstagram.Text = "";
            txtFacebook.Text = "";
            txtTelefono.Text = "";
            ComboTipo.SelectedIndex = 0;
            Log.Debug("Se han limpiado los campos de texto.");

            Eliminar.IsEnabled = false;
        }

        /// <summary>
        /// Lógica para el campo de busqueda.
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
                DataEstablecimientos.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                var noNull = new Predicate<object>(cliente => {
                    if (cliente == null) return false;
                    return ((Cliente) cliente).Activo == 1;
                });
                collection.Filter += noNull;
                DataEstablecimientos.ItemsSource = collection;
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
            var sucursal = (Sucursal) obj;
            if (sucursal.NombreTienda.Contains(text) ||
                sucursal.Dirección.Contains(text) ||
                sucursal.Rfc.Contains(text)) {
                //return cliente.Activo == 1;
                return true;
            }
            return false;
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
        }

        /// <summary>
        /// Lógica para el boton de guardar establecimiento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuardarEstablecimiento(object sender, RoutedEventArgs e) {
            Log.Debug("Se ha precionado el boton de guardar.");
            try {
                var newSucursal = new Sucursal {
                    NombreTienda = txtNombreEstablecimiento.Text,
                    Dirección = txtDirección.Text,
                    Telefono = txtTelefono.Text,
                    Rfc = txtRFC.Text,
                    Facebook = txtFacebook.Text,
                    Instagram = txtInstagram.Text,
                    TipoTienda = ComboTipo.SelectedItem.ToString()
                };
                Log.Debug("Se ha creado el objeto sucursal con los campos te de texto.");

                if (!Validar(newSucursal)) {
                    Log.Warn("la sucursal selecionada no ha pasado las validaciones.");
                    return;
                }
                Log.Debug("la sucursal selecionada ha pasado las validaciones.");

                if (SelectedSucursal != null && SelectedSucursal == newSucursal) {
                    Edit(newSucursal);
                }
                else {
                    var alta = true;
                    if (ListaSucursales != null) {
                        Activar(newSucursal, ref alta);
                    }

                    if (alta) {
                        Alta(newSucursal);
                    }
                }
                FillData();
                ClearFields();
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrdo un error al hacer el proceso de guardar.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la alta de un nuevo cliente.
        /// </summary>
        /// <param name="newSucursal"></param>
        private static void Alta(Sucursal newSucursal) {
            Log.Debug("Detectada alta de sucursal.");
            Database.NewSucursal(newSucursal);
            var msg = $"Se ha dado de alta la sucursal {newSucursal.NombreTienda}.";
            MessageBox.Show(msg, "Sucursal Actualizada");
        }

        /// <summary>
        /// Método que se encarga de la edición de un producto.
        /// </summary>
        /// <param name="newSucursal"></param>
        private void Edit(Sucursal newSucursal) {
            Log.Debug("Detectada edición de una sucursal.");
            Database.UpdateData(newSucursal);
            
            var msg = $"Se ha actualizado la sucursal {newSucursal.IdTienda.ToString()} {newSucursal.NombreTienda}.";
            MessageBox.Show(msg, "Sucursal Actualizada");
            
        }

        /// <summary>
        /// Método que se encarga de la activación de un producto.
        /// </summary>
        /// <param name="newSucursal"></param>
        /// <param name="alta"></param>
        private void Activar(Sucursal newSucursal, ref bool alta) {
            for (var index = 0; index < ListaSucursales.Count; index++) {
                var sucursal = ListaSucursales[index];
                
                //TODO: ver que onda con los activos y definir el operador == en sucursal.
                //if (newSucursal != cliente || cliente.Activo != 0) continue;
                // Log.Debug("Detectado cliente equivalente no activo, actualizando y activando.");
                // newClient.IdCliente = cliente.IdCliente;
                // newClient.Activo = 1;
                // Database.UpdateData(newClient);
                // alta = false;
                // var msg =
                //     $"Se ha activado y actualizado el cliente {newClient.IdCliente.ToString()} {newClient.Nombre}.";
                // MessageBox.Show(msg, "Cliente Actualizado");
                // break;
            }
        }

        /// <summary>
        /// Método que valida un cliente.
        /// </summary>
        /// <param name="newSucursal"></param>
        /// <returns></returns>
        private static bool Validar(Sucursal newSucursal) {
            var validator = new SucursalValidation();
            var results = validator.Validate(newSucursal);
            if (!results.IsValid) {
                foreach (var error in results.Errors) {
                    // MessageBox.Show(error.ErrorMessage, "Error");
                    // Log.Warn(error.ErrorMessage);
                }

                return false;
            }

            return true;
        }

        private void EliminarEstablecimiento(object sender, RoutedEventArgs e) {

        }

        private void txtUpdateNombre(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateTelefono(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateInstagram(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateFacebook(object sender, TextChangedEventArgs e) {

        }

        private void txtUpdateDescripcion(object sender, TextChangedEventArgs e) {

        }

        private void ComboTipo_SelectionChanged()
        {

        }
    }
}
