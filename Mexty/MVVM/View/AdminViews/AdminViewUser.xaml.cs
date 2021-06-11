using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Cms;

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewUser.xaml
    /// </summary>
    public partial class AdminViewUser : UserControl {

        public AdminViewUser() {
            InitializeComponent();
            
            FillDataGrid();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
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
            
        }

        /// <summary>
        /// Funcion que obtiene el item selecionado de la <c>datagrid</c>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemSelected(object sender, EventArgs e) {
            ClearFields();
            Usuarios usuario = (Usuarios) DataUsuarios.SelectedItem;
            nombreUsuario.Text = usuario.Usuario;
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
            if (usuario.Activo == 1) {
                activo.IsChecked = true;
            }
            else {
                activo.IsChecked = false;
            }
        }
        /// <summary>
        /// Función que limpia los campos de datos.
        /// </summary>
        public void ClearFields() {
            nombreUsuario.Text = "";
            apPaternoUsuario.Text = "";
            apMaternoUsuario.Text = "";
            sucursal.SelectedItem = "Mexty";
            txtDireccion.Text = "";
            txtContraseña.Text = "";
            txtTelefono.Text = "";
            activo.IsChecked = false;
        }
        
        //private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {

        //    var tbx = sender as TextBox;
        //    string empty = "";
        //    if (tbx.Text != "") {
        //        var newtext = tbx.Text;
        //        labelText.Content = newtext;
        //        labelText.Visibility = Visibility.Visible;
        //    }
        //    else labelText.Content = empty;
        //}
    }
}
