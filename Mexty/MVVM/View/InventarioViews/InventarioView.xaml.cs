using Mexty.MVVM.Model.DatabaseQuerys;
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

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for InventarioView.xaml
    /// </summary>
    public partial class InventarioView : UserControl {
        public InventarioView() {
            InitializeComponent();
            if (DatabaseInit.GetIdRol().Equals(3) && !DatabaseInit.GetMatrizEnabled()) {
                inventarioMatriz.Visibility = Visibility.Collapsed;
            }
        }
    }
}
