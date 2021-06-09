using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Mexty.MVVM.Model
{
    public class Database
    {
        private MySqlDataReader _isConnected;
        private MySqlConnection _sqlSession;
        public Database(string username, string password)
        {
            var connObj =
                new MySqlConnection("server=localhost; database = mexty; Uid=root; pwd = root");
            
           connObj.Open();
           _sqlSession = connObj;

           var login = new MySqlCommand
           {
               Connection = connObj,
               CommandText = "select nombre_usuario, usuario, contrasenia, id_rol from usuario where USUARIO=@user and CONTRASENIA=@pass"
           };  
            
           login.Parameters.AddWithValue("@user", username);
           login.Parameters.AddWithValue("@pass", password);
           
           
           var connectionSuccess = login.ExecuteReader();
           _isConnected = connectionSuccess;
            
        }

        public bool IsConnected()
        {
            return _isConnected.Read();
        }

        public void CloseConnection()
        {
            _sqlSession.Close();
        }
        public string GetRol() {
            return _isConnected.GetString("id_rol");
        }

        public string GetNombreUsuario() {
            return _isConnected.GetString("nombre_usuario");
        }


    }
}
