using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Mexty.MVVM.Model;
using log4net;
using log4net.Config;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using Mexty.MVVM.Model.DatabaseQuerys;

namespace Mexty {
    /// <summary>
    /// Lógica para <c>Login.xaml</c>.
    /// </summary>
    public partial class Login : Window {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        public Login() {
            XmlConfigurator.Configure();
            Log.Info("Iniciado login");

            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        /// <summary>
        /// Lógica de el botón de Log-in.
        /// </summary>
        private async void PasswordKeyDown(object sender, RoutedEventArgs e) {
            LogIn();
            await Task.Run(DepuraAveces);
        }

        /// <summary>
        /// Método para mostrar hora y fecha actual en pantalla.
        /// </summary>
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Método que decide si depurar la base de datos o no
        /// </summary>
        private static void DepuraAveces() {
            var rndGen = new Random();
            var rndNum = rndGen.Next(1, 100);
            if (rndNum % 3 == 0) {
                QuerysMantenimiento.DepCamposVentas();
                QuerysMantenimiento.DepCamposImport();
            }
        }

        /// <summary>
        /// Logica del boton para salir de la aplicación.
        /// </summary>
        private void LogOut(object sender, RoutedEventArgs e) {
            Log.Debug("Presionado boton de Salir en el login.");
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Método de logIn.
        /// </summary>
        private void LogIn() {
            try {
                if (DatabaseInit.UserLogIn(txtUsuario.Text, pswrdUsuario.Password)) {
                    Log.Info("Login exitoso");
                    MainWindow win = new();
                    win.Show();
                    Close();
                }
                else {
                    Log.Info("Login fallido, Usuario o contraseña incorrectos.");
                    MessageBox.Show("Usuario o contraseña incorrectos, intente de nuevo");
                }
            }
            catch (Exception e) {
                Log.Error("Proceso de inicio de sesión Fallido.");
                Log.Error($"Error: {e.Message}");
                //throw; // Descomentar si queremos que si ocurre un error se cierre la aplicación.
            }
        }

        /// <summary>
        /// Lógica para detectar el Enter en el password e inicie la sesion.
        /// </summary>
        //TODO: juntarlo con el login o hacer función de login
        private async void EnterKeyPassword(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                LogIn();
                await Task.Run(DepuraAveces);
            }
        }

        private void passwordChanged(object sender, RoutedEventArgs e) {
            if (pswrdUsuario.Password.Length == 0)
                pswrdUsuario.Background.Opacity = 1;
            else
                pswrdUsuario.Background.Opacity = 0;
        }

        private void textChanged(object sender, TextChangedEventArgs e) {
            if (txtUsuario.Text.Length == 0)
                txtUsuario.Background.Opacity = 1;
            else
                txtUsuario.Background.Opacity = 0;
        }

        private void hideText(object sender, DependencyPropertyChangedEventArgs e) {
            if (txtUsuario.Text.Length == 0)
                txtUsuario.Background.Opacity = 0;
        }

        private void showText(object sender, RoutedEventArgs e) {
            if (txtUsuario.Text.Length == 0)
                txtUsuario.Background.Opacity = 1;
        }

        private void hidePswrd(object sender, DependencyPropertyChangedEventArgs e) {
            if (pswrdUsuario.Password.Length == 0)
                pswrdUsuario.Background.Opacity = 0;
        }

        private void showPswrd(object sender, RoutedEventArgs e) {
            if (pswrdUsuario.Password.Length == 0)
                pswrdUsuario.Background.Opacity = 1;
        }
    }
}