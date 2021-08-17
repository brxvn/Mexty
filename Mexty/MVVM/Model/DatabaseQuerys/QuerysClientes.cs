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
    /// Clase que se encarga de hacer las consultas a la base de datos del modulo de clientes.
    /// </summary>
    public static class QuerysClientes {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);


        /// <summary>
        /// Método para obtener todos los datos de la tabla de clientes Mayoreo.
        /// Si se le da true como parametero solo manda los clientes activos.
        /// </summary>
        /// <returns>Una lista de elementos tipo <c>Cliente</c>.</returns>
        public static List<Cliente> GetTablesFromClientes(bool activos=false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var cmd = activos ? @"select * from cliente_mayoreo where ACTIVO=1" : @"select * from cliente_mayoreo";

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = cmd
            };

            var clientes = new List<Cliente>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var cliente = new Cliente() {
                        IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente"),
                        Nombre = reader.IsDBNull("nombre_cliente") ? "" : reader.GetString("nombre_cliente"),
                        ApPaterno = reader.IsDBNull("ap_paterno") ? "" : reader.GetString("ap_paterno"),
                        ApMaterno = reader.IsDBNull("ap_materno") ? "" : reader.GetString("ap_materno"),
                        Domicilio = reader.IsDBNull("domicilio") ? "" : reader.GetString("domicilio"),
                        Telefono = reader.IsDBNull("telefono") ? "" : reader.GetString("telefono"),
                        Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                        UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                        FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        Debe = reader.IsDBNull("debe") ? 0 : reader.GetFloat("debe")
                    };
                    clientes.Add(cliente);
                }
                Log.Debug("Se han obtenido con exito las tablas de clientes.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de clientes.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 14: ha ocurrido un error al intentar obtener la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;

            }
            finally{
                connObj.Close();
            }

            return clientes;
        }

        /// <summary>
        /// Método para actualizar los datos del Cliente.
        /// </summary>
        /// <param name="cliente"></param>
        public static int UpdateData(Cliente cliente) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cliente_mayoreo 
                set NOMBRE_CLIENTE=@nom, AP_PATERNO=@apP, AP_MATERNO=@apM, 
                    DOMICILIO=@dom, TELEFONO=@telX, ACTIVO=@actX, 
                    USUARIO_MODIFICA=@usMod, FECHA_MODIFICA=@date, 
                    COMENTARIO=@com, DEBE=@debeX
                where ID_CLIENTE=@idX"
            };
            query.Parameters.AddWithValue("@nom", cliente.Nombre);
            query.Parameters.AddWithValue("@apP", cliente.ApPaterno);
            query.Parameters.AddWithValue("@apM", cliente.ApMaterno);
            query.Parameters.AddWithValue("@dom", cliente.Domicilio);
            query.Parameters.AddWithValue("@telX", cliente.Telefono);
            query.Parameters.AddWithValue("@actX", cliente.Activo.ToString());
            query.Parameters.AddWithValue("@usMod", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@idX", cliente.IdCliente.ToString());
            query.Parameters.AddWithValue("@com", cliente.Comentario);
            query.Parameters.AddWithValue("@debeX", cliente.Debe.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se han actualizado los datos de cliente de manera exitosa.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar los datos de cliente.");
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
        /// Método que registra un nuevo cliente.
        /// </summary>
        /// <param name="newClient"></param>
        public static int NewClient(Cliente newClient) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"
                insert into cliente_mayoreo 
                    (id_cliente, nombre_cliente, ap_paterno, ap_materno, 
                     domicilio, telefono, activo, 
                     usuario_registra, fecha_registro, 
                     usuario_modifica, fecha_modifica, 
                     comentario, DEBE) 
                values (default, @nom, @apP, @apM, 
                        @dom, @telX, @actX, 
                        @usReg, @date, 
                        @usMod, @date, 
                        @com, @debeX)"
            };
            query.Parameters.AddWithValue("@nom", newClient.Nombre);
            query.Parameters.AddWithValue("@apP", newClient.ApPaterno);
            query.Parameters.AddWithValue("@apM", newClient.ApMaterno);
            query.Parameters.AddWithValue("@dom", newClient.Domicilio);
            query.Parameters.AddWithValue("@telX", newClient.Telefono);
            query.Parameters.AddWithValue("@actX", 1.ToString());
            query.Parameters.AddWithValue("@usReg", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@usMod", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@com", newClient.Comentario);
            query.Parameters.AddWithValue("@debeX", newClient.Debe.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
                Log.Info("Se ha dado de alta un nuevo cliente de manera exitosa.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo cliente.");
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