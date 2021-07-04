using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Mexty.MVVM.Model;
using log4net;
using log4net.Config;

namespace Mexty {
    /// <summary>
    /// Lógica para <c>Login.xaml</c>.
    /// </summary>
    public partial class Login : Window {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Login() {
            
            
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Test info");
            
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
            _ = new Database(txtUsuario.Text, pswrdUsuario.Password);
            if (Database.IsConnected()) {
                MainWindow win = new();
                win.Show();
                Close();
            }
            else {
                MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo");
            }
            
        }

        /// <summary>
        /// Método para mostrar hora y fecha actual en pantalla.
        /// </summary>
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Logica del boton para salir de la aplicación.
        /// </summary>
        private void LogOut(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Lógica para detectar el Enter en el password e inicie la sesion.
        /// </summary>
        private void EnterKeyPassword(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                _ = new Database(txtUsuario.Text, pswrdUsuario.Password);
                if (Database.IsConnected()) {
                    MainWindow win = new();
                    win.Show();
                    Close();
                }
                else MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo");
            }
        }
    }
}