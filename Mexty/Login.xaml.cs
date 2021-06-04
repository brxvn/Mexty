using MySql.Data.MySqlClient;
using System;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mexty {
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window {
        public Login() {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        
        /// <summary>
        /// Creamos la connexion con la base de datos para validad si el usuario introducido es un usuario activo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pswrdKeyDown(object sender, RoutedEventArgs e) {
            MySqlConnection conn = new MySqlConnection("server=localhost; database = mexty; Uid=root; pwd = root; ");

            conn.Open();

            MySqlCommand login = new MySqlCommand();
            login.Connection = conn;

            login.CommandText = ("select usuario, contrasenia from usuario where usuario = '"+txtUsuario.Text+"' and contrasenia = '"+pswrdUsuario.Password.ToString()+"' ");

            MySqlDataReader rd = login.ExecuteReader();

            if (rd.Read()) {
                MainWindow win = new MainWindow(txtUsuario.Text);
                win.Show();
                this.Close();
            }

            else
                MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo");

            conn.Close();
        }

        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }
    }
}
