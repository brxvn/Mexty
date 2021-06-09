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
using Mexty.MVVM.Model;

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
            Database dbConnection = new Database(txtUsuario.Text, pswrdUsuario.Password);

            if (dbConnection.IsConnected()) {
                var rol = dbConnection.GetRol();
                MainWindow win = new MainWindow(dbConnection, rol);
                win.Show();
                this.Close();                
            }

            else
                MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo");

            dbConnection.CloseConnection(); //TODO: no cerrar conección y pasar el objeto
        }

        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }
    }
}
