using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
using Mexty.MVVM.Model.DataTypes;

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
            SortDataGrid(DataProducts, 0);
            Log.Debug("Se ha llenado la data grid de moviemientos inventario con exito.");

        }

        /// <summary>
        /// Ordenar por piezas de manera ascendente 
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="columnIndex"></param>
        /// <param name="sortDirection"></param>
        void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Descending) {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns) {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }

        private void CerrarVentana(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e) {


        }

        private void ImprimirTxt(object sender, RoutedEventArgs e) {
            List<LogInventario> mensajes = new();
            IList list = DataProducts.SelectedItems;
            foreach (var item in list) {
                mensajes.Add(item as LogInventario);
            }
            TicketAsignacion ticket = new(mensajes);
            
        }
    }
}
