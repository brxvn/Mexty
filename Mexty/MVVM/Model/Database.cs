using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using K4os.Compression.LZ4.Internal;
using MySql.Data.MySqlClient;
using Mexty.MVVM.Model.DataTypes;
using System.Windows;

namespace Mexty.MVVM.Model {
    /// <summary>
    /// La clase principal de base de datos.
    /// Contiene todos los métodos necesarios para la conección y uso de la Base de datos.
    /// </summary>
    // TODO: Maybe usar herencia para crear diferentes clases Database 
    public class Database {
        private static MySqlDataReader _firstQuery;
        private static MySqlConnection _sqlSession;
        
        private static string Username { get; set; }
        private static string Password { get; set; }
        private static int Rol { get; set; }
        private static bool ConnectionSuccess { get; set; }
        
        /// <summary>
        /// Constructor principal de la clase <c>Database</c>, se encarga de
        /// hacer la conección principal a la base de datos.
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        public Database(string username, string password) {
            var connObj =
                new MySqlConnection("server=localhost; database = mexty; Uid=root; pwd = Jorgedavid12");
            
           connObj.Open();
           _sqlSession = connObj;

           var login = new MySqlCommand {
               Connection = connObj,
               CommandText = "select usuario, contrasenia, id_rol from usuario where USUARIO=@user and CONTRASENIA=@pass"
           };           
           login.Parameters.AddWithValue("@user", username);
           login.Parameters.AddWithValue("@pass", password);

           var connectionSuccess = login.ExecuteReader();
           _firstQuery = connectionSuccess;
           InitializeFields();
        }

        /// <summary>
        /// Constructor sin parametros de Database, se usa para acceder a los métodos una vez
        /// ya se ha hecho la conección inicial.
        /// </summary>
        public Database() {
            var connObj =
                new MySqlConnection("server=localhost; database = mexty; Uid=root; pwd = Jorgedavid12");
            
           connObj.Open();
           _sqlSession = connObj;
           
        }

        /// <summary>
        /// Inicializa los campos estaticos de la clase.
        /// </summary>
        private static void InitializeFields() {
            _firstQuery.Read();
            Username= _firstQuery.GetString("usuario");
            Rol = int.Parse(_firstQuery.GetString("id_rol"));
            Password = _firstQuery.GetString("contrasenia");
            if (Username != "" && Password != "") {
                ConnectionSuccess = true;
            }
        }

        /// <summary> Método para saber si la conección con la base de datos fue exitosa. </summary>
        /// <returns>
        /// <c>true</c> si la conección fue exitosa, <c>false</c> si no.
        /// </returns>
        public static bool IsConnected() {
            return ConnectionSuccess;
        }

        /// <summary>
        /// Método que retorna el username de la persona conectada.
        /// </summary>
        /// <returns></returns>
        public static string GetUsername() { // -------------------------
            return Username;
        }
        
        /// <summary>
        /// Método que retorna el ID del rol del usuario connectado.
        /// </summary>
        /// <returns>ID del usuario conectado </returns>
        public static int GetRol() { // -----------------------
            return Rol;
        }
        
        /// <summary>
        /// Método que cierra la conección con la base de datos.
        /// </summary>
        public void CloseConnection() {
            _sqlSession.Close();
        }

        /// <summary>
        /// Método para obtener todos los datos de la tabla usuario.
        /// </summary>
        /// <returns>Un objeto tipo <c>MySqlReader</c> con la informació con la información.</returns>
        public List<Usuarios> GetTablesFromUsuarios() {
            var query = new MySqlCommand() {
                Connection = _sqlSession,
                CommandText = "select * from usuario"
            };
            var users = new List<Usuarios>();
            using (MySqlDataReader reader = query.ExecuteReader()) {
                while (reader.Read()) {
                    var usuario = new Usuarios {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        ApPaterno = reader.GetString(2),
                        ApMaterno = reader.GetString(3),
                        Username = reader.GetString(4),
                        Contraseña = reader.GetString(5),
                        Domicilio = reader.GetString(6),
                        Telefono = reader.GetInt32(7),
                        Activo = reader.GetInt32(8),
                        IdTienda = reader.GetInt32(9),
                        IdRol = reader.GetInt32(10),
                        UsuraioRegistra = reader.GetString(11),
                        FechaRegistro = reader.GetString(12),
                        UsuarioModifica = reader.GetString(13),
                        FechaModifica = reader.GetString(14)
                    };
                    users.Add((usuario));
                }
            }
            return users;
        }

