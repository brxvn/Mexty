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
using Mexty.MVVM.Model.DatabaseQuerys;

using System.Windows.Shapes;

namespace Mexty.MVVM.View.ReportesViews {
    /// <summary>
    /// Interaction logic for ReportesView.xaml
    /// </summary>
    public partial class ReportesView : UserControl {
        public ReportesView() {
            InitializeComponent();

            if (DatabaseInit.GetIdRol().Equals(3)) {
                ReporteInventario.Visibility = Visibility.Collapsed;
            }
        }
    }
}
