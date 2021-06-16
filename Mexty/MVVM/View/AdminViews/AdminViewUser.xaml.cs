using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using Mexty.Theme;

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
            FillRol();
            FillSucursales();
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
        /// Método que llena la tabla con los datos de la tabla usuarios.
        /// </summary>
        private void FillDataGrid() {
            var connObj = new Database();
            var query = connObj.GetTablesFromUsuarios();
            UsuariosList = query;
            // TODO: Armar una lista de objetos con las sucursales y usarlas como source para el combobox de las sucursales.
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
            var data = new Database();
            var roles = data.GetTablesFromRoles();
            foreach (var rol in roles) {
                ComboRol.Items.Add(rol.RolDescription.ToLower());
            }
        }

        /// <summary>
        /// Función que llena el Combobox de Sucursales.
        /// </summary>
        private void FillSucursales() {
            var data = new Database();
            var sucursales = data.GetTablesFromSucursales();
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
            var usuario = (Usuario) DataUsuarios.SelectedItem;
            if (usuario == null) return; // Check si no es nulo.
            SelectedUser = usuario;
            nombreUsuario.Text = usuario.Nombre;
            apPaternoUsuario.Text = usuario.ApPaterno;
            apMaternoUsuario.Text = usuario.ApMaterno;
            ComboSucursal.SelectedIndex = usuario.IdTienda - 1;
            ComboRol.SelectedIndex = usuario.IdRol - 1;
            TxtDireccion.Text = usuario.Domicilio;
            TxtTelefono.Text = usuario.Telefono.ToString(); //ojo
            TxtContraseña.Text = usuario.Contraseña;
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
                var noNull = new Predicate<object>(empleado => {
                    if (empleado == null) return false;
                    return ((Usuario) empleado).Activo == 1;
                });
                var usuarios = new Predicate<object>(empleado => {
                    if (empleado == null) return false;
                    return (
                        FuzzySearch.FuzzyMatch(((Usuario) empleado).Username, newText));
                });
                var nombre = new Predicate<object>(empleado => {
                    if (empleado == null) return false;
                    return (
                        FuzzySearch.FuzzyMatch(((Usuario) empleado).Nombre, newText));
                });
                var apPat = new Predicate<object>(empleado => {
                    if (empleado == null) return false;
                    return (
                        FuzzySearch.FuzzyMatch(((Usuario) empleado).ApPaterno, newText));
                });
                
                collection.Filter = noNull;
                collection.Filter = usuarios;
                collection.Filter = nombre;
                collection.Filter = apPat;
                DataUsuarios.ItemsSource = collection;
                CollectionView = collection;
            }
            else {
                collection.Filter = null;
                var noNull = new Predicate<object>(empleado => {
                    if (empleado == null) return false;
                    return ((Usuario) empleado).Activo == 1;
                });
                collection.Filter = noNull;
                DataUsuarios.ItemsSource = collection;
                CollectionView = collection;
            }
        }

        /// <summary>
        /// Función para editar el usuario seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditarUsuario(object sender, RoutedEventArgs e) {
            if (StrPrep(nombreUsuario.Text) != StrPrep(SelectedUser.Nombre)) {
                SelectedUser.Nombre = nombreUsuario.Text;
            }
            if (StrPrep(apPaternoUsuario.Text) != StrPrep(SelectedUser.ApPaterno)) {
                SelectedUser.ApPaterno = apPaternoUsuario.Text;
            }
            if (StrPrep(apMaternoUsuario.Text) != StrPrep(SelectedUser.ApMaterno)) {
                SelectedUser.ApMaterno = apMaternoUsuario.Text;
            }
            if (ComboRol.SelectedIndex + 1 != SelectedUser.IdRol) {
                SelectedUser.IdRol = ComboRol.SelectedIndex + 1; }
            if (ComboSucursal.SelectedIndex + 1 != SelectedUser.IdTienda) {
                SelectedUser.IdTienda = ComboSucursal.SelectedIndex + 1;
            }
            if (StrPrep(TxtDireccion.Text) != StrPrep(SelectedUser.Domicilio)) {
                SelectedUser.Domicilio = TxtDireccion.Text;
            }
            if (StrPrep(TxtContraseña.Text) != StrPrep(SelectedUser.Contraseña)) {
                SelectedUser.Contraseña = TxtContraseña.Text;
            }
            if (int.Parse(StrPrep(TxtTelefono.Text)) != SelectedUser.Telefono) {
                SelectedUser.Telefono = int.Parse(TxtTelefono.Text);
            }

            var mensaje = "Está a punto de editar al usuario: \n" + SelectedUser.Nombre + " " + SelectedUser.ApPaterno + " " + SelectedUser.ApMaterno + "\n"
               + "¿Desea continuar?";
            var titulo = "Confirmación de Edicionde Usuario";
            if (MessageBox.Show(mensaje, titulo, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                Database.UpdateData(SelectedUser);
                FillDataGrid();
                ClearFields();
            }

               
        }

        /// <summary>
        /// Lógica detras del boton de registrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegistrarUsuario(object sender, RoutedEventArgs e) {
            var newUsuario = new Usuario();
            // Campos de texto
            newUsuario.Nombre = nombreUsuario.Text;
            newUsuario.ApPaterno = apPaternoUsuario.Text;
            newUsuario.ApMaterno = apMaternoUsuario.Text;
            newUsuario.Domicilio = TxtDireccion.Text;
            newUsuario.Telefono = int.Parse(TxtTelefono.Text);
            newUsuario.Contraseña = TxtContraseña.Text;
            newUsuario.IdTienda = ComboSucursal.SelectedIndex + 1;
            newUsuario.IdRol = ComboRol.SelectedIndex + 1; 
            // Campos generados
            newUsuario.Activo = 1;
            newUsuario.UsuraioRegistra = Database.GetUsername();
            newUsuario.UsuarioModifica = Database.GetUsername();
            newUsuario.Username = newUsuario.Nombre[..2] + newUsuario.ApPaterno.ToLower();

            // ventana de confirmación
            var mensaje = "Agregar al usuario: \n" + newUsuario.Nombre + " " + newUsuario.ApPaterno + " " + newUsuario.ApMaterno + "\n"
                + "Con el nombre de usuario: " + newUsuario.Username;
            var titulo = "Confirmación de Usuario Nuevo";

            if (MessageBox.Show(mensaje, titulo, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK) {
                // TODO: hacer valoración de que no existe.
                // TODO: si ya existe darlo de alta.
                foreach (var usuario in UsuariosList) {
                    if (usuario.Username == newUsuario.Username) {
                        var msg = "Error: usuario " + usuario.Nombre + " " + usuario.ApPaterno + " ya existe.";
                        const string title = "Usuario duplicado";
                        MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                Database.NewUser(newUsuario);           
                FillDataGrid();
                ClearFields();
            }
            
        }

        /// <summary>
        /// Elimina (hace inactivo) el usuario seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarUsuario(object sender, RoutedEventArgs e) {
            var usuario = SelectedUser;
            string mensaje = "¿Seguro quiere eliminar a el usuario " + usuario.Nombre + " " + usuario.ApPaterno +"?";
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            if (MessageBox.Show(mensaje, "Confirmación", buttons, icon) == MessageBoxResult.OK) {
                usuario.Activo = 0;
                Database.UpdateData(usuario);
                ClearFields();
                FillDataGrid();
            }
            

        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
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
            //string menssage = "¡Desea limpiar los campos?";
            //string titulo = "Confirmación";

            //if (MessageBox.Show("Do you want to close this window?",
            //"Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                
            //}

        }

        // -- Eventos de TextUpdate.
        // TODO: buscar una forma de hacerlos genericos

        private void TextUpdatePswd(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtContraseña.Text = textbox.Text;
        }

        private void TextUpdateUserName(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            nombreUsuario.Text = textbox.Text;
        }

        private void TextUpdateApMa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apMaternoUsuario.Text = textbox.Text;
        }

        private void TextUpdateApPa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apPaternoUsuario.Text = textbox.Text;
        }

        private void TextUpdateDir(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtDireccion.Text = textbox.Text;
        }

        private void TextUpdateTel(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            TxtTelefono.Text = textbox.Text;
        }

        //TODO: nunca usado
        private void TextUpdateRol(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            ComboRol.Text = textbox.Text;
        }
    }
}