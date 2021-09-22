using System;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase principal de Base de datos, se encarga de hacer el proceso de login y obtener los datos iniciales.
    /// </summary>
    public static class DatabaseInit {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        ///  El nombre de usuario de la persona loggeada.
        /// </summary>
        private static string Username { get; set; }

        /// <summary>
        /// La contraseña de la persona loggeada.
        /// </summary>
        private static string Password { get; set; }

        /// <summary>
        /// El rol de la persona loggeada.
        /// </summary>
        private static int IdRol { get; set; }

        /// <summary>
        /// La tienda asignada a la persona loggeada.
        /// </summary>
        private static int IdTienda { get; set; }

        /// <summary>
        /// El id de tienda leido desde el ini.
        /// </summary>
        private static int IdTiendaIni { get; set; }

        /// <summary>
        /// El nombre de la tienda escrito en el ini.
        /// </summary>
        private static string NombreTiendaInit { get; set; }

        /// <summary>
        /// <c>Bool</c> que guarda si el log-in fue exitoso.
        /// </summary>
        private static bool ConnectionSuccess { get; set; }

        /// <summary>
        /// <c>bool</c> true si este usuario tiene asignada matriz.
        /// </summary>
        private static bool MatrizAsigned { get; set; }

        /// <summary>
        /// <c>bool</c> true si la sucursal es matriz
        /// </summary>
        private static bool MatrizAsignedIni { get; set; }

        /// <summary>
        /// Método que se encarga del inicio de sesión al programa.
        /// </summary>
        /// <param name="username"><c>string</c> conteniendo el nombre de usuario.</param>
        /// <param name="password"><c>string</c> conteniendo la contraseña del usuario.</param>
        /// <returns><c>true</c> si el inicio de sesión fue exitoso.</returns>
        public static bool UserLogIn(string username, string password) {
            var connObj = new MySqlConnection(IniFields.ReadConnectionInfo());
            connObj.Open();

            var login = new MySqlCommand {
                Connection = connObj,
                CommandText = @"select usuario, contrasenia, id_rol, ID_TIENDA from usuario where USUARIO=@user and CONTRASENIA=@pass"
            };
            login.Parameters.AddWithValue("@user", username);
            login.Parameters.AddWithValue("@pass", password);

            try {
                var fistQuery = login.ExecuteReader();

                if (fistQuery.HasRows) {
                    InitializeFields(fistQuery);
                    ValidateIdTienda();
                    CheckMatriz();
                    Log.Info("Se ha iniciado sesión de manera exitosa.");
                    return true;
                }
                else {
                    Log.Info("Las credenciales de inicio de sesión no son correctas.");
                    return false;
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al hacer la consulta para iniciar sesión.");
                Log.Error($"Error: {e.Message}");
                throw;
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Inicializa los campos estaticos de la clase.
        /// </summary>
        private static void InitializeFields(MySqlDataReader firstQuery) {
            try {
                firstQuery.Read();

                // Obtenemos campos de interés para saber quien se loggeo.
                Username = firstQuery.IsDBNull("usuario") ? "" : firstQuery.GetString("usuario");
                IdRol = firstQuery.IsDBNull("id_rol") ? 0 : firstQuery.GetInt32("id_rol");
                Password = firstQuery.IsDBNull("contrasenia") ? "" : firstQuery.GetString("contrasenia");
                IdTienda = firstQuery.IsDBNull("id_tienda") ? 0 : firstQuery.GetInt32("id_tienda");

                if (Username != "" && Password != "" && IdRol != 0 && IdTienda != 0) {
                    ConnectionSuccess = true;
                    Log.Debug("Se han obtenido los datos para el log-in con exito.");
                }

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener los datos para el log-in.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 12: Ha ocurrido un error al validar las credenciales del proceso de autenticación. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que valida que el id de tienda leido del ini sea válido.
        /// </summary>
        private static void ValidateIdTienda() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select nombre_tienda from cat_tienda where ID_TIENDA=@idX"
            };

            try {
                var idTienda = IniFields.GetIdTiendaActual();

                query.Parameters.AddWithValue("@idX", idTienda.ToString());

                var res = query.ExecuteReader();

                if (!res.HasRows) {
                    Log.Error("No se ha podido validar el id de la tienda escrito en el ini.");
                    throw new Exception("Valor de IdTienda en ini invalido.");
                }
                else {
                    res.Read();
                    NombreTiendaInit = res.GetString("nombre_tienda");
                    IdTiendaIni = idTienda;
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al validar que el IdTienda sea valido.");
                Log.Error($"Error: {e.Message}");

                const MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show("Error 13: ha ocurrido un error al validar la sucursal del archivo de configuración.",
                    "Error", buttons, MessageBoxImage.Error);
                throw;
            }
            finally {
                connObj.Close();
            }
        }


        /// <summary>
        /// Método que checa si el usuario loggeado esta asignado a matriz.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void CheckMatriz(bool ini=false) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select ID_TIENDA from cat_tienda where TIPO_TIENDA='Matriz' and ID_TIENDA=@id"
            };
            if (!ini) {
                query.Parameters.AddWithValue("@id", IdTienda.ToString());
            }
            else {
                query.Parameters.AddWithValue("@id", IdTiendaIni.ToString());
            }

            try {
                var res = query.ExecuteReader();

                if (!res.HasRows) {
                    if (!ini) {
                        MatrizAsigned = false;
                    }
                    else {
                        MatrizAsignedIni = false;
                    }
                    Log.Info("Este usuario no esta asignado a matriz.");
                }
                else {
                    if (!ini) {
                        MatrizAsigned = true;
                    }
                    else {
                        MatrizAsignedIni = true;
                    }
                }

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al validar el id tienda asignado al usuario.");
                Log.Error($"Error: {e.Message}");
                MessageBox.Show(
                    $"Error 12: Ha ocurrido un error al validar las credenciales del proceso de autenticación. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que limpia todos los datos obtenidos de la base de datos.
        /// </summary>
        public static void CloseConnection() {
            try {
                Username = null;
                Password = null;
                IdRol = 0;
                ConnectionSuccess = false;
                Log.Info("Conexión con base de datos cerrada con exito.");
            }
            catch (Exception e) {
                Log.Error("Hubo un problema al cerrar la conección y borrar los campos estaticos de base de dato.");
                Log.Error($"Excepción: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método para obtener el username del usuario que ha iniciado sesión.
        /// </summary>
        /// <returns>Un <c>string</c> con el username.</returns>
        public static string GetUsername() {
            return Username;
        }

        /// <summary>
        /// Método para obtener el Id de rol del usuario que ha iniciado sesión.
        /// </summary>
        /// <returns>Un <c>int</c> con el Id del rol de usuario.</returns>
        public static int GetIdRol() {
            return IdRol;
        }

        /// <summary>
        /// Método para obtener el Id de tienda asignado a el usuario que ha iniciado sesión.
        /// </summary>
        /// <returns>Un <c>int</c> con el Id de tienda.</returns>
        public static int GetIdTiendaUsuario() {
            return IdTienda;
        }

        /// <summary>
        /// Método para obtener el id de tienda dado en el ini.
        /// </summary>
        /// <returns></returns>
        public static int GetIdTiendaIni() {
            return IdTiendaIni;
        }

        /// <summary>
        /// Método para obtener el nombre de la tienda del ini.
        /// </summary>
        /// <returns></returns>
        public static string GetNombreTiendaIni() {
            return NombreTiendaInit;
        }

        /// <summary>
        /// Método para saber si el usuario logueado tiene acceso a los campos de matriz.
        /// </summary>
        /// <returns></returns>
        public static bool GetMatrizEnabled() {
            return MatrizAsigned;
        }

        /// <summary>
        /// Método para saber si la sucursal dada tiene acceso a los campos de matriz.
        /// </summary>
        /// <returns></returns>
        public static bool GetMatrizEnabledFromIni() {
            CheckMatriz(true);
            return MatrizAsignedIni;
        }
    }
}