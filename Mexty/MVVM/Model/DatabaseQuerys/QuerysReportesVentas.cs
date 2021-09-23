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

        public static List<Venta> GetVentasPorSucursal(int id, string comando) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var cmd = "";

            switch (comando) {
                case "hoy":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_menudeo
                            WHERE (date(fecha_registro) >= date_sub(now(), interval 1 day)) and id_tienda=@id";
                            //UNION
                            //SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            //FROM venta_mayoreo
                            //WHERE(date(fecha_registro) >= date_sub(now(), interval 1 day)) and id_tienda = @id";
                    break;
                case "semana":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_menudeo
                            WHERE (date(fecha_registro) >= date_sub(now(), interval 1 week)) and id_tienda=@id";
                            //UNION
                            //SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            //FROM venta_mayoreo
                            //WHERE(date(fecha_registro) >= date_sub(now(), interval 1 week)) and id_tienda = @id";
                    break;
                case "mes":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_menudeo
                            WHERE (date(fecha_registro) >= date_sub(now(), interval 1 month)) and id_tienda=@id";
                            //UNION
                            //SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            //FROM venta_mayoreo
                            //WHERE(date(fecha_registro) >= date_sub(now(), interval 1 month)) and id_tienda = @id";
                    break;
            }

            var conn = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            conn.Parameters.AddWithValue("id", id.ToString());

            var items = new List<Venta>();
            try {
                using var reader = conn.ExecuteReader();    
                while (reader.Read()) {
                    var item = new Venta() {
                        //IdVenta = reader.IsDBNull("id_venta_menudeo") ? 0 : reader.GetInt32("id_venta_menudeo"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        TotalVenta = reader.IsDBNull("total_venta") ? 0 : reader.GetDecimal("total_venta"),
                        DetalleVenta = reader.IsDBNull("detalle_venta") ? "" : reader.GetString("detalle_venta"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? DateTime.Now : reader.GetDateTime("fecha_registro"),
                    };

                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario de venta menudeo.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario venta menudeo.");
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
            return items;
        }

        public static List<Venta> GetVentasPorUsuario(string username, string comando) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var cmd = "";

            switch (comando) {
                case "hoy":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_menudeo
                            WHERE(date(fecha_registro) >= date_sub(now(), interval 1 day)) and usuario_registra = @username
                            UNION
                            SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_mayoreo
                            WHERE(date(fecha_registro) >= date_sub(now(), interval 1 day)) and usuario_registra = @username";
                    break;
                case "semana":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_menudeo
                            WHERE(date(fecha_registro) >= date_sub(now(), interval 1 week)) and usuario_registra = @username
                            UNION
                            SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_mayoreo
                            WHERE(date(fecha_registro) >= date_sub(now(), interval 1 week)) and usuario_registra = @username";
                    break;
                case "mes":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_menudeo
                            WHERE(date(fecha_registro) >= date_sub(now(), interval 1 month)) and usuario_registra = @username
                            UNION
                            SELECT usuario_registra, fecha_registro, detalle_venta, total_venta
                            FROM venta_mayoreo
                            WHERE(date(fecha_registro) >= date_sub(now(), interval 1 month)) and usuario_registra = @username";
                    break;
            }

            var conn = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            conn.Parameters.AddWithValue("username", username);

            var items = new List<Venta>();
            try {
                using var reader = conn.ExecuteReader();
                while (reader.Read()) {
                    var item = new Venta() {
                        //IdVenta = reader.IsDBNull("id_venta_menudeo") ? 0 : reader.GetInt32("id_venta_menudeo"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        TotalVenta = reader.IsDBNull("total_venta") ? 0 : reader.GetDecimal("total_venta"),
                        DetalleVenta = reader.IsDBNull("detalle_venta") ? "" : reader.GetString("detalle_venta"),
                    };

                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario de venta menudeo.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario venta menudeo.");
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
            return items;
        }

        public static List<Venta> GetVentasMayoreo(int idTienda, string comando) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var cmd = "";

            switch (comando) {
                case "hoy":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta, id_cliente
                            FROM venta_mayoreo
                            WHERE (date(fecha_registro) >= date_sub(now(), interval 1 day)) and id_tienda=@id
                            order by month(fecha_registro) ASC, day(fecha_registro) ASC, id_cliente ASC";
                    break;
                case "semana":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta, id_cliente
                            FROM venta_mayoreo
                            WHERE (date(fecha_registro) >= date_sub(now(), interval 1 week)) and id_tienda=@id
                            order by month(fecha_registro) ASC, day(fecha_registro) ASC, id_cliente ASC";
                    break;
                case "mes":
                    cmd = @"SELECT usuario_registra, fecha_registro, detalle_venta, total_venta, id_cliente
                            FROM venta_mayoreo
                            WHERE (date(fecha_registro) >= date_sub(now(), interval 1 month)) and id_tienda=@id
                            order by month(fecha_registro) ASC, day(fecha_registro) ASC, id_cliente ASC";
                    break;
            }

            var conn = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            conn.Parameters.AddWithValue("id", idTienda.ToString());


            var items = new List<Venta>();

            try {
                using var reader = conn.ExecuteReader();
                while (reader.Read()) {
                    var item = new Venta() {
                        //IdVenta = reader.IsDBNull("id_venta_menudeo") ? 0 : reader.GetInt32("id_venta_menudeo"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        TotalVenta = reader.IsDBNull("total_venta") ? 0 : reader.GetDecimal("total_venta"),
                        DetalleVenta = reader.IsDBNull("detalle_venta") ? "" : reader.GetString("detalle_venta"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? DateTime.Now : reader.GetDateTime("fecha_registro"),
                        IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente")
                    };

                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario de venta mayoreo.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario venta mayoreo.");
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
            return items;

        }
    }
}