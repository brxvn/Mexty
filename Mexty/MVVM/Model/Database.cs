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

namespace Mexty.MVVM.Model {
    /// <summary>
    /// La clase principal de base de datos.
    /// Contiene todos los métodos necesarios para la conección y uso de la Base de datos.
    /// </summary>
    public class Database {
        private static MySqlDataReader _isConnected;
        private static MySqlConnection _sqlSession;
        
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
               CommandText = "select nombre_usuario, usuario, contrasenia, id_rol from usuario where USUARIO=@user and CONTRASENIA=@pass"
           };           
           login.Parameters.AddWithValue("@user", username);
           login.Parameters.AddWithValue("@pass", password);

           var connectionSuccess = login.ExecuteReader();
           _isConnected = connectionSuccess;
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

        /// <summary> Método para saber si la conección con la base de datos fue exitosa. </summary>
        /// <returns>
        /// <c>true</c> si la conección fue exitosa, <c>false</c> si no.
        /// </returns>
        public bool IsConnected() {
            return _isConnected.Read();
        }

        /// <summary>
        /// Método que cierra la conección con la base de datos.
        /// </summary>
        public void CloseConnection() {
            _sqlSession.Close();
        }
        
        /// <summary>
        /// Método que retorna el ID del rol del usuario connectado.
        /// </summary>
        /// <returns>ID del usuario conectado</returns>
        public string GetRol() {
            return _isConnected.GetString("id_rol");
        }

        /// <summary>
        /// Método que retorna el nombre de usuario del usuario conectado.
        /// </summary>
        /// <returns>Retorna un <c>string</c> con el nombre de usuario.</returns>
        public string GetNombreUsuario() {
            return _isConnected.GetString("nombre_usuario");
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
            List<Usuarios> users = new List<Usuarios>();
            //var data = query.ExecuteReader();
            using (MySqlDataReader reader = query.ExecuteReader()) {
                while (reader.Read()) {
                    var usuario = new Usuarios();
                    usuario.Id = reader.GetInt32(0);
                    usuario.Username = reader.GetString(1);
                    usuario.ApPaterno = reader.GetString(2);
                    usuario.ApMaterno = reader.GetString(3);
                    usuario.Usuario = reader.GetString(4);
                    usuario.Contraseña = reader.GetString(5);
                    usuario.Domicilio = reader.GetString(6);
                    usuario.Telefono = reader.GetInt32(7);
                    usuario.Activo = reader.GetInt32(8);
                    usuario.IdTienda = reader.GetInt32(9);
                    usuario.IdRol = reader.GetInt32(10);
                    usuario.UsuraioRegistra = reader.GetString(11);
                    usuario.FechaRegistro = reader.GetString(12);
                    usuario.UsuarioModifica = reader.GetString(13);
                    usuario.FechaModifica = reader.GetString(14);
                    users.Add((usuario));
                }
            }
            return users;
        }

        public void GetSucursales() {
            
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
