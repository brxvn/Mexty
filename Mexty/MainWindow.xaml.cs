using Mexty.MVVM.Model;
using Mexty.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;

namespace Mexty {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
            private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        public MainWindow() {
            log4net.Config.XmlConfigurator.Configure();
            
            InitializeComponent();
            BarCodes barCodes = new();
                        
            DataContext = new MainViewModel();
            
            if (DatabaseInit.GetIdRol().Equals(3)) {
                Admn.Visibility = Visibility.Collapsed;
                Reportes.Visibility = Visibility.Collapsed;
            }

            activeUser.Text = DatabaseInit.GetUsername();

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
                DatabaseInit.CloseConnection();
                Application.Current.Shutdown();
            }
        }
    }
}
