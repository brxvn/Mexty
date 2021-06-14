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

        private void FillRol() {
            
        }

        private void FillSucursales() {
            
        }
        
        /// <summary>
        /// Funcion que obtiene el item selecionado de la <c>datagrid</c>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e) {
            ClearFields();
            Usuario usuario = (Usuario) DataUsuarios.SelectedItem;
            SelectedUser = usuario;
            nombreUsuario.Text = usuario.Username;
            apPaternoUsuario.Text = usuario.ApPaterno;
            apMaternoUsuario.Text = usuario.ApMaterno;
            switch (usuario.IdTienda) {
                case 1 :
                    sucursal.SelectedItem = "Matriz";
                    break;
                case 2 :
                    sucursal.SelectedItem = "Sucursal 1";
                    break;
                case 3:
                    sucursal.SelectedItem = "Sucusal 2";
                    break;
            }
            txtDireccion.Text = usuario.Domicilio;
            txtTelefono.Text = usuario.Telefono.ToString(); //ojo
            txtContraseña.Text = usuario.Contraseña;

        }
        /// <summary>
        /// Función que limpia los campos de datos.
        /// </summary>
        private void ClearFields() {
            nombreUsuario.Text = "";
            apPaternoUsuario.Text = "";
            apMaternoUsuario.Text = "";
            sucursal.SelectedIndex = 0;
            txtDireccion.Text = "";
            txtContraseña.Text = "";
            txtTelefono.Text = "";
            Rol.SelectedIndex = 0 ;
        }

        /// <summary>
        /// Lógica para el boton de busqueda.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterSearch(object sender, TextChangedEventArgs e) {
            TextBox tbx = sender as TextBox;
            if (tbx.Text != "") {
                var newText = tbx.Text;
                CollectionView.Filter = (e) => { // TODO: probablemente Hacer una clase con esto para reutilizarlo
                    Usuario emp = e as Usuario;
                    
                    if (emp.Activo == 0) { // solo usuarios activos.
                        return false;
                    }
                    if (FuzzySearch.FuzzyMatch(emp.Nombre, newText)) { // Por nombre.
                        return true;
                    }
                    if (FuzzySearch.FuzzyMatch(emp.Username, newText)) { // Por Nombre de usuario.
                        return true;
                    }
                    if (FuzzySearch.FuzzyMatch(emp.ApPaterno, newText)) { // Por Ap paterno.
                        return true;
                    }
                    return false;
                };
                DataUsuarios.ItemsSource = CollectionView;
            }
            else {
                CollectionView.Filter = null;
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
            // TODO: verificar sucursal
            // if (Int32.TryParse(StrPrep(sucursal.GetValue( T
            //     //
            // }
            if (StrPrep(txtDireccion.Text) != StrPrep(SelectedUser.Domicilio)) {
                SelectedUser.Domicilio = txtDireccion.Text;
            }
            if (StrPrep(txtContraseña.Text) != StrPrep(SelectedUser.Contraseña)) {
                SelectedUser.Contraseña = txtContraseña.Text;
            }
            if (int.Parse(StrPrep(txtTelefono.Text)) != SelectedUser.Telefono) {
                SelectedUser.Telefono = int.Parse(txtTelefono.Text);
            }
            var dbObj = new Database(); 
            Database.UpdateData(SelectedUser);
            FillDataGrid();
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
            newUsuario.Domicilio = txtDireccion.Text;
            newUsuario.Telefono = int.Parse(txtTelefono.Text);
            newUsuario.Contraseña = txtContraseña.Text;
            newUsuario.IdTienda = 1;// TODO: armar objetos de tienda.
            newUsuario.IdRol = 1; // TODO: armar objetos de rol. tabla cat_rol_usuario.
            // Campos generados
            newUsuario.Activo = 1;
            newUsuario.UsuraioRegistra = Database.GetUsername();
            newUsuario.UsuarioModifica = Database.GetUsername();
            newUsuario.Username = newUsuario.Nombre[..2] + newUsuario.ApPaterno;
            // TODO: hacer valoración de que no existe.
            // TODO: si ya existe darlo de alta.
            foreach (var usuario in UsuariosList) {
                if (usuario.Username == newUsuario.Username) {
                    var msg = "Error: usuario " + usuario.Nombre + " " + usuario.ApMaterno +
                                 " Ya tiene el mismo nombre de usuario y probablemente ya este registrado.";
                    const string title = "Posible registro de usuario duplicado";
                    MessageBox.Show(msg, title);
                }
            }
            
            Database.NewUser(newUsuario);
            ClearFields();
            FillDataGrid();
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
        }

        /// <summary>
        /// Elimina (hace inactivo) el usuario seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarUsuario(object sender, RoutedEventArgs e) {
            var usuario = SelectedUser;
            usuario.Activo = 0;
            var db = new Database();
            Database.UpdateData(usuario);
            ClearFields();
            FillDataGrid();
        }

        // -- Eventos de TextUpdate.
        // TODO: buscar una forma de hacerlos genericos

        private void TextUpdatePswd(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtContraseña.Text = textbox.Text;
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
            txtDireccion.Text = textbox.Text;
        }

        private void TextUpdateTel(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtTelefono.Text = textbox.Text;
        }

        //TODO: nunca usado
        private void TextUpdateRol(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            Rol.Text = textbox.Text;
        }
    }
}