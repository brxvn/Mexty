using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que contiene las consultas a la base de datos para los reportes de ventas.
    /// </summary>
    public class QuerysReportesVentas {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        public static List<Venta> GetTablesFromVentasToReport(int idtienda, string username, bool mayoreo = false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = mayoreo ? @"select * from venta_mayoreo where ID_TIENDA=@idT and USUARIO_REGISTRA=@user" :
                @"select * from venta_menudeo where ID_TIENDA=@idT and USUARIO_REGISTRA=@user";

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            query.Parameters.AddWithValue("@idT", idtienda.ToString());
            query.Parameters.AddWithValue("@user", username);

            var listaVentas = new List<Venta>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var venta = new Venta();
                    if (mayoreo) {
                        venta.IdVenta = reader.IsDBNull("id_venta_mayoreo") ? 0 : reader.GetInt32("id_venta_mayoreo");
                        venta.IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente");
                        venta.Debe =
                            reader.IsDBNull("id_cliente")
                                ? 0
                                : reader.GetDecimal("id_cliente"); // TODO ver k pedo con este
                        venta.Comentarios = reader.IsDBNull("comentarios") ? "" : reader.GetString("comentarios");
                    }
                    else {
                        venta.IdVenta = reader.IsDBNull("id_venta_menudeo") ? 0 : reader.GetInt32("id_venta_menudeo");
                    }

                    venta.DetalleVenta = reader.IsDBNull("detalle_venta") ? "" : reader.GetString("detalle_venta");
                    venta.TotalVenta = reader.IsDBNull("total_venta") ? 0 : reader.GetDecimal("total_venta");
                    venta.Pago = reader.IsDBNull("pago") ? 0 : reader.GetDecimal("pago");
                    venta.Cambio = reader.IsDBNull("cambio") ? 0 : reader.GetDecimal("cambio");
                    venta.IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda");
                    venta.UsuarioRegistra =
                        reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra");
                    venta.FechaRegistro = reader.GetDateTime("fecha_registro");
                    listaVentas.Add(venta);
                }

                Log.Debug("Se han obtenido con exito las tablas de ventas");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de ventas.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 14: ha ocurrido un error al intentar obtener la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }

            return listaVentas;
        }
    }
}