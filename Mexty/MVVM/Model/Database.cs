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
using Google.Protobuf.WellKnownTypes;

namespace Mexty.MVVM.Model {
    /// <summary>
    /// La clase principal de base de datos.
    /// Contiene todos los métodos necesarios para la conección y uso de la Base de datos.
    /// </summary>
    // TODO: Maybe usar herencia para crear diferentes clases Database 
    public class Database {
        private static MySqlDataReader _firstQuery;
        private static MySqlConnection _sqlSession;

        /// <summary>
        /// Campo con el nombre de usuario de la persona logeada.
        /// </summary>
        private static string Username { get; set; }
        /// <summary>
        /// Campo con el password de la persona logeada.
        /// </summary>
        private static string Password { get; set; }

        /// <summary>
        /// ID del Rol de la persona logeada.
        /// </summary>
        private static int Rol { get; set; }

        /// <summary>
        /// <c>Bool</c> que guarda si el log-in fue exitoso.
        /// </summary>
        private static bool ConnectionSuccess { get; set; }

        /// <summary>
        /// Constructor principal de la clase <c>Database</c>, se encarga de
        /// hacer la conección principal a la base de datos.
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        public Database(string username, string password) {
            var connObj =
                new MySqlConnection(ConnectionInfo());
            
           connObj.Open(); // lanzar exepción o minimo un combo box cuando el susuario no sea correcto.
           _sqlSession = connObj;

           var login = new MySqlCommand {
               Connection = connObj,
               CommandText = "select usuario, contrasenia, id_rol from usuario where USUARIO=@user and CONTRASENIA=@pass"
           };           
           login.Parameters.AddWithValue("@user", username);
           login.Parameters.AddWithValue("@pass", password);

           var connectionSuccess = login.ExecuteReader();
           _firstQuery = connectionSuccess;
            //InitializeFields();
            if (_firstQuery.Read()) {
                InitializeFields();
            }
            //

        }

        /// <summary>
        /// Constructor sin parametros de Database, se usa para acceder a los métodos una vez
        /// ya se ha hecho la conección inicial.
        /// </summary>
        public Database() {
            var connObj =
                new MySqlConnection(ConnectionInfo());
            
           connObj.Open();
           _sqlSession = connObj;
        }

