using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Lógica de interación de AdminViewUser.xaml
    /// </summary>
    public partial class AdminViewUser : UserControl {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// La vista actual de la tabla de usuarios.
        /// </summary>
        private ListCollectionView CollectionView { get; set; }

        /// <summary>
        /// Lista de los usuarios de la base de datos.
        /// </summary>
        private List<Usuario> UsuariosList { get; set; }

        /// <summary>
        /// El último usuario seleccionado.
        /// </summary>
        private Usuario SelectedUser { get; set; }

        public AdminViewUser() {
            InitializeComponent();
            Log.Info("Iniciado modulo Usuarios");

            try {
                FillDataGrid();
                ClearFields();
                FillRol();
                FillSucursales();
                Log.Debug("Se han inicializado los campos del modulo Usuarios exitosamente.");
            }
            catch (Exception ex) {
                Log.Error("Ha ocurrido un error al inicializar los campos del modulo Usuarios.");
                Log.Error($"Error: {ex.Message}");
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
        /// Método que llena la tabla con los datos de la tabla usuarios.
        /// </summary>
        private void FillDataGrid() {
            var query = Database.GetTablesFromUsuarios();
            UsuariosList = query;
            var collectionView = new ListCollectionView(query) {
                Filter = (e) => e is Usuario emp && emp.Activo != 0 // Solo usuarios activos en la tabla.
            };
            CollectionView = collectionView;
            DataUsuarios.ItemsSource = collectionView;
            Log.Debug("Se ha llenado la data grid de usuarios.");
        }

        /// <summary>
        /// Función que llena el ComboBox de Rol.
        /// </summary>
        private void FillRol() {
            var roles = Database.GetTablesFromRoles();
            foreach (var rol in roles) {
                ComboRol.Items.Add(rol.RolDescription.ToUpper());
            }
            Log.Debug("Se ha llenado el combo box de roles.");
        }

        /// <summary>
        /// Función que llena el Combobox de Sucursales.
        /// </summary>
        private void FillSucursales() {
            var sucursales = Database.GetTablesFromSucursales();
            foreach (var sucursal in sucursales) {
                ComboSucursal.Items.Add(sucursal.NombreTienda.ToUpper());
            }
            Log.Debug("Se ha llenado el combo box de sucursales.");
        }

        /// <summary>
        /// Funcion que obtiene el item selecionado de la <c>datagrid</c>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e) {
            ClearFields();
            nombreUsuario.IsReadOnly = true;
            apPaternoUsuario.IsReadOnly = true;
            apMaternoUsuario.IsReadOnly = true;

            if (DataUsuarios.SelectedItem == null) return; //Check si no es nulo.
            Log.Debug("Se ha seleccionado un usuario de la data grid.");
            var usuario = (Usuario)DataUsuarios.SelectedItem;
            SelectedUser = usuario;
            nombreUsuario.Text = usuario.Nombre.ToUpper();
            apPaternoUsuario.Text = usuario.ApPaterno.ToUpper();
            apMaternoUsuario.Text = usuario.ApMaterno.ToUpper();
            ComboSucursal.SelectedIndex = usuario.IdTienda - 1;
            ComboRol.SelectedIndex = usuario.IdRol - 1;
            TxtDireccion.Text = usuario.Domicilio.ToUpper();
            TxtTelefono.Text = usuario.Telefono; //ojo
            TxtContraseña.Text = usuario.Contraseña;
            Limpiar.IsEnabled = true;
            Eliminar.IsEnabled = true;
            SearchBox.Text = "";
        }

        /// <summary>
        /// Función que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            nombreUsuario.Text = "";
            apPaternoUsuario.Text = "";
            apMaternoUsuario.Text = "";
            ComboSucursal.SelectedIndex = 0;
            TxtDireccion.Text = "";
            TxtContraseña.Text = "";
            TxtTelefono.Text = "";
            ComboRol.SelectedIndex = 0;
            nombreUsuario.IsReadOnly = false;
            apPaternoUsuario.IsReadOnly = false;
            apMaternoUsuario.IsReadOnly = false;
            Eliminar.IsEnabled = false;
            Guardar.IsEnabled = false;
            Log.Debug("Se han limpiado los campos de texto del modulo usuarios.");
        }

        /// <summary>
        /// Lógica para el boton de busqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            TextBox tbx = sender as TextBox;
            var collection = CollectionView;
            if (tbx != null && tbx.Text != "") {
                string newText = tbx.Text;
                var customFilter = new Predicate<object>(o => FilterLogic(o, newText));

                collection.Filter = customFilter;
                DataUsuarios.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                var noNull = new Predicate<object>(empleado =>
                {
                    if (empleado == null) return false;
                    return ((Usuario)empleado).Activo == 1;
                });
                collection.Filter += noNull;
                DataUsuarios.ItemsSource = collection;
                CollectionView = collection;
                ClearFields();
            }
        }

        /// <summary>
        /// Lógica para el filtro del datagrid.
        /// </summary>
        /// <param name="obj">Objeto en el que se busca.</param>
        /// <param name="text">Texto del cuadro de búsqueda.</param>
        /// <returns></returns>
        private static bool FilterLogic(object obj, string text) {
            var usuario = (Usuario)obj;
            if (usuario.Username.Contains(text) ||
                usuario.ApPaterno.Contains(text) ||
                usuario.SucursalNombre.Contains(text) ||
                usuario.Nombre.Contains(text)) {
                return usuario.Activo == 1;
            }
            return false;
        }

        /// <summary>
        /// Lógica detras del boton de registrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarUsuario(object sender, RoutedEventArgs e) {
            Log.Debug("Precionado boton guardar.");
            try {
                var newUsuario = new Usuario {
                    Nombre = nombreUsuario.Text,
                    ApPaterno = apPaternoUsuario.Text,
                    ApMaterno = apMaternoUsuario.Text,
                    Domicilio = TxtDireccion.Text,
                    Telefono = TxtTelefono.Text.Equals("") ? "0" : TxtTelefono.Text,
                    Contraseña = TxtContraseña.Text,
                    IdTienda = ComboSucursal.SelectedIndex + 1,
                    IdRol = ComboRol.SelectedIndex + 1
                };
                Log.Debug("Se ha creado el objeto Usuario exitosamente.");

                if (!Validar(newUsuario)) {
                    Log.Warn("El objeto creado tipo Usuario no ha pasado las validaciones.");
                    return;
                }
                Log.Debug("El objeto creado tipo Usuario ha pasado las validaciones.");

                if (SelectedUser != null && SelectedUser == newUsuario) {
                    Edit(newUsuario);
                }
                else {
                    var flag = true;
                    if (UsuariosList != null) {
                        Activar(newUsuario, ref flag);
                    }
                    if (flag) {
                        Alta(newUsuario);
                    }
                }
                FillDataGrid();
                ClearFields();
                DesactivarBotones();
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error a la hora de hacer el proceso de guardar.");
                Log.Error($"Error: {exception.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la alta de un usuario.
        /// </summary>
        /// <param name="newUsuario"></param>
        private static void Alta(Usuario newUsuario) {
            newUsuario.Activo = 1;
            newUsuario.Username = Usuario.GenUsername(newUsuario); // Generamos el usename si el usuario es nuevo.
            Log.Debug("Detectado nuevo usuario, dando de alta.");
            Database.NewUser(newUsuario);
            var msg = $"Se ha creado el usuario {newUsuario.Username}.";
            MessageBox.Show(msg, "Nuevo Usuario registrado.");
        }

        /// <summary>
        /// Método que se encarga de la edición de un usuario.
        /// </summary>
        private void Edit(Usuario newUsuario) {
            Log.Debug("Detectada edición de usuario.");
            newUsuario -= SelectedUser;

            Database.UpdateData(newUsuario);

            var msg = $"Se ha actualizado el usuario: {SelectedUser.Username}.";
            MessageBox.Show(msg, "Usuario Actualizado");
        }

        /// <summary>
        /// Método que se encarga de la activación de un usuario.
        /// </summary>
        /// <param name="newUsuario"> El usuario a activar.</param>
        /// <param name="flag">Bandera para señalizar si es necesario dar de alta un nuevo usuario.</param>
        private void Activar(Usuario newUsuario, ref bool flag) {
            for (var index = 0; index < UsuariosList.Count; index++) {
                var usuario = UsuariosList[index];
                if (usuario != newUsuario || usuario.Activo != 0) continue;
                Log.Debug("Detectado usuario equivalente no activo, activando y actualizando.");
                newUsuario += usuario;
                newUsuario.Activo = 1;
                Database.UpdateData(newUsuario);
                var msg =
                    $"Se ha activado el usuario {newUsuario.Nombre} {newUsuario.ApPaterno} {newUsuario.ApMaterno}.";
                MessageBox.Show(msg, "Nuevo Usuario registrado.");
                flag = false;
                break;
            }
        }

        /// <summary>
        /// Método que valida a un objeto tipo usuario.
        /// </summary>
        /// <param name="newUsuario"></param>
        /// <returns></returns>
        private static bool Validar(Usuario newUsuario) {
            try {
                var validator = new UserValidation();
                var results = validator.Validate(newUsuario);
                if (!results.IsValid) {
                    //Guardar.IsEnabled = false;
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

        /// <summary>
        /// Elimina (hace inactivo) el usuario seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarUsuario(object sender, RoutedEventArgs e) {
            Log.Debug("Precionado boton eliminar usuario.");
            var usuario = SelectedUser;
            var mensaje = $"¿Seguro quiere eliminar el usuario:  {usuario.Username} ?";
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Eliminar", buttons, icon) != MessageBoxResult.OK) return;
            usuario.Activo = 0;
            try {
                Database.UpdateData(usuario);
                Log.Info("Usuario eliminado.");
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al eliminar el usuario.");
                Log.Error($"Error: {exception.Message}");
            }
            FillDataGrid();
            ClearFields();
            Eliminar.IsEnabled = false;
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            Log.Debug("Precionado boton limpiar en modulo usuario.");
            ClearFields();
            DesactivarBotones();
        }

        // -- Eventos de TextUpdate.

        private void TextUpdatePswd(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtContraseña.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdateUserName(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            nombreUsuario.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdateApMa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apMaternoUsuario.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdateApPa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apPaternoUsuario.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdateDir(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtDireccion.Text = textbox.Text;
            EnableGuardar();
        }

        private void TextUpdateTel(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtTelefono.Text = textbox.Text;
            EnableGuardar();
        }

        private void DesactivarBotones() {
            Log.Debug("Botones desactivados en modulo usuarios.");
            Guardar.IsEnabled = false;
            Eliminar.IsEnabled = false;
        }


        /// <summary>
        /// Metodo que solamente activa el boton de guardar una vex que todos los cambppos de texto estan completos
        /// </summary>
        private void EnableGuardar() {
            if (nombreUsuario.Text.Length > 2 && 
                apPaternoUsuario.Text.Length > 2 && 
                apMaternoUsuario.Text.Length > 2 && 
                TxtDireccion.Text.Length > 2 && 
                TxtTelefono.Text.Length == 10 && 
                TxtContraseña.Text.Length > 3) {
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
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x) || '#'.Equals(x));
        }

        private void OnlyuLetterNumbersSB(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }
    }
}