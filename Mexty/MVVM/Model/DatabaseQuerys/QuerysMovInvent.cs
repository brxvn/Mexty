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
    /// Clase que se encarga de las consultas a base de datos del modulo de movimientos inventario.
    /// </summary>
    public static class QuerysMovInvent {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Método que obtiene el contenido de la tabla movimientos_inventario.
        /// </summary>
        /// <returns>Una lista con objetos tipo <c>LogInventario</c></returns>
        public static List<LogInventario> GetTablesFromMovInventario() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var querry = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from movimientos_inventario"
            };

            var listaLogs = new List<LogInventario>();

            try {
                using var reader = querry.ExecuteReader();
                while (reader.Read()) {
                    var log = new LogInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        Mensaje = reader.IsDBNull("mensaje") ? "" : reader.GetString("mensaje"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra")
                            ? ""
                            : reader.GetString("usuario_registra"),
                        FechaRegistro = reader.IsDBNull("fecha_registro")
                            ? Convert.ToDateTime("")
                            : reader.GetDateTime("fecha_registro"),
                    };
                    listaLogs.Add(log);
                }

                Log.Debug("Se han obtenido con exito las tablas de movimientos inventario.");
                return listaLogs;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de movimientos inventario.");
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
        }

        /// <summary>
        /// Método que da de alta un nuevo log en moviemientos inventario.
        /// </summary>
        /// <returns>El número de columnas afectadas por la querry.</returns>
        public static int NewLogInventario(LogInventario newLog) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                insert into movimientos_inventario 
                    (ID_REGISTRO, MENSAJE, USUARIO_REGISTRA, FECHA_REGISTRO) 
                values (default, @mensaje, @usuarioReg, @fechaReg)"
            };

            query.Parameters.AddWithValue("@mensaje", newLog.Mensaje);
            query.Parameters.AddWithValue("@usuarioReg", newLog.UsuarioRegistra);
            query.Parameters.AddWithValue("@fechaReg", newLog.FechaRegistro);

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha dado de alta un nuevo log en movimientos inventario.");
                return res;

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevoe log en movimientos inventario.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 15: ha ocurrido un error al intentar guardar la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}