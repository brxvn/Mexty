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
    /// Lógica para <c>Login.xaml</c>.
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
        /// Lógica de el botón de Log-in.
        /// </summary>
        private void PasswordKeyDown(object sender, RoutedEventArgs e) {
            var dbConnection = new Database(txtUsuario.Text, pswrdUsuario.Password);
            if (Database.IsConnected()) {
                var rol = Database.GetRol(); // TODO pasar el rol
                MainWindow win = new MainWindow(dbConnection);
                win.Show();
                this.Close();
            }
            else { 
                MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo");
            }
            Database.CloseConnection(); //TODO: no cerrar conección y pasar el objeto
        }

        // TODO: documentar
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        private void LogOut(object sender, RoutedEventArgs e) {
            App.Current.Shutdown();
        }
    }
}