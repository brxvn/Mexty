using Mexty.MVVM.Model;
using Mexty.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Mexty {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
            private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainWindow() {
            log4net.Config.XmlConfigurator.Configure();
            
            InitializeComponent();
            BarCodes barCodes = new();
                        
            DataContext = new MainViewModel();
            
            if (Database.GetRol().Equals(2)) {
                Admn.Visibility = Visibility.Collapsed;
            }
            activeUser.Text = Database.GetUsername();

            //Para mostrar la hora actual del sistema
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
 
        }

        /// <summary>
        /// Método para mostrar hora y fecha actual en pantalla.
        /// </summary>
        public void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Logica del botón para cerrar sesión desde la pantalla principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SignOut(object sender, RoutedEventArgs e) {
            var message = "¿Desea salir?";
            var title = "Confirmación.";
            if (MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                Database.CloseConnection();
                Application.Current.Shutdown();

            }
            
        }

    }
}
