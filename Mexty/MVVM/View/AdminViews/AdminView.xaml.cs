using Mexty.MVVM.ViewModel;
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

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl {
       

        public AdminView() {
            InitializeComponent();
            DataContext = new MainViewModel();

            if (Model.DatabaseQuerys.DatabaseInit.GetIdTiendaIni() != 6) {
                tabUsuarios.Visibility = Visibility.Collapsed;
                tabProductos.Visibility = Visibility.Collapsed;
                tabEstablecimientos.Visibility = Visibility.Collapsed;
                tabClientes.Visibility = Visibility.Collapsed;
            }
            else {
                tabUsuarios.Visibility = Visibility.Visible;
                tabProductos.Visibility = Visibility.Visible;
                tabEstablecimientos.Visibility = Visibility.Visible;
                tabClientes.Visibility = Visibility.Visible;
            }
            
        }
    }
}
