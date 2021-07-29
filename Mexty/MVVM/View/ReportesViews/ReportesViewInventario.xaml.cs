using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Mexty.MVVM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Common.Logging;

namespace Mexty.MVVM.View.ReportesViews {
    /// <summary>
    /// Interaction logic for ReportesViewInventario.xaml
    /// </summary>
    public partial class ReportesViewInventario : UserControl {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        Reports report = new Reports();
        public ReportesViewInventario() {
            try {
                InitializeComponent();
                FillData();
                Log.Debug("Se han llenado los campos de reportes de inventario de manera exitosa.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al llenar los campos de reportes inventario");
                Log.Error($"Error {e.Message}");
            }

            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        /// <summary>
        /// Actualiza la hora.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }

        /// <summary>
        /// Método que se encarga de llenar los campos de Reportes inventario.
        /// </summary>
        private void FillData() {
            try {
                var dataSucursal = Database.GetTablesFromSucursales();
                foreach (var sucu in dataSucursal) {
                    ComboSucursal.Items.Add(sucu.NombreTienda);
                }
                Log.Debug("Se ha llenado el combo de sucursal");

                var dataProductos = Database.GetTablesFromProductos();
                foreach (var pro in dataProductos) {
                    ComboTipo.Items.Add($"{pro.TipoProducto} {pro.NombreProducto}");
                }
                Log.Debug("Se ha llenado el combo de productos.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al llenar los campos de reportes inventario");
                Log.Error($"Error {e.Message}");
            }
        }
    }
}
