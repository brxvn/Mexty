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

namespace Mexty.MVVM.View.VentasViews {
    /// <summary>
    /// Interaction logic for VentasView.xaml
    /// </summary>
    public partial class VentasView : UserControl {
        public VentasView() {
            InitializeComponent();

            if (DatabaseInit.GetIdRol().Equals(3)) {
                VentasMayoreo.Visibility = Visibility.Collapsed;
            }

            if (!DatabaseInit.GetMatrizEnabledFromIni()) {
                VentasMayoreo.Visibility = Visibility.Collapsed;
            }
        }
    }
}
