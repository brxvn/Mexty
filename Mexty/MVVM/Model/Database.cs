using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model
{
    /// <summary>
    /// La clase principal de base de datos.
    /// Contiene todos los métodos necesarios para la conección y uso de la Base de datos.
    /// </summary>
    public class Database
    {
        private readonly MySqlDataReader _isConnected;
        private readonly MySqlConnection _sqlSession;
        
        /// <summary> Constructor principal de la clase <c>Database</c></summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        public Database(string username, string password)
        {
            var connObj =
                new MySqlConnection("server=localhost; database = mexty; Uid=root; pwd = Jorgedavid12"); // TODO: Cambiar contraseñas.
            connObj.Open();
           _sqlSession = connObj;

           var login = new MySqlCommand
           {
               Connection = connObj,
               CommandText = "select usuario, contrasenia from usuario where USUARIO=@user and CONTRASENIA=@pass"
           };           
           // Siempre usar parametros para sanitizar imput.
           login.Parameters.AddWithValue("@user", username);
           login.Parameters.AddWithValue("@pass", password);
           
           var connectionSuccess = login.ExecuteReader();
           _isConnected = connectionSuccess; 
        }

        /// <summary>
        /// Método para saber si la conección con la base de datos fue exitosa.
        /// </summary>
        /// <returns>
        /// <c>true</c> si la conección fue exitosa, <c>false</c> si no.
        /// </returns>
        public bool IsConnected()
        {
            return _isConnected.Read();
        }

        /// <summary>
        /// Método que cierra la conección con la base de datos.
        /// </summary>
        public void CloseConnection()
        {
            _sqlSession.Close();
        }
    }
}
