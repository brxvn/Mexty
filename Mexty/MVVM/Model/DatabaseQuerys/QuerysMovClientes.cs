using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que se encarga de las consultas a base de datos del modulo de movimientos clientes.
    /// </summary>
    public class QuerysMovClientes {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Método que obtiene el contenido de la tabla de movimientos clientes.
        /// </summary>
        /// <returns></returns>
        public static List<LogCliente> GetTablesFromMovClientes(int idCliente) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var querry = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from movimientos_clientes where ID_CLIENTE=@id"
            };

            querry.Parameters.AddWithValue("@id", idCliente.ToString());

            var listaLogs = new List<LogCliente>();

            try {
                using var reader = querry.ExecuteReader();
                while (reader.Read()) {
                    var log = new LogCliente() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente"),
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
                Log.Error("Ha ocurrido un error al obtener las tablas de movimientos clientes.");
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
        /// Método que da de alta un nuevo log en movimientos cliente.
        /// </summary>
        /// <param name="newLog"></param>
        /// <returns></returns>
        public static int NewLogCliente(LogCliente newLog) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                insert into movimientos_clientes 
                    (ID_REGISTRO, ID_CLIENTE, MENSAJE, USUARIO_REGISTRA, FECHA_REGISTRO)
                values (default, @idX ,@mensaje, @usuarioReg, @fechaReg)"
            };

            query.Parameters.AddWithValue("@idX", newLog.IdCliente.ToString());
            query.Parameters.AddWithValue("@mensaje", newLog.Mensaje);
            query.Parameters.AddWithValue("@usuarioReg", newLog.UsuarioRegistra);
            query.Parameters.AddWithValue("@fechaReg", newLog.FechaRegistro);

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha dado de alta un nuevo log en movimientos cliente.");
                return res;

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevoe log en movimientos cliente.");
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