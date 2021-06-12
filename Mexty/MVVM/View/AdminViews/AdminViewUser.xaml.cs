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
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewUser.xaml
    /// </summary>
    public partial class AdminViewUser : UserControl {

        private ListCollectionView _collectionView;
        private Database _usuarioActivo;
        private int _sucursalID;
        public AdminViewUser() {
            InitializeComponent();
            
            FillDataGrid();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            switch (sucursal.Text) {
                case "Mexty":
                    _sucursalID = 1;
                    break;
                
            }
        }

        public void UpdateTimerTick(object sender, EventArgs e) { // TODO: hacerlo una clase para no repetir código
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Método que llena la tabla con los datos de la tabla usuarios.
        /// </summary>
        public void FillDataGrid() {
            var connObj = new Database();
            var query = connObj.GetTablesFromUsuarios();
            DataUsuarios.ItemsSource = query;
            // TODO: Armar una lista de objetos con las sucursales y usarlas como source para el combobox de las sucursales.
            ListCollectionView collectionView = new ListCollectionView(query);
            _collectionView = collectionView;
        }

        /// <summary>
        /// Funcion que obtiene el item selecionado de la <c>datagrid</c>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemSelected(object sender, EventArgs e) {
            ClearFields();
            Usuarios usuario = (Usuarios) DataUsuarios.SelectedItem;
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
        public void ClearFields() {
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
                string newtext = tbx.Text;
                _collectionView.Filter = (e) => { // TODO: probablemente Hacer una clase con esto para reutilizarlo
                    Usuarios emp = e as Usuarios;// TODO: Armar mejor lógica para filtrado
                    if (emp.Id.ToString() == newtext) {
                        return true;
                    }
                    if (emp.Nombre == newtext) {
                        return true;
                    }
                    if (emp.Username == newtext) {
                        return true;
                    }
                    return false;
                };
                DataUsuarios.ItemsSource = _collectionView;
            }
            else {
                _collectionView.Filter = null;
            }
        }

        public void EditBtn(object sender, RoutedEventArgs e) {
            Usuarios selectedUser = (Usuarios) DataUsuarios.SelectedItem;
            if (StrPrep(nombreUsuario.Text) != StrPrep(selectedUser.Nombre)) {
                selectedUser.Nombre = nombreUsuario.Text;
            }
            if (StrPrep(apPaternoUsuario.Text) != StrPrep(selectedUser.ApPaterno)) {
                selectedUser.ApPaterno = apPaternoUsuario.Text;
            }
            if (StrPrep(apMaternoUsuario.Text) != StrPrep(selectedUser.ApMaterno)) {
                selectedUser.ApMaterno = apMaternoUsuario.Text;
            }
            // TODO: verificar sucursal
            // if (Int32.TryParse(StrPrep(sucursal.GetValue( T
            //     //
            // }
            if (StrPrep(txtDireccion.Text) != StrPrep(selectedUser.Domicilio)) {
                selectedUser.Domicilio = txtDireccion.Text;
            }
            if (StrPrep(txtContraseña.Text) != StrPrep(selectedUser.Contraseña)) {
                selectedUser.Contraseña = txtContraseña.Text;
            }
            if (int.Parse(StrPrep(txtTelefono.Text)) != selectedUser.Telefono) {
                selectedUser.Telefono = int.Parse(txtTelefono.Text);
            }
            var dbObj = new Database(); 
            dbObj.UpdateData(selectedUser);
            FillDataGrid();
        }

        private void btnRegistrar(object sender, RoutedEventArgs e) {
            Database connObj = new();
                
            connObj.NewUser(
                nombreUsuario.Text,
                apPaternoUsuario.Text,
                apMaternoUsuario.Text,
                txtDireccion.Text,
                txtTelefono.Text,
                txtContraseña.Text,
                _sucursalID  
                );


            FillDataGrid();
            ClearFields();
        }

        /// <summary>
        /// Función que valida los campos númericos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnlyNumbersValidation(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        
        /// <summary>
        /// Regresa la cadena dada en Mayusculas y sin espeacios.
        /// </summary>
        /// <param name="text">Texto a Preparar.</param>
        /// <returns></returns>
        public string StrPrep(string text) {
            return text.ToUpper().Replace(" ", "");
        }

        public void TextUpdatePswd(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtContraseña.Text = textbox.Text;
        }
        
        public void TextUpdateUserName(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            nombreUsuario.Text = textbox.Text;
        }
        
        
        public void TextUpdateApMa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apMaternoUsuario.Text = textbox.Text;
        }
        
        public void TextUpdateApPa(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            apPaternoUsuario.Text = textbox.Text;
        }
        
        public void TextUpdateDir(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtDireccion.Text = textbox.Text;
        }
        
        public void TextUpdateTel(object sender, TextChangedEventArgs a) {
            TextBox textbox = sender as TextBox;
            txtTelefono.Text = textbox.Text;
        }

        private void TextUpdateRol(object sender, TextChangedEventArgs e) {
            TextBox textbox = sender as TextBox;
            Rol.Text = textbox.Text;
        }

        /// <summary>
        /// Limpia los text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLimpiar(object sender, RoutedEventArgs e) {
            ClearFields();
        }
    }
}