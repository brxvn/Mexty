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

        /// <summary>
        /// Métoddo que lee si matriz esta activada en esta sucursal.
        /// </summary>
        /// <returns></returns>
        public static bool ChekMatrizEnabled() {
            try {
                var myIni = new IniFile(@"C:\Mexty\Settings.ini");
                var enabled = myIni.Read("Matriz");
                var matrizEnabled = int.Parse(enabled);

                if (matrizEnabled <= 1) {
                    // Es un valor entre 0 y 1 si no, retornamos.
                    return matrizEnabled switch {
                        1 => true,
                        0 => false,
                        _ => false
                    };
                }
                else {
                    throw new Exception(
                        "Se ha dado un valor no valido a el campo de Matriz en el archivo de configuración");
                }

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al leer el campo de Matriz de el archivo de configuración.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    "Error 10.5: Ha ocurrido un error al leer el campo de Matriz del arhivo de configuración, este solo acepta valores de 0 o 1. Ejemplo: Matriz=1 (Para señalizar que esta sucursal debe se ser tratada como matriz y habilitar las ventas Mayoreo).",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}