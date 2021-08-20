using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que contiene las consultas a la base de datos del modulo de administración de sucursales.
    /// </summary>
    public class QuerysSucursales {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        
        /// <summary>
        /// Función que obtiene las tablas de las Sucursales.
        /// </summary>
        /// <returns>Una lista de objetos tipo <c>Sucursal</c>.</returns>
        public static List<Sucursal> GetTablesFromSucursales() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cat_tienda"
            };

            var sucursales = new List<Sucursal>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var sucursal = new Sucursal {
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                        NombreTienda = reader.IsDBNull("nombre_tienda") ? "" : reader.GetString("nombre_tienda"),
                        Dirección = reader.IsDBNull("direccion") ? "" : reader.GetString("direccion"),
                        Telefono = reader.IsDBNull("telefono") ? "" : reader.GetString("telefono"),
                        Rfc = reader.IsDBNull("rfc") ? "" : reader.GetString("rfc"),
                        Mensaje = reader.IsDBNull("mensaje") ? "" : reader.GetString("mensaje"),
                        Facebook = reader.IsDBNull("facebook") ? "" : reader.GetString("facebook"),
                        Instagram = reader.IsDBNull("instagram") ? "" : reader.GetString("instagram"),
                        TipoTienda = reader.IsDBNull("tipo_tienda") ? "" : reader.GetString("tipo_tienda"),
                    };
                    sucursales.Add(sucursal);
                    Log.Debug("Se han obtenido con exito las tablas de sucursales.");
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de surcursales.");
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

            return sucursales;
        }

        /// <summary>
        /// Método que actualiza una sucursal en la base de datos
        /// </summary>
        /// <param name="sucursal"></param>
        public static int UpdateData(Sucursal sucursal) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cat_tienda 
                set NOMBRE_TIENDA=@nom, 
                    DIRECCION=@dir, 
                    TELEFONO=@telX, 
                    RFC=@rfc, 
                    MENSAJE=@msg, 
                    FACEBOOK=@face, 
                    INSTAGRAM=@inst,
                    TIPO_TIENDA=@suc 
                where ID_TIENDA=@idX"
            };

            query.Parameters.AddWithValue("@nom", sucursal.NombreTienda);
            query.Parameters.AddWithValue("@dir", sucursal.Dirección);
            query.Parameters.AddWithValue("@telX", sucursal.Telefono);
            query.Parameters.AddWithValue("@rfc", sucursal.Rfc);
            query.Parameters.AddWithValue("@msg", sucursal.Mensaje);
            query.Parameters.AddWithValue("@face", sucursal.Facebook);
            query.Parameters.AddWithValue("@inst", sucursal.Instagram);
            query.Parameters.AddWithValue("@suc", sucursal.TipoTienda);
            query.Parameters.AddWithValue("@idX", sucursal.IdTienda.ToString());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha actualizado la sucursal exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar la sucursal.");
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
        /// Método que registra una nueva sucursal.
        /// </summary>
        /// <param name="newSucursal"></param>
        public static int NewSucursal(Sucursal newSucursal) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                insert into cat_tienda 
                    (ID_TIENDA, NOMBRE_TIENDA, DIRECCION, TELEFONO, 
                     RFC, 
                     MENSAJE, 
                     FACEBOOK, INSTAGRAM, TIPO_TIENDA) 
                values (default, @nom, @dir, @telX, 
                        @rfc, 
                        @msg, 
                        @face, @insta, @tTienda)"
            };
            query.Parameters.AddWithValue("@nom", newSucursal.NombreTienda);
            query.Parameters.AddWithValue("@dir", newSucursal.Dirección);
            query.Parameters.AddWithValue("@telX", newSucursal.Telefono);
            query.Parameters.AddWithValue("@rfc", newSucursal.Rfc);
            query.Parameters.AddWithValue("@msg", newSucursal.Mensaje);
            query.Parameters.AddWithValue("@face", newSucursal.Facebook);
            query.Parameters.AddWithValue("@insta", newSucursal.Instagram);
            query.Parameters.AddWithValue("@tTienda", newSucursal.TipoTienda);

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
                Log.Info("Se ha creado una nueva sucursal exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta una nueva sucursal.");
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
        /// Método que elimina una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public static int DeleteSuc(int idSucursal) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"delete from cat_tienda where ID_TIENDA=@idX"
            };

            query.Parameters.AddWithValue("@idX", idSucursal.ToString());


            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha eliminado una sucursal de manera exitosa.");
                return res;

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al eliminar los datos de la sucursal.");
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