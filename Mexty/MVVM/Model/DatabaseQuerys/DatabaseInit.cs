using System;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase principal de Base de datos.
    /// Se encarga de hacer el proceso de login y obtener los datos iniciales.
    /// </summary>
    public class DatabaseInit {
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
        /// <c>Bool</c> que guarda si el log-in fue exitoso.
        /// </summary>
        private static bool ConnectionSuccess { get; set; }

        /// <summary>
        /// Constructor inicial para el login.
        /// </summary>
        /// <param name="username">String que contiene el nombre de usuario.</param>
        /// <param name="password">String que contiene la contraseña.</param>
        public DatabaseInit(string username, string password) {
            var connObj = new MySqlConnection(IniFields.ConnectionInfo());
            connObj.Open();

            var login = new MySqlCommand {
                Connection = connObj,
                CommandText = @"select usuario, contrasenia, id_rol, ID_TIENDA from usuario where USUARIO=@user and CONTRASENIA=@pass"
            };
            login.Parameters.AddWithValue("@user", username);
            login.Parameters.AddWithValue("@pass", password);

            try {
                var fistQuery = login.ExecuteReader();

                InitializeFields(fistQuery);
                ValidateIdTienda();
                Log.Info("Se ha iniciado sesión de manera exitosa.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al hacer la consulta para iniciar sesión.");
                Log.Error($"Error: {e.Message}");
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
                // TODO: hacer distinción si la tienda asignada es Matriz.
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
                    "Error 12: Ha ocurrido un error al validar las credenciales del proceso de autenticación.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que valida que el id de tienda leido del ini sea válido.
        /// </summary>
        private static void ValidateIdTienda() {
            var connObj = new MySqlConnection(IniFields.ConnectionInfo());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select nombre_tienda from cat_tienda where ID_TIENDA=@idX"
            };

            try {
                var idTienda = IniFields.GetIdTiendaActual();
                query.Parameters.AddWithValue("@idX", idTienda.ToString());

                var res = query.ExecuteReader();

                if (res.Read().ToString().ToLower() == "false") {
                    Log.Error("No se ha podido validar el id de la tienda escrito en el ini.");
                    throw new Exception("Valor de IdTienda en ini invalido.");
                }
                else {
                    IdTienda = idTienda;
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al validar que el IdTienda sea valido.");
                Log.Error($"Error: {e.Message}");

                const MessageBoxButton buttons = MessageBoxButton.OK;
                MessageBox.Show("Error 13: ha ocurrido un error al validar la sucursal del archivo de configuración.",
                    "Error", buttons, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método para obtener el username del usuario que ha iniciado sesión.
        /// </summary>
        /// <returns></returns>
        public static string GetUsername() {
            return Username;
        }

        /// <summary>
        /// Método para obtener el Id de rol del usuario que ha iniciado sesión.
        /// </summary>
        /// <returns></returns>
        public static int GetIdRol() {
            return IdRol;
        }

        /// <summary>
        /// Método para obtener el Id de tienda asignado a el usuario que ha iniciado sesión.
        /// </summary>
        /// <returns></returns>
        public static int GetIdTienda() {
            return IdTienda;
        }
    }
}