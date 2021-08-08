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
using System.Windows.Shapes;
using log4net;
using Mexty.MVVM.Model.DatabaseQuerys;

namespace Mexty.MVVM.View.InventarioViews {
    /// <summary>
    /// Interaction logic for MovimientosInventario.xaml
    /// </summary>
    public partial class MovimientosInventario : Window {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public MovimientosInventario() {
            try {
                InitializeComponent();
                FillData();
                Log.Debug("Se han inicializado los campos de movimientos inventario de manera exitosa.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de movimientos inventario.");
                Log.Error($"Error: {e.Message}");
            }
        }

        private void FillData() {
            var data = QuerysMovInvent.GetTablesFromMovInventario();
            DataProducts.ItemsSource = data;
            Log.Debug("Se ha llenado la data grid de moviemientos inventario con exito.");

        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }

        private void ImprimirTxt(object sender, RoutedEventArgs e) {
            MessageBox.Show("TODO:");
        }
    }
}