        /// <summary>
        /// Método para actualziar los datos de Usuarios
        /// </summary>
        public void UpdateData(Usuarios usuario) {
            var query = new MySqlCommand() {
                Connection = _sqlSession,
                CommandText = "update usuario set NOMBRE_USUARIO=@nomUsr, AP_PATERNO=@apPat, AP_MATERNO=@apMat, ID_TIENDA=@idTi, DOMICILIO=@dom, CONTRASENIA=@pass, TELEFONO=@tel where ID_USUARIO=@ID"
            };
            query.Parameters.AddWithValue("@nomUsr", usuario.Nombre.ToLower());
            query.Parameters.AddWithValue("@apPat", usuario.ApPaterno.ToLower());
            query.Parameters.AddWithValue("@apMat", usuario.ApMaterno.ToLower());
            query.Parameters.AddWithValue("@idTi", usuario.IdTienda);
            query.Parameters.AddWithValue("@dom", usuario.Domicilio.ToLower());
            query.Parameters.AddWithValue("@pass", usuario.Contraseña);
            query.Parameters.AddWithValue("@tel", usuario.Telefono);
            query.Parameters.AddWithValue("@ID", usuario.Id);
            query.ExecuteReader();
        }

        public void GetSucursales() {
            
        }

        /// <summary>
        /// Método que registra un nuevo usuario.
        /// </summary>
        /// <param name="newUser">Objeto tipo <c>Usuarios</c> que tiene la información del usuario nuevo.</param>
        public static void NewUser(Usuarios newUser) {
            MySqlCommand query = new() {
                Connection = _sqlSession,
                CommandText = "insert into usuario values (default, @nombre, @apPat, @apMat, @usr, @pass, @dom, @tel, @act, @idT, @idR, @usrReg, sysdate(), @usrMod, sysdate())"
            };
            query.Parameters.AddWithValue("@nombre", newUser.Nombre.ToLower());
            query.Parameters.AddWithValue("@apPat", newUser.ApPaterno.ToLower());
            query.Parameters.AddWithValue("@apMat", newUser.ApMaterno.ToLower());
            query.Parameters.AddWithValue("@usr", newUser.Username.ToLower());
            query.Parameters.AddWithValue("@pass", newUser.Contraseña);
            query.Parameters.AddWithValue("@dom", newUser.Domicilio.ToLower());
            query.Parameters.AddWithValue("@tel", newUser.Telefono);
            query.Parameters.AddWithValue("@act", newUser.Activo);
            query.Parameters.AddWithValue("@idT", newUser.IdTienda);
            query.Parameters.AddWithValue("@idR", newUser.IdRol);
            query.Parameters.AddWithValue("@usrReg", newUser.UsuraioRegistra);
            query.Parameters.AddWithValue("@usrMod", newUser.UsuarioModifica);

            try {
                query.ExecuteNonQuery();// retorna el número de columnas cambiadas.
            }
            catch (MySqlException e) {
                MessageBox.Show("Error exepción: "+e);
            }
        }

        /// <summary>
        /// Destructor para la clase Database.
        /// </summary>
        // TODO: implementar que cuando se ejecute el destructor te mande al login.
        // TODO: Ver si es necesario esto o no xd (puede Q no)
        ~Database() {
            //_sqlSession.Close();
        }
    }
}
