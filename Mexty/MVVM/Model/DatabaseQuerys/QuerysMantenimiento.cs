using System;
using System.Windows;
using log4net;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que contiene las querys que se ocupan para el mantenimiento de la base de datos.
    /// </summary>
    public static class QuerysMantenimiento {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Método que activa la depuración de las tablas de Ventas.
        /// </summary>
        public static void DepCamposVentas() {
            Log.Info("Se ha empezado la limpieza regular de las tablas de Ventas.");

            const int numRegistrosMinParaDepurar = 400; // La cantidad de registros que debe de tener las tablas para que se active la depuración.
            const int numRegistrosADepurar = 100; // La cantidad de registros que se borrarán en el caso de que se active la depuración.

            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var numberCheckMenudeo = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select count(*) as cantidad from venta_menudeo"
            };

            var numberCheckMayoreo = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select count(*) as cantidad from venta_mayoreo"
            };

            var delMenudeo = new MySqlCommand() {
                Connection = connObj,
                CommandText = $"delete from venta_menudeo order by ID_VENTA_MENUDEO limit {numRegistrosADepurar.ToString()}"
            };

            var delMayoreo = new MySqlCommand() {
                Connection = connObj,
                CommandText = $"delete from venta_mayoreo order by ID_VENTA_MAYOREO limit {numRegistrosADepurar.ToString()}"
            };

            try {
                // Hacemos las querys para ver la cantidad de registros en ventas
                var resQueryCheckMayoreo = numberCheckMayoreo.ExecuteScalar();
                var resQueryCheckMenudeo = numberCheckMenudeo.ExecuteScalar();

                // leemos los resultados de ventas mayoreo
                var numeroDeRegistrosMayoreo = Convert.ToInt32(resQueryCheckMayoreo);

                if (numeroDeRegistrosMayoreo >= numRegistrosMinParaDepurar) {
                    var numRegEliminados = delMayoreo.ExecuteNonQuery();
                    Log.Info(
                        $"Se han eliminado los {numRegEliminados.ToString()} registros más antiguos de la tabla de ventas mayoreo como parte de la limpieza regular del sistema.");
                }

                var numeroDeRegistrosMenudeo = Convert.ToInt32(resQueryCheckMenudeo);

                if (numeroDeRegistrosMenudeo >= numRegistrosMinParaDepurar) {
                    var numRegEliminados = delMenudeo.ExecuteNonQuery();
                    Log.Info(
                        $"Se han eliminado los {numRegEliminados.ToString()} registros más antiguos de la tabla de ventas menudeo como parte de la limpieza regular del sistema.");
                }

                Log.Info("Se ha terminado el proceso de depuración");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al hacer el proceso de depuración de las tablas de ventas.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show("Ha ocurrido un error al hacer la depuración programada de la Base de datos, contacte al administrador.");
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que depura los campos de la tabla control_imports
        /// </summary>
        public static void DepCamposImport() {
            Log.Info("Se ha iniciado la depuración de los campos de import.");

            const int numRegistrosMinParaDepurar = 300; // La cantidad de registros que debe de tener las tablas para que se active la depuración.

            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var numberCheckImport = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select count(*) as cantidad from control_imports"
            };

            var delImports = new MySqlCommand() {
                Connection = connObj,
                CommandText = "delete from venta_menudeo order by ID_VENTA_MENUDEO limit 100"
            };

            try {
                var resQueryNumberCheckImport = numberCheckImport.ExecuteScalar();

                var numeroDeRegistrosImport = Convert.ToInt32(resQueryNumberCheckImport);

                if (numeroDeRegistrosImport >= numRegistrosMinParaDepurar) {
                    var numRegEliminados = delImports.ExecuteNonQuery();
                    Log.Info(
                        $"Se han eliminado los {numRegEliminados.ToString()} registros más antiguos de la tabla de contorl_sincdb como parte de la limpieza regular del sistema.");
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al hacer el proceso de depuración de las tablas de sinc_bd.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    "Ha ocurrido un error al hacer la depuración programada de la Base de datos, contacte al administrador.");
            }
            finally {
                connObj.Close();
            }
        }
    }
}