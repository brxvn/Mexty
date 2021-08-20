using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using log4net;
using Mexty.MVVM.Model.DataTypes;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model.DatabaseQuerys {
    /// <summary>
    /// Clase que contiene las consultas a la base de datos de el modulo de usuarios.
    /// </summary>
    public static class QuerysUsuario {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        /// <summary>
        /// Método para obtener todos los datos de la tabla usuario.
        /// </summary>
        /// <returns>Un objeto tipo <c>MySqlReader</c> con la informació con la información.</returns>
        public static List<Usuario> GetTablesFromUsuarios() {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from usuario"
            };

            var users = new List<Usuario>();
            try {
                using MySqlDataReader reader = query.ExecuteReader();
                while (reader.Read()) {
                    var usuario = new Usuario {
                        Id = reader.IsDBNull("id_usuario") ? 0 : reader.GetInt32("id_usuario"),
                        Nombre = reader.IsDBNull("nombre_usuario") ? "" : reader.GetString("nombre_usuario"),
                        ApPaterno = reader.IsDBNull("ap_paterno") ? "" : reader.GetString("ap_paterno"),
                        ApMaterno = reader.IsDBNull("ap_materno") ? "" : reader.GetString("ap_materno"),
                        Username = reader.IsDBNull("usuario") ? "" : reader.GetString("usuario"),
                        Contraseña = reader.IsDBNull("contrasenia") ? "" : reader.GetString("contrasenia"),
                        Domicilio = reader.IsDBNull("domicilio") ? "" : reader.GetString("domicilio"),
                        Telefono = reader.IsDBNull("telefono") ? "" : reader.GetString("telefono"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                        IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32("id_rol"),
                        UsuraioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                        UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                        FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica")
                    };
                    users.Add(usuario);
                }
                Log.Debug("Se han obtenido con exito los datos de la tabla usuarios");
                connObj.Close();
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un problema al obtener los datos de la tabla usuarios.");
                Log.Error($"Excepción: {e.Message}");
                MessageBox.Show(
                    $"Error 14: ha ocurrido un error al intentar obtener la información de la base de datos. {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }

            return users;
        }


        /// <summary>
        /// Método para actualziar los datos de Usuario
        /// </summary>
        public static int UpdateData(Usuario usuario) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                    update usuario 
                    set NOMBRE_USUARIO=@nomUsr, 
                        AP_PATERNO=@apPat, 
                        AP_MATERNO=@apMat, 
                        ID_TIENDA=@idTX, 
                        DOMICILIO=@dom, 
                        CONTRASENIA=@pass, 
                        TELEFONO=@telX, 
                        ID_ROL=@idRX, 
                        USUARIO_MODIFICA=@uMod, 
                        FECHA_MODIFICA=@date 
                    where ID_USUARIO=@idX"
            };

            query.Parameters.AddWithValue("@nomUsr", usuario.Nombre);
            query.Parameters.AddWithValue("@apPat", usuario.ApPaterno);
            query.Parameters.AddWithValue("@apMat", usuario.ApMaterno);
            query.Parameters.AddWithValue("@idTX",usuario.IdTienda.ToString()); // evitamos boxing//evitamos boxing.
            query.Parameters.AddWithValue("@dom", usuario.Domicilio);
            query.Parameters.AddWithValue("@pass", usuario.Contraseña);
            query.Parameters.AddWithValue("@telX", usuario.Telefono);
            query.Parameters.AddWithValue("@idX", usuario.Id.ToString());
            query.Parameters.AddWithValue("@idRX",usuario.IdRol.ToString());
            query.Parameters.AddWithValue("@uMod", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha actualizado el usuario exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar el usuario");
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
        /// Método que registra un nuevo usuario.
        /// </summary>
        /// <param name="newUser">Objeto tipo <c>Usuario</c> que tiene la información del usuario nuevo.</param>
        public static int NewUser(Usuario newUser) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"
                    insert into usuario 
                        (ID_USUARIO, NOMBRE_USUARIO, AP_PATERNO, AP_MATERNO, 
                         USUARIO, CONTRASENIA, DOMICILIO, TELEFONO, 
                         ID_TIENDA, ID_ROL, 
                         USUARIO_REGISTRA, 
                         FECHA_REGISTRO, 
                         USUARIO_MODIFICA, 
                         FECHA_MODIFICA) 
                    values (default, @nombre, @apPat, @apMat, 
                            @usuario, @pass, @dom, @telX, 
                            @idTX, @idRX, 
                            @usrReg, 
                            @date, 
                            @usrMod, 
                            @date)"
            };

            query.Parameters.AddWithValue("@nombre", newUser.Nombre);
            query.Parameters.AddWithValue("@apPat", newUser.ApPaterno);
            query.Parameters.AddWithValue("@apMat", newUser.ApMaterno);
            query.Parameters.AddWithValue("@usuario", newUser.Username);
            query.Parameters.AddWithValue("@pass", newUser.Contraseña);
            query.Parameters.AddWithValue("@dom", newUser.Domicilio);
            query.Parameters.AddWithValue("@telX", newUser.Telefono);
            query.Parameters.AddWithValue("@idTX", newUser.IdTienda.ToString());
            query.Parameters.AddWithValue("@idRX", newUser.IdRol.ToString());
            query.Parameters.AddWithValue("@usrReg", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@usrMod", DatabaseInit.GetUsername());
            query.Parameters.AddWithValue("@date", DatabaseHelper.GetCurrentTimeNDate());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha creado un nuevo usuario exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta un un nuevo usuario.");
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
        /// Método que elimina un usuario de la base de datos.
        /// </summary>
        /// <returns></returns>
        public static int DelProduct(int idClient) {
            var connObj = new MySqlConnection(IniFields.GetConnectionString());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"delete from usuario where ID_USUARIO=@idX"
            };

            query.Parameters.AddWithValue("@idX", idClient.ToString());

            try {
                QuerysDatabase.ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha eliminado un usuario de manera exitosa.");
                return res;

            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al eliminar los datos del usuario.");
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