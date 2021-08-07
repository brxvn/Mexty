using System;
using System.Windows;
using log4net;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que se encarga de leer del ini.
    /// </summary>
    public static class IniFields {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// String de conección para acceder a la base de datos.
        /// </summary>
        private static string ConnectionString { get; set; }

        /// <summary>
        /// Método que Lee el contenido del archivo ini para la connección.
        /// </summary>
        /// <returns>String con la información de log-in a la base de datos</returns>
        public static string ReadConnectionInfo() {
            try {
                var myIni = new IniFile(@"C:\Mexty\Settings.ini");
                var user = myIni.Read("DbUser");
                var pass = myIni.Read("DbPass");
                var connString = $"server=localhost; database=mexty; Uid={user}; pwd={pass}";

                Log.Debug("Se han leido las credenciales del ini exitosamente.");
                ConnectionString = connString;
                return connString;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al leer los campos de conexión del archivo de configuración.");
                Log.Error($"Error: {e.Message}");

                MessageBox.Show(
                    "Error 11: ha ocurrido un error al leer las credenciales de la base de datos en el archivo de configuración.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Método que retorna el string de conección una vez este se ha leido del ini.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString() {
            return ConnectionString;
        }

        /// <summary>
        /// Método que obtiene el Id de la tienda en la que se ejecuta el programa leyendolo desde el ini.
        /// </summary>
        /// <returns></returns>
        public static int GetIdTiendaActual() {
            try {
                var myIni = new IniFile(@"C:\Mexty\Settings.ini");
                var sucursal = myIni.Read("IdTienda");
                var idTienda = int.Parse(sucursal);
                if (idTienda == 0) throw new Exception(); // No puede existir el id 0, something went wrong!!
                Log.Info("Se ha leido exitosamente el id de la tienda del ini.");

                return idTienda;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al leer el Id de la tienda en el ini.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    "Error 10: Ha ocurrido un error al leer el id de tienda en el archivo de configuración.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}