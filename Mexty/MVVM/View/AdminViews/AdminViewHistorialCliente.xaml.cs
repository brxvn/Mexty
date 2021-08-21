using Mexty.MVVM.Model.DataTypes;
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

namespace Mexty.MVVM.View.AdminViews {
    /// <summary>
    /// Interaction logic for AdminViewHistorialCliente.xaml
    /// </summary>
    public partial class AdminViewHistorialCliente : Window {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Cliente seleccionado.
        /// </summary>
        private Cliente SelectedClient { get; set; }

        public AdminViewHistorialCliente(Cliente cliente) {
            try {
                InitializeComponent();
                SelectedClient = cliente;
                FillData();

                Log.Debug("Se han inicializado los cambios de ");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al inicializar los campos de Historial cliente.");
                Log.Error($"Error: {e.Message}");
            }
        }

        private void FillData() {
            var data = QuerysMovClientes.GetTablesFromMovClientes(SelectedClient.IdCliente);
            DataMovimientos.ItemsSource = data;
            Log.Debug("Se ha llendado la datagrid de historial cliente con exito.");
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {

        }
    }
}
