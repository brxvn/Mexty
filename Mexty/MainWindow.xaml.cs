using Mexty.MVVM.Model;
using Mexty.MVVM.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace Mexty {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow(Database usuario, string rol) { // TODO: no ne cesitamos pasar rol
            
            InitializeComponent();

            DataContext = new MainViewModel();
            // Mostramos el usuario activo

            //switch (rol) {
            //    case "2":
            //        Invent.Visibility = Visibility.Collapsed;
            //        break;
            //}
            activeUser.Text = usuario.GetNombreUsuario();

            //Para mostrar la hora actual del sistema
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
 
        }

        public void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

    }
}
