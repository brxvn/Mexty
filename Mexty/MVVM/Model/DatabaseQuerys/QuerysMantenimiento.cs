using System;
using log4net;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que contiene las querys que se ocupan para el mantenimiento de la base de datos.
    /// </summary>
    public class QuerysMantenimiento {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public static bool LimpiarVentas() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();


            var numberCheckMenudeo = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select count(*) from venta_menudeo"
            };

            var numberCheckMayoreo = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select count(*) from venta_mayoreo"
            };

            var delMenudeo = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"delete from venta_menudeo order by FECHA_REGISTRO ASC limit 100"
            };

            var delMayoreo = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"delete from venta_mayoreo order by FECHA_REGISTRO ASC limit 100"
            };

            try {
                return true;

            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}