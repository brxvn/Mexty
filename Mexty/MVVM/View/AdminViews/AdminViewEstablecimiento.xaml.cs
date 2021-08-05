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
                ClearFields();
                Log.Debug("Se han inicializado los campos del modulo de establecimiento.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo de establecimiento.");
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
        /// Método que llena el data grid de clientes.
        /// </summary>
        private void FillData() {
            var data = Database.GetTablesFromSucursales();
            ListaSucursales = data;
            var collectionView = new ListCollectionView(data) {
                Filter = e => e is Sucursal sucursal && sucursal.Activo != 0
            };
            CollectionView = collectionView;
            DataEstablecimientos.ItemsSource = collectionView;
            Log.Debug("Se ha llenado datagrid de sucursales.");

            var tipos = new[] { "Matriz", "Sucursal" };
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
            Eliminar.ToolTip = "Eliminar registro";
            Guardar.IsEnabled = true;
            SearchBox.Text = "";

            if (DataEstablecimientos.SelectedItem == null) return; // si no hay nada selecionado, bye
            Log.Debug("Se ha selecionado un establecimiento.");
            var sucursal = (Sucursal) DataEstablecimientos.SelectedItem;

            SelectedSucursal = sucursal;
            txtNombreEstablecimiento.Text = sucursal.NombreTienda;
            txtRFC.Text = sucursal.Rfc;
            txtDirección.Text = sucursal.Dirección;
            txtInstagram.Text = sucursal.Instagram;
            txtFacebook.Text = sucursal.Facebook;
            txtTelefono.Text = sucursal.Telefono;
            txtMensaje.Text = sucursal.Mensaje;
            ComboTipo.SelectedItem = sucursal.TipoTienda;
            Log.Debug("Se ha seleccionado el elemento.");
        }

        /// <summary>
        /// Método que limpia los campos de texto.
        /// </summary>
        private void ClearFields() {
            txtNombreEstablecimiento.IsReadOnly = false;
            SearchBox.Text = "";
            txtNombreEstablecimiento.Text = "";
            txtRFC.Text = "";
            txtDirección.Text = "";
            txtInstagram.Text = "";
            txtFacebook.Text = "";
            txtTelefono.Text = "";
            txtMensaje.Text = "";
            ComboTipo.SelectedIndex = 0;
            Log.Debug("Se han limpiado los campos de texto.");
            Eliminar.IsEnabled = false;
            Eliminar.ToolTip = "Seleccionar un registro para eliminar.";
            Guardar.IsEnabled = false;
            EnableGuardar();
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
                var noNull = new Predicate<object>(sucursal =>
                {
                    if (sucursal == null) return false;
                    return ((Sucursal)sucursal).Activo == 1;
                });
                collection.Filter += noNull;
                DataEstablecimientos.ItemsSource = collection;
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
            var sucursal = (Sucursal)obj;
            if (sucursal.NombreTienda.Contains(text) ||
                sucursal.Dirección.Contains(text) ||
                sucursal.Rfc.Contains(text)) {
                    //return cliente.Activo == 1;
                return true;
            }
            return false;
        }

        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            DataEstablecimientos.SelectedItem = null;
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
                    TipoTienda = ComboTipo.SelectedItem.ToString(),
                    Mensaje = txtMensaje.Text
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
            try {
                Log.Debug("Detectada alta de sucursal.");
                
                var res = Database.NewSucursal(newSucursal);
                if (res == 0) return;
                
                var msg = $"Se ha dado de alta la sucursal {newSucursal.NombreTienda}.";
                MessageBox.Show(msg, "Sucursal Actualizada");
                Log.Debug("Alta exitosa de sucursal");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta una nueva sucursal.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la edición de un producto.
        /// </summary>
        /// <param name="newSucursal"></param>
        private void Edit(Sucursal newSucursal) {
            try {
                Log.Debug("Detectada edición de una sucursal.");
                newSucursal.Activo = SelectedSucursal.Activo;
                newSucursal.IdTienda = SelectedSucursal.IdTienda;
                
                var res = Database.UpdateData(newSucursal);
                if (res == 0) return;

                var msg = $"Se ha actualizado la sucursal {newSucursal.IdTienda.ToString()} {newSucursal.NombreTienda}.";
                MessageBox.Show(msg, "Sucursal Actualizada");
                Log.Debug("Edición de sucursal exitosa.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta la sucurzal.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la activación de un producto.
        /// </summary>
        /// <param name="newSucursal"></param>
        /// <param name="alta"></param>
        private void Activar(Sucursal newSucursal, ref bool alta) {
            try {
                for (var index = 0; index < ListaSucursales.Count; index++) {
                    var sucursal = ListaSucursales[index];

                    if (newSucursal != sucursal || sucursal.Activo != 0) continue;
                    
                     Log.Debug("Detectada sucursal equivalente no activa, actualizando y activando.");
                     newSucursal.IdTienda = sucursal.IdTienda;
                     newSucursal.Activo = 1;
                     alta = false;
                     
                     var res = Database.UpdateData(newSucursal);
                     if (res != 0) {
                        var msg =
                            $"Se ha activado y actualizado el cliente {newSucursal.IdTienda.ToString()} {newSucursal.NombreTienda}.";
                        MessageBox.Show(msg, "Cliente Actualizado");
                        Log.Debug("Se ha activado la sucursal de manera exitosa.");
                     }
                     
                     break;
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrrido un error al activar la sucursal.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que valida un cliente.
        /// </summary>
        /// <param name="newSucursal"></param>
        /// <returns></returns>
        private static bool Validar(Sucursal newSucursal) {
            try {
                var validator = new SucursalValidation();
                var results = validator.Validate(newSucursal);
                if (!results.IsValid) {
                    foreach (var error in results.Errors) {
                        MessageBox.Show(error.ErrorMessage, "Error");
                        Log.Warn(error.ErrorMessage);
                    }

                    return false;
                }

                return true;

            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al hacer la validación.");
                Log.Error($"Error: {exception.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lógica detras del boton de eliminar sucursal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarEstablecimiento(object sender, RoutedEventArgs e) {
            Log.Debug("Presionado eliminar establecimiento.");
            var sucursal = SelectedSucursal;
            var mensaje = $"¿Seguro quiere eliminar la sucursal {sucursal.NombreTienda}?";
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) != MessageBoxResult.OK) return;
            sucursal.Activo = 0;
            Database.UpdateData(sucursal);
            Log.Debug("Sucursal eliminada.");
            SelectedSucursal = null;
            ClearFields();
            FillData();
        }

        private void txtUpdateNombre(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtNombreEstablecimiento.Text = textbox.Text;
            EnableGuardar();
        }

        private void txtUpdateTelefono(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtTelefono.Text = textbox.Text;
            EnableGuardar();
        }

        private void txtUpdateInstagram(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtInstagram.Text = textbox.Text;
            EnableGuardar();
        }

        private void txtUpdateFacebook(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtFacebook.Text = textbox.Text;
            EnableGuardar();
        }

        private void txtUpdateDescripcion(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtDirección.Text = textbox.Text;
            EnableGuardar();
        }

        private void txtUpdateMsg(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtMensaje.Text = textbox.Text;
        }
        private void txtUpdateRFC(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            txtRFC.Text = textbox.Text;
            EnableGuardar();
        }

        /// <summary>
        /// Metodo para la validacion de solo Letras y numeros en el input
        /// </summary>
        private void OnlyLetterAndNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }

        /// <summary>
        /// Metodo para la validacion de solo Letras en el input
        /// </summary>
        private void OnlyLettersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(c => char.IsLetter(c));
        }

        /// <summary>
        /// Metodo para la validacion de solo numeros en el input
        /// </summary>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => Char.IsDigit(x) || '.'.Equals(x));
        }

        private void EnableGuardar() {
            if (txtNombreEstablecimiento.Text.Length >= 3) {

                Guardar.IsEnabled = true;
                Guardar.ToolTip = "Guardar Cambios";
            }

            else {
                Guardar.IsEnabled = false;
                Guardar.ToolTip = "Verificar los datos para guardar.\nEl establecimineto al menos debe contar con un nombre.";
            }


        }

    }
}
