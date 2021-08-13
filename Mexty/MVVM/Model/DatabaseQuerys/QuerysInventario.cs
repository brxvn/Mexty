using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que se encarga de hacer las consultas a la base de datos del modulo de Inventario.
    /// </summary>
    /// TODO: Bloated, quitar querys redundantes y separar.
    public static class QuerysInventario {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Método para optener las tablas de inventario.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInventario> GetTablesFromInventario() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select * from inventario where ID_TIENDA=@idX"
            };
            query.Parameters.AddWithValue("@idX", IniFields.GetIdTiendaActual().ToString());

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                        UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                        FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica")
                    };
                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario.");
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


        /// <summary>
        /// Método para optener las tablas de inventario_matriz.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInventario> GetTablesFromInventarioMatrix() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                SELECT p.id_producto, 
                       p.tipo_producto, 
                       p.nombre_producto, 
                       p.medida, 
                       i.ID_REGISTRO, 
                       i.piezas, 
                       i.cantidad, 
                       i.comentario 
                FROM   cat_producto p, 
                       inventario_matriz i 
                WHERE  p.id_producto = i.id_producto"
            };

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        Medida = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                    };
                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario-producto matriz.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario-producto matriz.");
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


        /// <summary>
        /// Función que obtiene las tablas de el log de movimientos inventario matriz.
        /// </summary>
        /// <returns></returns>
        /// TODO: Quiza esto moverlo a otro lado
        public static List<LogInventario> GetTablesFromMovimientos() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select * from movimientos_inventario"
            };

            var logs = new List<LogInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new LogInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        Mensaje = reader.IsDBNull("mensaje") ? "" : reader.GetString("mensaje"),
                        FechaRegistro = reader.GetDateTime("fecha_registro"),
                        UsuarioRegistra = reader.IsDBNull("USUARIO_REGISTRA") ? "" : reader.GetString("USUARIO_REGISTRA"),
                    };
                    logs.Add(item);
                }
                Log.Debug("Se han obtenido los movimientos del inventario de matriz.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de movimientos_inventario.");
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

            return logs;
        }

        /// <summary>
        /// Método que obtiene el inventario de una tienda en especifico.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInventario> GetItemsFromInventarioById(int idTienda) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                SELECT p.id_producto, 
                       p.tipo_producto, 
                       p.nombre_producto, 
                       p.medida, 
                       i.ID_REGISTRO, 
                       i.piezas, 
                       i.cantidad, 
                       i.comentario, 
                       i.ID_TIENDA 
                FROM   cat_producto p, 
                       inventario i 
                WHERE  p.id_producto = i.id_producto 
                       and i.ID_TIENDA=@idTX"
            };
            query.Parameters.AddWithValue("@idTX", idTienda.ToString());

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        Medida = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                    };
                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario-productos.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario-productos.");
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

        /// <summary>
        /// Método para obtener la información conjunta de inventario y de cat_producto.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInventario> GetItemsFromInventario() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                SELECT p.id_producto, 
                       p.tipo_producto, 
                       p.nombre_producto, 
                       p.medida, 
                       i.ID_REGISTRO, 
                       i.piezas, 
                       i.cantidad, 
                       i.comentario, 
                       i.ID_TIENDA 
                FROM   cat_producto p, 
                       inventario i 
                WHERE  p.id_producto = i.id_producto 
                       and i.ID_TIENDA=@idTX"
            };
            query.Parameters.AddWithValue("@idTX", IniFields.GetIdTiendaActual().ToString());

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        Medida = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                    };
                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario-productos.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario-productos.");
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

        /// <summary>
        /// Método para actualizar los datos de la tabla inventario-general.
        /// </summary>
        /// <param name="item">El objeto tipo <c>ItemInventario</c> a guardar.</param>
        /// <param name="matriz">Bandera para guardar a inventario de matriz if <c>true</c> guarda a matriz.</param>
        public static int UpdateData(ItemInventario item, bool matriz = false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = matriz ? @"
                update inventario_matriz 
                set ID_PRODUCTO=@idPX, 
                    CANTIDAD=@cantidadX, PIEZAS=@piezasX, 
                    COMENTARIO=@comentario,
                    USUARIO_MODIFICA=@usrM, FECHA_MODIFICA=@date 
                where ID_REGISTRO=@idRX"
                :
                @"
                update inventario 
                set ID_PRODUCTO=@idPX, 
                    CANTIDAD=@cantidadX, PIEZAS=@piezasX, 
                    COMENTARIO=@comentario, ID_TIENDA=@idTX, 
                    USUARIO_MODIFICA=@usrM, FECHA_MODIFICA=@date 
                where ID_REGISTRO=@idRX";

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            query.Parameters.AddWithValue("@idRX", item.IdRegistro.ToString());
            query.Parameters.AddWithValue("@idPX", item.IdProducto.ToString());
            query.Parameters.AddWithValue("@tipo", item.TipoProducto);
            query.Parameters.AddWithValue("@med", item.Medida);
            query.Parameters.AddWithValue("@cantidadX", item.Cantidad.ToString());
            query.Parameters.AddWithValue("@piezasX", item.Piezas.ToString());
            query.Parameters.AddWithValue("@comentario", item.Comentario);
            query.Parameters.AddWithValue("@idTX", IniFields.GetIdTiendaActual().ToString());
            query.Parameters.AddWithValue("@usrM", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                var msg = matriz ? "inventario_matriz" : "inventario-general";
                Log.Info($"Se han actualizado los datos de la tabla {msg} de manera exitosa.");
                return res;
            }
            catch (Exception e) {
                var msg = matriz ? "inventario_matriz" : "inventario-general";
                Log.Error($"Ha ocurrido un error al actualizar los datos de {msg}.");
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
        /// Método que registra un nuevo item en la tabla de inventario-general.
        /// </summary>
        /// <param name="newItem"></param>
        public static int NewItem(ItemInventario newItem, bool matriz = false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = matriz
                ? @"
                insert into inventario_matriz 
                    (ID_REGISTRO, ID_PRODUCTO, 
                     CANTIDAD, PIEZAS, 
                     COMENTARIO, 
                     usuario_registra, fecha_registro, 
                     usuario_modifica, fecha_modifica) 
                values (default, @idPX, 
                        @cantidadX, @piezasX, 
                        @comentario, 
                        @usReg, @date, 
                        @usMod, @date)"
                : @"
                insert into inventario 
                    (ID_REGISTRO, ID_PRODUCTO, 
                     CANTIDAD, PIEZAS, 
                     COMENTARIO, ID_TIENDA, 
                     usuario_registra, fecha_registro, 
                     usuario_modifica, fecha_modifica) 
                values (default, @idPX, 
                        @cantidadX, @piezasX, 
                        @comentario, @idTX, 
                        @usReg, @date, 
                        @usMod, @date)";

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = cmd
            };

            query.Parameters.AddWithValue("@idPX", newItem.IdProducto.ToString());
            query.Parameters.AddWithValue("@cantidadX", newItem.Cantidad.ToString());
            query.Parameters.AddWithValue("@piezasX", newItem.Piezas.ToString());
            query.Parameters.AddWithValue("@comentario", newItem.Comentario);
            query.Parameters.AddWithValue("@idTX", IniFields.GetIdTiendaActual().ToString());
            query.Parameters.AddWithValue("@usReg", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@usMod", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha dado de alta un nuevo item en el inventario de manera exitosa.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo item en el inventario.");
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
        /// Método que guarda un nuevo registro de movimientos.
        /// </summary>
        /// <param name="newItem"> Un objeto tipo <c>LogInventario</c>.</param>
        /// <returns>La cantidad de columnas modificadas.</returns>
        public static int NewLog(LogInventario newItem) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"
                insert into movimientos_inventario 
                    (ID_REGISTRO, MENSAJE,
                     USUARIO_REGISTRA, FECHA_REGISTRO) 
                values (default, @mensaje,
                        @usMod, @date)"
            };

            query.Parameters.AddWithValue("@mensaje", newItem.Mensaje);
            query.Parameters.AddWithValue("@usMod", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha dado de alta un nuevo item en movimientos_inventario de manera exitosa.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo item en el movimientos_inventario.");
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
    }
}