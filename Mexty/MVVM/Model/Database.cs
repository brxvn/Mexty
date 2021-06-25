using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Mexty.MVVM.Model.DataTypes;
using System.Windows;


namespace Mexty.MVVM.Model {
    /// <summary>
    /// La clase principal de base de datos.
    /// Contiene todos los métodos necesarios para la conección y uso de la Base de datos.
    /// </summary>
    // TODO: Hacer la clase Database estacica y re implementar login.
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
            if (_firstQuery.Read()) {
                InitializeFields();
            }

        }

        /// <summary>
        /// Método que Lee el contenido del archivo ini para la connección.
        /// </summary>
        /// <returns>String con la información de log-in a la base de datos</returns>
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

        // =================================================
        // ------- Info sobre el usuario connectado. -------
        // =================================================

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
        /// <returns><c>string</c> con el username de la persona logeada.</returns>
        public static string GetUsername() {
            return Username;
        }

        /// <summary>
        /// Método que retorna el ID del rol del usuario connectado.
        /// </summary>
        /// <returns>ID <c>int</c> del usuario conectado </returns>
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
        public static List<Usuario> GetTablesFromUsuarios() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from usuario"
            };
            var users = new List<Usuario>();
            using MySqlDataReader reader = query.ExecuteReader();
            while (reader.Read()) {
                var usuario = new Usuario {
                    Id = reader.IsDBNull("id_usuario") ? 0 : reader.GetInt32(0),
                    Nombre = reader.IsDBNull("nombre_usuario") ? "" : reader.GetString(1),
                    ApPaterno = reader.IsDBNull("ap_paterno") ? "" : reader.GetString(2),
                    ApMaterno = reader.IsDBNull("ap_materno") ? "" : reader.GetString(3),
                    //Username = reader.IsDBNull("usuario") ? "" : reader.GetString(4),
                    Contraseña = reader.IsDBNull("contrasenia") ? "" : reader.GetString(5),
                    Domicilio = reader.IsDBNull("domicilio") ? "" : reader.GetString(6),
                    Telefono = reader.IsDBNull("telefono") ? 0 : reader.GetInt32(7),
                    Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32(8),
                    IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32(9),
                    IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32(10),
                    UsuraioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString(11),
                    FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString(12),
                    UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString(13),
                    FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString(14)
                };
                users.Add(usuario);
            }
            connObj.Close();

            return users;
        }

        /// <summary>
        /// Método para actualziar los datos de Usuario
        /// </summary>
        public static void UpdateData(Usuario usuario) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "update usuario set NOMBRE_USUARIO=@nomUsr, AP_PATERNO=@apPat, AP_MATERNO=@apMat, ID_TIENDA=@idTi, DOMICILIO=@dom, CONTRASENIA=@pass, TELEFONO=@tel, ACTIVO=@act, ID_ROL=@idRo, USUARIO_MODIFICA=@uMod, FECHA_MODIFICA=sysdate() where ID_USUARIO=@ID"
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
            query.Parameters.AddWithValue("@uMod", Database.GetUsername());

            try {
                query.ExecuteReader();
            }
            catch (MySqlException e) {
                MessageBox.Show("Error (update) exepción: {0}", e.ToString());
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra un nuevo usuario.
        /// </summary>
        /// <param name="newUser">Objeto tipo <c>Usuario</c> que tiene la información del usuario nuevo.</param>
        public static void NewUser(Usuario newUser) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            MySqlCommand query = new() {
                Connection = connObj,
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
            query.Parameters.AddWithValue("@usrReg", Database.GetUsername());
            query.Parameters.AddWithValue("@usrMod", Database.GetUsername());

            try {
                query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
            }
            catch (MySqlException e) {
                MessageBox.Show("Error (new User) exepción: {0}", e.ToString());
            }
            finally {
                connObj.Close();
            }
        }

        // ============================================
        // ------- Querrys de Sucursales --------------
        // ============================================

        /// <summary>
        /// Función que obtiene las tablas de las Sucursales.
        /// </summary>
        /// <returns>Una lista de objetos tipo <c>Sucursal</c>.</returns>
        public static List<Sucursal> GetTablesFromSucursales() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cat_tienda"
            };
            var sucursales = new List<Sucursal>();
            using var reader = query.ExecuteReader();
            while (reader.Read()) {
                var sucursal = new Sucursal {
                    IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32(0),
                    NombreTienda = reader.IsDBNull("nombre_tienda") ? "" : reader.GetString(1),
                    Dirección = reader.IsDBNull("direccion") ? "" : reader.GetString(2),
                    Telefono = reader.IsDBNull("telefono") ? 0 : reader.GetInt32(3), 
                    Rfc = reader.IsDBNull("rfc") ? "" : reader.GetString(4),
                    // Logo = reader.GetBytes(5);  TODO: ver como hacerle con el logo.
                    Mensaje = reader.IsDBNull("mensaje") ? "" : reader.GetString(6),
                    Facebook = reader.IsDBNull("facebook") ? "" : reader.GetString(7),
                    Instagram = reader.IsDBNull("instagram") ? "" : reader.GetString(8),
                    TipoTienda = reader.IsDBNull("tipo_tienda") ? "" : reader.GetString(9)
                };
                sucursales.Add(sucursal);
            }

            connObj.Close();
            return sucursales;
        }
        


        // ============================================
        // ------- Querrys de Rol ---------------------
        // ============================================


        /// <summary>
        /// Función que obtiene las tablas de los Roles.
        /// </summary>
        /// <returns>Una lista con objetos tipo <c>Rol</c>.</returns>
        public static List<Rol> GetTablesFromRoles() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            var querry = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cat_rol_usuario"
            };
            var roles = new List<Rol>();
            using var reader = querry.ExecuteReader();
            while (reader.Read()) {
                var rol = new Rol() {
                    IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32(0),
                    RolDescription = reader.IsDBNull("desc_rol") ? "" : reader.GetString(1),
                    IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32(2)
                };
                roles.Add(rol);
            }

            connObj.Close();
            return roles;
        }

        // ============================================
        // ------- Querrys de Productos ---------------
        // ============================================

        /// <summary>
        /// Función que retorna una lista con los productos de la base de datos.
        /// </summary>
        /// <returns> Una lista de objetos tipo <c>Producto</c>.</returns>
        public static List<Producto> GetTablesFromProductos() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var querry = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cat_producto"
            };
            var productos = new List<Producto>();
            using var reader = querry.ExecuteReader();
            while (reader.Read()) {
                var producto = new Producto {
                    IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32(0),
                    NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString(1),
                    MedidaProducto = reader.IsDBNull("medida") ? "" : reader.GetString(2),
                    TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString(3),
                    TipoVenta = reader.IsDBNull("tipo_venta") ? 0 : reader.GetInt32(4),
                    PrecioMayoreo = reader.IsDBNull("precio_mayoreo") ? 0 : reader.GetInt32(5),
                    PrecioMenudeo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetInt32(6),
                    DetallesProducto = reader.IsDBNull("especificacion_producto") ? "" : reader.GetString(7),
                    Activo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetInt32(8)
                };
                productos.Add(producto);
            }

            connObj.Close();
            return productos;
        }

        /// <summary>
        /// Método que actualiza un producto en la base de datos.
        /// </summary>
        /// <param name="producto"></param>
        public static void UpdateData(Producto producto) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "update cat_producto set NOMBRE_PRODUCTO=@nom, MEDIDA=@med, TIPO_PRODUCTO=@tipoP, TIPO_VENTA=@tipoV, PRECIO_MAYOREO=@pMayo, PRECIO_MENUDEO=@pMenu, ESPECIFICACION_PRODUCTO=@esp, ACTIVO=@act where ID_PRODUCTO=@id"
            };
            query.Parameters.AddWithValue("@nom", producto.NombreProducto);
            query.Parameters.AddWithValue("@med", producto.MedidaProducto);
            query.Parameters.AddWithValue("@tipoP", producto.TipoProducto);
            query.Parameters.AddWithValue("@tipoV", producto.TipoVenta.ToString());
            query.Parameters.AddWithValue("@pMayo", producto.PrecioMayoreo.ToString());
            query.Parameters.AddWithValue("@pMenu", producto.PrecioMenudeo.ToString());
            query.Parameters.AddWithValue("@esp", producto.DetallesProducto);
            query.Parameters.AddWithValue("@id", producto.IdProducto.ToString());
            query.Parameters.AddWithValue("@act", producto.Activo.ToString());
            
            try {
                query.ExecuteReader();
            }
            catch (MySqlException e) {
                MessageBox.Show("Error (update Producto) exepción: {0}", e.ToString());
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra un nuevo producto.
        /// </summary>
        /// <param name="newProduct">Objeto tipo <c>Producto</c>.</param>
        public static void NewProduct(Producto newProduct) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "insert into cat_producto values (default, @nom, @medida, @tipoP, @tipoV, @pMayo, @pMenu, @esp, @act)"
            };
            query.Parameters.AddWithValue("@nom", newProduct.NombreProducto);
            query.Parameters.AddWithValue("@medida", newProduct.MedidaProducto);
            query.Parameters.AddWithValue("@tipoP", newProduct.TipoProducto);
            query.Parameters.AddWithValue("@tipoV", newProduct.TipoVenta.ToString());
            query.Parameters.AddWithValue("@pMayo", newProduct.PrecioMayoreo.ToString());
            query.Parameters.AddWithValue("@pMenu", newProduct.PrecioMenudeo.ToString());
            query.Parameters.AddWithValue("@esp", newProduct.DetallesProducto);
            query.Parameters.AddWithValue("@act", 1.ToString());

            try {
                query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
            }
            catch (MySqlException e) {
                MessageBox.Show("Error (new Producto) exepción: {0}", e.ToString());
            }
            finally {
                connObj.Close();
            }
        }
        
        // ============================================
        // ------- Métodos De la clase ----------------
        // ============================================


    }
}
