using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using Mexty.MVVM.Model.Validations;
using Mexty.Theme;
using Color = System.Windows.Media.Color;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Lógica de interación de AdminViewUser.xaml
    /// </summary>
    public partial class AdminViewUser : UserControl {

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
            
            FillDataGrid();
            ClearFields();
            FillRol();
            FillSucursales();
           
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
        }

        /// <summary>
        /// Función que llena el ComboBox de Rol.
        /// </summary>
        private void FillRol() {
            var roles = Database.GetTablesFromRoles();
            foreach (var rol in roles) {
                ComboRol.Items.Add(rol.RolDescription.ToLower());
            }
        }

        /// <summary>
        /// Función que llena el Combobox de Sucursales.
        /// </summary>
        private void FillSucursales() {
            var sucursales = Database.GetTablesFromSucursales();
            foreach (var sucursal in sucursales) {
                ComboSucursal.Items.Add(sucursal.NombreTienda);
            }
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
            var usuario = (Usuario) DataUsuarios.SelectedItem;
            SelectedUser = usuario;
            nombreUsuario.Text = usuario.Nombre;
            apPaternoUsuario.Text = usuario.ApPaterno;
            apMaternoUsuario.Text = usuario.ApMaterno;
            ComboSucursal.SelectedIndex = usuario.IdTienda - 1;
            ComboRol.SelectedIndex = usuario.IdRol - 1;
            TxtDireccion.Text = usuario.Domicilio;
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
            ComboRol.SelectedIndex = 0 ;
            nombreUsuario.IsReadOnly = false;
            apPaternoUsuario.IsReadOnly = false;
            apMaternoUsuario.IsReadOnly = false;
            Eliminar.IsEnabled = false;
            Guardar.IsEnabled = false;
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
                var noNull = new Predicate<object>(empleado => {
                    if (empleado == null) return false;
                    return ((Usuario) empleado).Activo == 1;
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
            var usuario = (Usuario) obj;
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
            
            var validator = new UserValidation();
            var results = validator.Validate(newUsuario);
            if (!results.IsValid) {
                //Guardar.IsEnabled = false;
                foreach (var error in results.Errors) {
                    MessageBox.Show(error.ErrorMessage);
                }
                return;
            }

            if (SelectedUser != null && SelectedUser == newUsuario) {
                newUsuario -= SelectedUser;
                
                Database.UpdateData(newUsuario);
                
                var msg = $"Se ha actualizado el usuario: {SelectedUser.Username}.";
                MessageBox.Show(msg, "Usuario Actualizado");
            }
            else {
                var flag = true;
                foreach (var usuario in UsuariosList) {
                    if (usuario != newUsuario || usuario.Activo != 0) continue;
                    newUsuario += usuario;
                    newUsuario.Activo = 1;
                    Database.UpdateData(newUsuario);
                    var msg = $"Se ha activado el usuario {newUsuario.Nombre} {newUsuario.ApPaterno} {newUsuario.ApMaterno}.";
                    MessageBox.Show(msg, "Nuevo Usuario registrado.");
                    flag = false;
                }
                if (flag) {
                    newUsuario.Activo = 1;
                    newUsuario.Username = Usuario.GenUsername(newUsuario); // Generamos el usename si el usuario es nuevo.
                    Database.NewUser(newUsuario);
                    var msg = $"Se ha creado el usuario {newUsuario.Username}.";
                    MessageBox.Show(msg, "Nuevo Usuario registrado.");
                }
            }
            
            FillDataGrid();
            ClearFields();
            DesactivarBotones();

        }
        

        /// <summary>
        /// Elimina (hace inactivo) el usuario seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarUsuario(object sender, RoutedEventArgs e) {
            var usuario = SelectedUser;
            var mensaje = "¿Seguro quiere eliminar el usuario: " + usuario.Username +"?";
            const MessageBoxButton buttons = MessageBoxButton.OKCancel;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Eliminar", buttons, icon) != MessageBoxResult.OK) return;
            usuario.Activo = 0;
            Database.UpdateData(usuario);
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
        /// Regresa la cadena dada en Mayusculas y sin espeacios.
        /// </summary>
        /// <param name="text">Texto a Preparar.</param>
        /// <returns></returns>
        private static string StrPrep(string text) {
            return text.ToUpper().Replace(" ", "");
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarCampos(object sender, RoutedEventArgs e) {
            ClearFields();
            DesactivarBotones();
        }

        // -- Eventos de TextUpdate.

        private void TextUpdatePswd(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtContraseña.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdateUserName(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            nombreUsuario.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdateApMa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apMaternoUsuario.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdateApPa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apPaternoUsuario.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        private void TextUpdateDir(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtDireccion.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }
    

        private void TextUpdateTel(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtTelefono.Text = textbox.Text;
            Guardar.IsEnabled = textbox.Text != "";
        }

        //private void PhoneValidation(object sender, RoutedEventArgs e) {
        //    TextBox textbox = sender as TextBox;
        //    if (textbox.Text.Length < 10) {
        //        //MessageBox.Show("El número de teléfono debe de tener 10 dígitos.");
        //    }
        //}

        //private void PwdValidation(object sender, RoutedEventArgs e) {
        //    TextBox textbox = sender as TextBox;
        //    if (textbox.Text.Equals("")) {
        //        //MessageBox.Show("La contraseña debe de tener al menos 8 carácteres.");
        //    }
        //}

        private void DesactivarBotones() {
            Guardar.IsEnabled = false;
            Eliminar.IsEnabled = false;
        }

    }
}