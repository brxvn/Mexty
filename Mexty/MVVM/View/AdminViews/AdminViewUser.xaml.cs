using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using iText.Layout.Element;
using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
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
        /// Lista de sucusales de la bd
        /// </summary>
        private List<Sucursal> ListaSucursales { get; set; }

        /// <summary>
        /// El último usuario seleccionado.
        /// </summary>
        private Usuario SelectedUser { get; set; }

        public AdminViewUser() {
            InitializeComponent();
            Log.Info("Iniciado modulo Usuarios");

            try {
                FillDataGrid();
                FillRol();
                FillSucursales();
                ClearFields();
                pswrdUsuario.Background = Brushes.Beige;
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
            var query = QuerysUsuario.GetTablesFromUsuarios();
            UsuariosList = query;

            var collectionView = new ListCollectionView(query) {
                Filter = (e) => e is Usuario emp //&& emp.Activo != 0 // Solo usuarios activos en la tabla.
            };
            CollectionView = collectionView;
            DataUsuarios.ItemsSource = collectionView;
            Log.Debug("Se ha llenado la data grid de usuarios.");
        }

        /// <summary>
        /// Función que llena el ComboBox de Rol.
        /// </summary>
        private void FillRol() {
            var roles = QuerysRol.GetTablesFromRoles();
            foreach (var rol in roles) {
                ComboRol.Items.Add(rol.RolDescription.ToUpper());
            }
            Log.Debug("Se ha llenado el combo box de roles.");
        }

        /// <summary>
        /// Función que llena el Combobox de Sucursales.
        /// </summary>
        private void FillSucursales() {
            var sucursales = QuerysSucursales.GetTablesFromSucursales();
            ComboSucursal.Items.Add($"{0.ToString()} General");
            foreach (var sucursal in sucursales) {
                ComboSucursal.Items.Add($"{sucursal.IdTienda.ToString()} {sucursal.NombreTienda.ToUpper()}");
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
            if (usuario.IdTienda == 0) {
                ComboSucursal.SelectedIndex = 0;
            }
            else {
                var suc = ComboSucursal.Items;
                for (var index = 0; index < suc.Count; index++) {
                    var sucursal = suc[index];

                    if (sucursal.ToString().Split(' ')[0].Contains(usuario.IdTienda.ToString())) {
                        ComboSucursal.SelectedIndex = index;
                    }
                }
            }
            ComboRol.SelectedIndex = usuario.IdRol - 1;
            TxtDireccion.Text = usuario.Domicilio.ToUpper();
            TxtTelefono.Text = usuario.Telefono;
            pswrdUsuario.Password = usuario.Contraseña;
            Limpiar.IsEnabled = true;
            Eliminar.IsEnabled = true;
            Eliminar.ToolTip = "Eliminar Registro";
            
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
            pswrdUsuario.Password = "";
            TxtTelefono.Text = "";
            SearchBox.Text = "";
            ComboRol.SelectedIndex = 0;
            nombreUsuario.IsReadOnly = false;
            apPaternoUsuario.IsReadOnly = false;
            apMaternoUsuario.IsReadOnly = false;
            Eliminar.IsEnabled = false;
            Eliminar.ToolTip = "Seleccionar un registro para eliminar.";
            Guardar.IsEnabled = false;
            EnableGuardar();
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
                DataUsuarios.ItemsSource = collection;
                CollectionView = collection;
                ClearFields();
            }

            SearchBox.Text = tbx.Text;

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
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lógica detras del boton de registrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarUsuario(object sender, RoutedEventArgs e) {
            Log.Debug("Presionado boton guardar.");
            try {
                var newUsuario = new Usuario {
                    Nombre = nombreUsuario.Text,
                    ApPaterno = apPaternoUsuario.Text,
                    ApMaterno = apMaternoUsuario.Text,
                    Domicilio = TxtDireccion.Text,
                    Telefono = TxtTelefono.Text.Equals("") ? "0" : TxtTelefono.Text,
                    Contraseña = pswrdUsuario.Password,
                    IdTienda = int.Parse(ComboSucursal.Text.Split(' ')[0]),
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
                    Alta(newUsuario);
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
            try {
                newUsuario.Username = Usuario.GenUsername(newUsuario); // Generamos el usename si el usuario es nuevo.
                Log.Debug("Detectado nuevo usuario, dando de alta.");
                
                var res = QuerysUsuario.NewUser(newUsuario);
                if (res == 0) return;
                
                var msg = $"Se ha creado el usuario {newUsuario.Username}.";
                MessageBox.Show(msg, "Nuevo Usuario registrado.");
                Log.Debug("Se ha dado de alta un nuevo usuario de manera exitosa.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo usuario.");
                Log.Error($"Error: {e.Message}");
            }
        }

        /// <summary>
        /// Método que se encarga de la edición de un usuario.
        /// </summary>
        private void Edit(Usuario newUsuario) {
            try {
                Log.Debug("Detectada edición de usuario.");
                newUsuario -= SelectedUser;

                var res = QuerysUsuario.UpdateData(newUsuario);
                if (res == 0) return;
                
                var msg = $"Se ha actualizado el usuario: {SelectedUser.Username.ToUpper()}.";
                MessageBox.Show(msg, "Usuario Actualizado");
                Log.Debug("Se ha editado el usuario de manera exitosa.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al editar el usuario.");
                Log.Error($"Error: {e.Message}");
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

            try {
                QuerysUsuario.DelProduct(usuario.Id);
                Log.Info("Usuario eliminado.");
            }
            catch (Exception exception) {
                Log.Error("Ha ocurrido un error al eliminar el usuario.");
                Log.Error($"Error: {exception.Message}");
            }
            SelectedUser = null;
            FillDataGrid();
            ClearFields();
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => Char.IsDigit(x) || '.'.Equals(x));
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            Log.Debug("Precionado boton limpiar en modulo usuario.");
            DataUsuarios.SelectedItem = null;
            ClearFields();
            DesactivarBotones();
        }

        // -- Eventos de TextUpdate.

        private void TextUpdatePswd(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            //TxtContraseña.Text = textbox.Text;
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
                apMaternoUsuario.Text.Length > 2 ) {

                Guardar.IsEnabled = true;
                Guardar.ToolTip = "Guardar cambios";
            }
            else {
                Guardar.IsEnabled = false;
                Guardar.ToolTip = "Verificar los datos para guardar.\nTodos los usuarios deben de tener al menos Nombre y Apellidos.";
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
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x) || '#'.Equals(x) || '/'.Equals(x));
        }

        private void OnlyuLetterNumbersSB(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.Any(x => char.IsLetterOrDigit(x));
        }


        private void passwordChanged(object sender, RoutedEventArgs e) {
            if (pswrdUsuario.Password.Length == 0)
                pswrdUsuario.Background.Opacity = 1;
            else
                pswrdUsuario.Background.Opacity = 0;
        }

        private void hidePswrd(object sender, DependencyPropertyChangedEventArgs e) {
            if (pswrdUsuario.Password.Length == 0)
                pswrdUsuario.Background.Opacity = 0;
        }

        private void showPswrd(object sender, RoutedEventArgs e) {
            if (pswrdUsuario.Password.Length == 0)
                pswrdUsuario.Background.Opacity = 1;
        }

        private void EnterKeyPassword(object sender, KeyEventArgs e) {

        }
    }
}