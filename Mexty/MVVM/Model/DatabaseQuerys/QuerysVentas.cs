using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que se encarga de hacer las consultas a la base de datos del modulo de ventas.
    /// </summary>
    public static class QuerysVentas {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Método para obtener las tablas de la tabla de ventas ya sea mayoreo o menundeo.
        /// </summary>
        /// <param name="mayoreo">Si <c>true</c> obtiene las tablas de ventas-mayoreo.</param>
        /// <returns> Una lista de objetos tipo <c>Venta</c>.</returns>
        public static List<Venta> GetTablesFromVentas(bool mayoreo = false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = mayoreo ? @"select * from venta_mayoreo" : @"select * from venta_menudeo";

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            var listaVentas = new List<Venta>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var venta = new Venta();
                    if (mayoreo) {
                        // Mayoreo
                        venta.IdVenta = reader.IsDBNull("id_venta_mayoreo") ? 0 : reader.GetInt32("id_venta_mayoreo");
                        venta.IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente");
                        venta.Debe =
                            reader.IsDBNull("id_cliente")
                                ? 0
                                : reader.GetDecimal("id_cliente"); // TODO ver k pedo con este
                        venta.Comentarios = reader.IsDBNull("comentarios") ? "" : reader.GetString("comentarios");
                    }
                    else {
                        // Menudeo
                        venta.IdVenta = reader.IsDBNull("id_venta_menudeo") ? 0 : reader.GetInt32("id_venta_menudeo");
                    }

                    // Generales
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


        /// <summary>
        /// Método para actualizar los datos de ventas.
        /// </summary>
        /// <param name="venta">El objeto tipo <c>Venta</c> a guardar.</param>
        /// <param name="mayoreo">Bandera par guardar en inventario maoreo o menudeo if <c>true</c> guarda a mayoreo</param>
        /// <returns></returns>
        public static int UpdateData(Venta venta, bool mayoreo = false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = mayoreo ? @"
                update venta_mayoreo 
                set ID_CLIENTE=@idClienteX, 
                    DETALLE_VENTA=@detalle, TOTAL_VENTA=@totalX, 
                    PAGO=@pagoX, CAMBIO=@cambioX, DEBE=@debeX, 
                    COMENTARIOS=@comentarios, ID_TIENDA=@idTiendaX, 
                    USUARIO_REGISTRA=@usrRegistra, FECHA_REGISTRO=@fechaRegistro 
                where ID_VENTA_MAYOREO = @idX"
                : @"
                update venta_menudeo 
                set DETALLE_VENTA=@detalle, TOTAL_VENTA=@totalX, 
                    PAGO=@pagoX, CAMBIO=@cambioX, 
                    ID_TIENDA=@idTiendaX, 
                    USUARIO_REGISTRA=@usrRegistra, FECHA_REGISTRO=@fechaRegistro 
                where ID_VENTA_MENUDEO = @idX";

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            query.Parameters.AddWithValue("@idX", venta.IdVenta.ToString());
            query.Parameters.AddWithValue("@idClienteX", venta.IdCliente.ToString());
            query.Parameters.AddWithValue("@detalle", venta.DetalleVenta);
            query.Parameters.AddWithValue("@totalX", venta.TotalVenta.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@pagoX", venta.Pago.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@cambioX", venta.Cambio.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@debeX", venta.Debe.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@comentarios", venta.Comentarios);
            query.Parameters.AddWithValue("@idTiendaX", venta.IdTienda.ToString());
            query.Parameters.AddWithValue("@usrRegistra", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@fechaRegistro", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se han actualizado los datos de ventas de manera exitosa.");
                return res;

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al acualizar los datos de ventas.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 15: ha ocurrido un error al intentar guardar la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }
        }


        /// <summary>
        /// Método que registra un nuevo item en la tabla de ventas.
        /// </summary>
        /// <param name="newVenta">El objeto tipo <c>Venta</c> a dar de alta.</param>
        /// <param name="mayoreo">Bandera <c>true</c> para guardar en ventas mayoreo.</param>
        /// <returns>El numero de tablas afectadas.</returns>
        public static int NewItem(Venta newVenta, bool mayoreo = false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = mayoreo
                ? @"
                insert into venta_mayoreo 
                    (ID_VENTA_MAYOREO, ID_CLIENTE, DETALLE_VENTA, 
                     TOTAL_VENTA, PAGO, CAMBIO, DEBE, 
                     COMENTARIOS, ID_TIENDA, 
                     USUARIO_REGISTRA, FECHA_REGISTRO) 
                values (default, @idCX, @detalle, 
                        @totalX, @pagoX, @cambioX, @debeX, 
                        @comentarios, @idTX, 
                        @usrRegistra, @fechaRegistro)"
                : @"
                insert into venta_menudeo 
                    (ID_VENTA_MENUDEO, DETALLE_VENTA, 
                     TOTAL_VENTA, PAGO, CAMBIO, 
                     ID_TIENDA, 
                     USUARIO_REGISTRA, FECHA_REGISTRO) 
                values (default, @detalle, 
                        @totalX, @pagoX, @cambioX, 
                        @idTX, 
                        @usrRegistra, @fechaRegistro)";

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            query.Parameters.AddWithValue("@idCX", newVenta.IdCliente.ToString());
            query.Parameters.AddWithValue("@detalle", newVenta.DetalleVenta);
            query.Parameters.AddWithValue("@totalX", newVenta.TotalVenta.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@pagoX", newVenta.Pago.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@cambioX", newVenta.Cambio.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@debeX", newVenta.Debe.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@comentario", newVenta.Comentarios);
            query.Parameters.AddWithValue("@idTX", newVenta.IdTienda.ToString());
            query.Parameters.AddWithValue("@usrRegistra", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@fechaRegistro", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha dado de alta una nueva venta de manera exitosa.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta una nueva venta.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 15: ha ocurrido un error al intentar guardar la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }
        }

        public static List<ItemInventario> GetListaInventarioVentasMenudeo() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                SELECT c.ID_PRODUCTO, c.TIPO_PRODUCTO,
                       c.NOMBRE_PRODUCTO, c.PRECIO_MAYOREO,
                       c.PRECIO_MENUDEO, c.ACTIVO,
                       c.DEPENDENCIAS, c.ESPECIFICACION_PRODUCTO, 
                    i.CANTIDAD
                FROM   cat_producto c, inventario i
                WHERE  c.ID_PRODUCTO = i.ID_PRODUCTO"
            };

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        //Dependencias = reader.IsDBNull("dependencias") ? 0 : reader.GetInt32("dependencias"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        PrecioMenudeo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetDecimal("precio_menudeo"),
                        PrecioMayoreo = reader.IsDBNull("precio_mayoreo") ? 0 : reader.GetDecimal("precio_mayoreo"),
                        //Comentario = reader.IsDBNull("especificacion_producto") ? "" : reader.GetString("especificacion_producto"),
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
    }
}