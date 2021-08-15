using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {

    /// <summary>
    /// Clase que contiene las consultas a la base de datos de las tablas de Rol.
    /// </summary>
    public static class QuerysRol {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);


        /// <summary>
        /// Función que obtiene las tablas de los Roles.
        /// </summary>
        /// <returns>Una lista con objetos tipo <c>Rol</c>.</returns>
        public static List<Rol> GetTablesFromRoles() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var querry = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cat_rol_usuario"
            };

            var roles = new List<Rol>();
            try {
                using var reader = querry.ExecuteReader();
                while (reader.Read()) {
                    var rol = new Rol() {
                        IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32("id_rol"),
                        RolDescription = reader.IsDBNull("desc_rol") ? "" : reader.GetString("desc_rol"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda")
                    };
                    roles.Add(rol);
                }

                Log.Debug("Se han obtenido con exito las tablas de roles.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de roles.");
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

            return roles;
        }
    }
}