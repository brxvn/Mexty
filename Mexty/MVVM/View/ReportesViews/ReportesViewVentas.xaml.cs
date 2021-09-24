using log4net;
using Mexty.MVVM.Model;
using Mexty.MVVM.Model.DatabaseQuerys;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mexty.MVVM.View.ReportesViews {
    /// <summary>
    /// Interaction logic for ReportesViewVentas.xaml
    /// </summary>
    public partial class ReportesViewVentas : UserControl {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        ReportesVentas report = new();
        ReporteVentasMayoreo VentasMayoreo = new();
        List<Sucursal> dataSucursal = QuerysSucursales.GetTablesFromSucursales();
        List<Usuario> dataUsuarios = QuerysUsuario.GetTablesFromUsuarios();

        private int idSucursal;
        private int idUsuario;
        public ReportesViewVentas() {
            try {
                InitializeComponent();
                FillDataSucursales();
                FillDataUsuarios();
                Log.Debug("Se han llenado los campos de reportes de ventas de manera exitosa.");

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al llenar los campos de reportes de ventas");
                Log.Error($"Error {e.Message}");
            }
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(UpdateTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            lblSucursal.Content = DatabaseInit.GetNombreTiendaIni();

            if (DatabaseInit.GetIdRol().Equals(3)) {
                Row0.Height = new GridLength(0, GridUnitType.Star);
                Row1.Height = new GridLength(0, GridUnitType.Star);
                Row2.Height = new GridLength(0, GridUnitType.Star);
                Row3.Height = new GridLength(0, GridUnitType.Star);
            }

        }

        private void FillDataUsuarios() {
            try {
                foreach (var sucu in dataSucursal) {
                    var nombre = sucu.NombreTienda;
                    ComboSucursal.Items.Add($"{char.ToUpper(nombre[0]) + nombre.Substring(1)} ");
                }

                ComboSucursal.SelectedIndex = 0;
                Log.Debug("Se ha llenado el combo de sucursal");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al llenar los campos de reportes inventario");
                Log.Error($"Error {e.Message}");
            }
        }

        private void FillDataSucursales() {
            try {

                if (DatabaseInit.GetIdRol().Equals(3)) {
                    var username = DatabaseInit.GetUsername();
                    foreach (var user in dataUsuarios) {
                        var nombre = user.Nombre;
                        var apPat = user.ApPaterno;
                        if (username == user.Username) {
                            ComboEmpleado.Items.Add($"{char.ToUpper(nombre[0]) + nombre.Substring(1)} {char.ToUpper(apPat[0]) + apPat.Substring(1)}");
                            ComboEmpleado.SelectedIndex = 0;
                            ComboEmpleado.IsEnabled = false;
                            ComboEmpleado.IsReadOnly = true;
                            break;
                        }
                    }
                }
                else {
                    foreach (var user in dataUsuarios) {
                        var nombre = user.Nombre;
                        var apPat = user.ApPaterno;
                        ComboEmpleado.Items.Add($"{char.ToUpper(nombre[0]) + nombre.Substring(1)} {char.ToUpper(apPat[0]) + apPat.Substring(1)}");
                    }

                    ComboEmpleado.SelectedIndex = 0;
                    Log.Debug("Se ha llenado el combo de empleados");

                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al llenar los campos de reportes inventario");
                Log.Error($"Error {e.Message}");
            }
        }

        /// <summary>
        /// Actualiza la hora.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e) {
            time.Content = DateTime.Now.ToString("G");
        }


        private void BtnHoy_Click(object sender, RoutedEventArgs e) {
            foreach (var sucu in dataSucursal) {
                var seleccion = ComboSucursal.SelectedItem.ToString().ToLower().Trim();
                if (sucu.NombreTienda.ToLower() == seleccion) {
                    idSucursal = sucu.IdTienda;
                    break;
                }
            }
            if (comboVenta.SelectedIndex == 0) {
                VentasMayoreo.ReportesVentasMayoreo(idSucursal, "hoy");
            }
            else {
                report.ReporteVentasSucursal(idSucursal, "hoy");
            }
        }


        private void UltimaSemana_Click(object sender, RoutedEventArgs e) {
            foreach (var sucu in dataSucursal) {
                var seleccion = ComboSucursal.SelectedItem.ToString().ToLower().Trim();
                if (sucu.NombreTienda.ToLower() == seleccion) {
                    idSucursal = sucu.IdTienda;
                    break;
                }
            }
            if (comboVenta.SelectedIndex == 0) {
                VentasMayoreo.ReportesVentasMayoreo(idSucursal, "semana");
            }
            else {
                report.ReporteVentasSucursal(idSucursal, "semana");
            }
        }

        private void Mes_Click(object sender, RoutedEventArgs e) {
            foreach (var sucu in dataSucursal) {
                var seleccion = ComboSucursal.SelectedItem.ToString().ToLower().Trim();
                if (sucu.NombreTienda.ToLower() == seleccion) {
                    idSucursal = sucu.IdTienda;
                    break;
                }
            }
            if (comboVenta.SelectedIndex == 0) {
                VentasMayoreo.ReportesVentasMayoreo(idSucursal, "mes");
            }
            else {
                report.ReporteVentasSucursal(idSucursal, "mes");
            }
        }

        private void btnHoyUsuario_Click(object sender, RoutedEventArgs e) {
            string username = "";
            var allText = ComboEmpleado.SelectedItem.ToString().ToLower();
            string[] nombre = allText.Split(' ');
            foreach (var item in dataUsuarios) {
                if (item.Nombre.ToLower() == nombre[0]) {
                    username = item.Username;
                    break;
                }
            }

            report.ReporteVentasUsuario(username, "hoy");
        }

        private void btnSemanaUsuario_Click(object sender, RoutedEventArgs e) {
            string username = "";
            var allText = ComboEmpleado.SelectedItem.ToString().ToLower();
            string[] nombre = allText.Split(' ');
            foreach (var item in dataUsuarios) {
                if (item.Nombre.ToLower() == nombre[0]) {
                    username = item.Username;
                    break;
                }
            }

            report.ReporteVentasUsuario(username, "semana");
        }

        private void btnMesUsuario_Click(object sender, RoutedEventArgs e) {
            string username = "";
            var allText = ComboEmpleado.SelectedItem.ToString().ToLower();
            string[] nombre = allText.Split(' ');
            foreach (var item in dataUsuarios) {
                if (item.Nombre.ToLower() == nombre[0]) {
                    username = item.Username;
                    break;
                }
            }

            report.ReporteVentasUsuario(username, "mes");
        }

        private void ComboSucursal_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ComboSucursal.SelectedValue.ToString().ToLower().Trim() == "jiquipilco".ToLower()) {
                gridTipoVenta.Visibility = Visibility.Visible;
            }
            else {
                gridTipoVenta.Visibility = Visibility.Collapsed;
            }
        }
    }
}