        /// <summary>
        /// Método que Lee el contenido del archivo ini para la connección.
        /// </summary>
        /// <returns></returns>
        private static string ConnectionInfo() {
            var myIni = new IniFile(@"C:\Mexty\Settings.ini");
            var user = myIni.Read("DbUser");
            var pass = myIni.Read("DbPass");
            var connString = $"server=localhost; database = mexty; Uid={user}; pwd ={pass}";
            return connString;
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
        public static string GetUsername() {
            return Username;
        }

        /// <summary>
        /// Método que retorna el ID del rol del usuario connectado.
        /// </summary>
        /// <returns>ID del usuario conectado </returns>
        public static int GetRol() {
            return Rol;
        }

        /// <summary>
        /// Método que cierra la conección con la base de datos.
        /// </summary>
        public static void CloseConnection() {
            _sqlSession.Close();
            Username = null;
            Password = null;
            Rol = 0;
            ConnectionSuccess = false;
        }

        // ============================================
        // ------- Querrys de Usuario ----------------
        // ============================================

        /// <summary>
        /// Método para obtener todos los datos de la tabla usuario.
        /// </summary>
        /// <returns>Un objeto tipo <c>MySqlReader</c> con la informació con la información.</returns>
        public List<Usuario> GetTablesFromUsuarios() {
            var query = new MySqlCommand() {
                Connection = _sqlSession,
                CommandText = "select * from usuario"
            };
            var users = new List<Usuario>();
            using MySqlDataReader reader = query.ExecuteReader();
            while (reader.Read()) {
                var usuario = new Usuario {
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

            return users;
        }

        /// <summary>
        /// Método para actualziar los datos de Usuario
        /// </summary>
        public static void UpdateData(Usuario usuario) {
            var query = new MySqlCommand() {
                Connection = _sqlSession,
                CommandText = "update usuario set NOMBRE_USUARIO=@nomUsr, AP_PATERNO=@apPat, AP_MATERNO=@apMat, ID_TIENDA=@idTi, DOMICILIO=@dom, CONTRASENIA=@pass, TELEFONO=@tel, ACTIVO=@act, ID_ROL=@idRo where ID_USUARIO=@ID"
            };
            query.Parameters.AddWithValue("@nomUsr", usuario.Nombre);
            query.Parameters.AddWithValue("@apPat", usuario.ApPaterno);
            query.Parameters.AddWithValue("@apMat", usuario.ApMaterno);
            query.Parameters.AddWithValue("@idTi",usuario.IdTienda.ToString()); // evitamos boxing//evitamos boxing.
            query.Parameters.AddWithValue("@dom", usuario.Domicilio);
            query.Parameters.AddWithValue("@pass", usuario.Contraseña);
            query.Parameters.AddWithValue("@tel", usuario.Telefono.ToString());
            query.Parameters.AddWithValue("@ID", usuario.Id.ToString());
            query.Parameters.AddWithValue("@act", usuario.Activo.ToString());
            query.Parameters.AddWithValue("@idRo",usuario.IdRol.ToString());

            try {
                query.ExecuteReader();
            }
            catch (MySqlException e) {
                MessageBox.Show("Error (update) exepción: {0}",e.ToString());
            }
        }

        /// <summary>
        /// Método que registra un nuevo usuario.
        /// </summary>
        /// <param name="newUser">Objeto tipo <c>Usuario</c> que tiene la información del usuario nuevo.</param>
        public static void NewUser(Usuario newUser) {
            MySqlCommand query = new() {
                Connection = _sqlSession,
                CommandText = "insert into usuario values (default, @nombre, @apPat, @apMat, @usr, @pass, @dom, @tel, @act, @idT, @idR, @usrReg, sysdate(), @usrMod, sysdate())"
            };

            query.Parameters.AddWithValue("@nombre", newUser.Nombre);
            query.Parameters.AddWithValue("@apPat", newUser.ApPaterno);
            query.Parameters.AddWithValue("@apMat", newUser.ApMaterno);
            query.Parameters.AddWithValue("@usr", newUser.Username);
            query.Parameters.AddWithValue("@pass", newUser.Contraseña);
            query.Parameters.AddWithValue("@dom", newUser.Domicilio);
            query.Parameters.AddWithValue("@tel", newUser.Telefono.ToString());// Evitamos boxing
            query.Parameters.AddWithValue("@act", newUser.Activo.ToString());
            query.Parameters.AddWithValue("@idT", newUser.IdTienda.ToString());
            query.Parameters.AddWithValue("@idR", newUser.IdRol.ToString());
            query.Parameters.AddWithValue("@usrReg", newUser.UsuraioRegistra);
            query.Parameters.AddWithValue("@usrMod", newUser.UsuarioModifica);

            try {
                query.ExecuteNonQuery();// retorna el número de columnas cambiadas.
            }
            catch (MySqlException e) {
                Console.WriteLine("Usuario existente");            
            }
        }

        // ============================================
        // ------- Querrys de Sucursales --------------
        // ============================================

        /// <summary>
        /// Función que obtiene las tablas de las Sucursales.
        /// </summary>
        public List<Sucursal> GetTablesFromSucursales() {
            var query = new MySqlCommand() {
                Connection = _sqlSession,
                CommandText = "select * from cat_tienda"
            };
            var sucursales = new List<Sucursal>();
            using MySqlDataReader reader = query.ExecuteReader();
            while (reader.Read()) {
                var sucursal = new Sucursal {
                    IdTienda = reader.GetInt32(0),
                    NombreTienda = reader.GetString(1),
                    Dirección = reader.GetString(2),
                    // Telefono = reader.GetInt32(3), TODO: Lidiar con datos nulos.
                    // Rfc = reader.GetString(4),
                    // Logo = reader.GetBytes(5);  TODO: ver como hacerle con el logo.
                    // Mensaje = reader.GetString(6),
                    // Facebook = reader.GetString(7),
                    // Instagram = reader.GetString(8),
                    // TipoTienda = reader.GetString(9)
                };
                sucursales.Add(sucursal);
            }

            return sucursales;
        }


        // ============================================
        // ------- Querrys de Rol ---------------------
        // ============================================


        /// <summary>
        /// Función que obtiene las tablas de los Roles.
        /// </summary>
        public List<Rol> GetTablesFromRoles() {
            var querry = new MySqlCommand() {
                Connection = _sqlSession,
                CommandText = "select * from cat_rol_usuario"
            };
            var roles = new List<Rol>();
            using MySqlDataReader reader = querry.ExecuteReader();
            while (reader.Read()) {
                var rol = new Rol() {
                    IdRol = reader.GetInt32(0),
                    RolDescription = reader.GetString(1),
                    IdTienda = reader.GetInt32(2)
                };
                roles.Add(rol);
            }

            return roles;
        }

        /// <summary>
        /// Destructor para la clase Database.
        /// </summary>
        // TODO: implementar que cuando se ejecute el destructor te mande al login.
        // TODO: Ver si es necesario esto o no xd (puede Q no)
        //~Database() {
        //    _sqlSession.Close();
        //}
    }
}
