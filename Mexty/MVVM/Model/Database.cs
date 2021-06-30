using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Mexty.MVVM.Model.DataTypes;
using System.Windows;
using System.Windows.Documents;


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
                    Id = reader.IsDBNull("id_usuario") ? 0 : reader.GetInt32("id_usuario"),
                    Nombre = reader.IsDBNull("nombre_usuario") ? "" : reader.GetString("nombre_usuario"),
                    ApPaterno = reader.IsDBNull("ap_paterno") ? "" : reader.GetString("ap_paterno"),
                    ApMaterno = reader.IsDBNull("ap_materno") ? "" : reader.GetString("ap_materno"),
                    Username = reader.IsDBNull("usuario") ? "" : reader.GetString("usuario"),
                    Contraseña = reader.IsDBNull("contrasenia") ? "" : reader.GetString("contrasenia"),
                    Domicilio = reader.IsDBNull("domicilio") ? "" : reader.GetString("domicilio"),
                    Telefono = reader.IsDBNull("telefono") ? 0 : reader.GetInt32("telefono"),
                    Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo"),
                    IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                    IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32("id_rol"),
                    UsuraioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                    FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                    UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                    FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica")
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
                CommandText = @"
                    update usuario 
                    set NOMBRE_USUARIO=@nomUsr, 
                        AP_PATERNO=@apPat, 
                        AP_MATERNO=@apMat, 
                        ID_TIENDA=@idTi, 
                        DOMICILIO=@dom, 
                        CONTRASENIA=@pass, 
                        TELEFONO=@tel, 
                        ACTIVO=@act, 
                        ID_ROL=@idRo, 
                        USUARIO_MODIFICA=@uMod, 
                        FECHA_MODIFICA=sysdate() 
                    where ID_USUARIO=@ID"
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
                MessageBox.Show($"Error (update Usuario) exepción: {e.ToString()}","Error");
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
                CommandText = @"
                    insert into usuario 
                        (ID_USUARIO, NOMBRE_USUARIO, AP_PATERNO, AP_MATERNO, 
                         USUARIO, CONTRASENIA, DOMICILIO, TELEFONO, 
                         ACTIVO, ID_TIENDA, ID_ROL, 
                         USUARIO_REGISTRA, 
                         FECHA_REGISTRO, 
                         USUARIO_MODIFICA, 
                         FECHA_MODIFICA) 
                    values (default, @nombre, @apPat, @apMat, 
                            @usr, @pass, @dom, @tel, 
                            @act, @idT, @idR, 
                            @usrReg, 
                            sysdate(), 
                            @usrMod, 
                            sysdate())"
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
                MessageBox.Show($"Error (new User) exepción: {e.ToString()}", "Error");
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
                    IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                    NombreTienda = reader.IsDBNull("nombre_tienda") ? "" : reader.GetString("nombre_tienda"),
                    Dirección = reader.IsDBNull("direccion") ? "" : reader.GetString("direccion"),
                    Telefono = reader.IsDBNull("telefono") ? 0 : reader.GetInt32("telefono"), 
                    Rfc = reader.IsDBNull("rfc") ? "" : reader.GetString("rfc"),
                    // Logo = reader.GetBytes(5);  TODO: ver como hacerle con el logo.
                    Mensaje = reader.IsDBNull("mensaje") ? "" : reader.GetString("mensaje"),
                    Facebook = reader.IsDBNull("facebook") ? "" : reader.GetString("facebook"),
                    Instagram = reader.IsDBNull("instagram") ? "" : reader.GetString("instagram"),
                    TipoTienda = reader.IsDBNull("tipo_tienda") ? "" : reader.GetString("tipo_tienda")
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
                    IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32("id_rol"),
                    RolDescription = reader.IsDBNull("desc_rol") ? "" : reader.GetString("desc_rol"),
                    IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda")
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
                    IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                    NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                    MedidaProducto = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                    TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                    TipoVenta = reader.IsDBNull("tipo_venta") ? 0 : reader.GetInt32("tipo_venta"),
                    PrecioMayoreo = reader.IsDBNull("precio_mayoreo") ? 0 : reader.GetInt32("precio_mayoreo"),
                    PrecioMenudeo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetInt32("precio_menudeo"),
                    DetallesProducto = reader.IsDBNull("especificacion_producto") ? "" : reader.GetString("especificacion_producto"),
                    Activo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetInt32("precio_menudeo")
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
                CommandText = @"
                update cat_producto 
                set NOMBRE_PRODUCTO=@nom, 
                    MEDIDA=@med, 
                    TIPO_PRODUCTO=@tipoP, 
                    TIPO_VENTA=@tipoV, 
                    PRECIO_MAYOREO=@pMayo, 
                    PRECIO_MENUDEO=@pMenu, 
                    ESPECIFICACION_PRODUCTO=@esp, 
                    ACTIVO=@act 
                where ID_PRODUCTO=@id"
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
                CommandText = @"
                insert into cat_producto 
                    (ID_PRODUCTO, NOMBRE_PRODUCTO, MEDIDA, TIPO_PRODUCTO, 
                     TIPO_VENTA, PRECIO_MAYOREO, PRECIO_MENUDEO, 
                     ESPECIFICACION_PRODUCTO, ACTIVO) 
                values (default, @nom, @medida, @tipoP, 
                        @tipoV, @pMayo, @pMenu, 
                        @esp, @act)"
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
        // ------- Querys de Clientes -----------------
        // ============================================

        /// <summary>
        /// Método para obtener todos los datos de la tabla de clientes Mayoreo.
        /// </summary>
        /// <returns>Una lista de elementos tipo <c>Cliente</c>.</returns>
        public static List<Cliente> GetTablesFromClientes() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from cliente_mayoreo"
            };
            var clientes = new List<Cliente>();
            using MySqlDataReader reader = query.ExecuteReader();
            while (reader.Read()) {
                var cliente = new Cliente() {
                    IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente"),
                    Nombre = reader.IsDBNull("nombre_cliente") ? "" : reader.GetString("nombre_cliente"),
                    ApPaterno = reader.IsDBNull("ap_paterno") ? "" : reader.GetString("ap_paterno"),
                    ApMaterno = reader.IsDBNull("ap_materno") ? "" : reader.GetString("ap_materno"),
                    Domicilio = reader.IsDBNull("domicilio") ? "" : reader.GetString("domicilio"),
                    Telefono = reader.IsDBNull("telefono") ? 0 : reader.GetInt32("telefono"),
                    Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo"),
                    UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                    FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                    UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                    FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica"),
                    Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario")
                };
                clientes.Add(cliente);
            }
            connObj.Close();
            return clientes;
        }

        /// <summary>
        /// Método para actualizar los datos del Cliente.
        /// </summary>
        /// <param name="cliente"></param>
        public static void UpdateData(Cliente cliente) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cliente_mayoreo 
                set NOMBRE_CLIENTE=@nom, AP_PATERNO=@apP, AP_MATERNO=@apM, 
                    DOMICILIO=@dom, TELEFONO=@tel, ACTIVO=@act, 
                    USUARIO_MODIFICA=@usMod, FECHA_MODIFICA=sysdate(), 
                    COMENTARIO=@com 
                where ID_CLIENTE=@id"
            };
            query.Parameters.AddWithValue("@nom", cliente.Nombre);
            query.Parameters.AddWithValue("@apP", cliente.ApPaterno);
            query.Parameters.AddWithValue("@apM", cliente.ApMaterno);
            query.Parameters.AddWithValue("@dom", cliente.Domicilio);
            query.Parameters.AddWithValue("@tel", cliente.Telefono.ToString());
            query.Parameters.AddWithValue("@act", cliente.Activo.ToString());
            query.Parameters.AddWithValue("@usMod", Database.GetUsername());
            query.Parameters.AddWithValue("@id", cliente.IdCliente.ToString());
            query.Parameters.AddWithValue("@com", cliente.Comentario);

            try {
                query.ExecuteReader();
            }
            catch (MySqlException e) {
                MessageBox.Show($"Error (update Cliente) exepción: {e.ToString()}", "Error");
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra un nuevo cliente.
        /// </summary>
        /// <param name="newClient"></param>
        public static void NewClient(Cliente newClient) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"
                insert into cliente_mayoreo 
                    (id_cliente, nombre_cliente, ap_paterno, ap_materno, 
                     domicilio, telefono, activo, 
                     usuario_registra, fecha_registro, 
                     usuario_modifica, fecha_modifica, 
                     comentario) 
                values (default, @nom, @apP, @apM, 
                        @dom, @tel, @act, 
                        @usReg, sysdate(), 
                        @usMod, sysdate(), 
                        @com)"
            };
            query.Parameters.AddWithValue("@nom", newClient.Nombre);
            query.Parameters.AddWithValue("@apP", newClient.ApPaterno);
            query.Parameters.AddWithValue("@apM", newClient.ApMaterno);
            query.Parameters.AddWithValue("@dom", newClient.Domicilio);
            query.Parameters.AddWithValue("@tel", newClient.Telefono.ToString());
            query.Parameters.AddWithValue("@act", newClient.Activo.ToString());
            query.Parameters.AddWithValue("@usReg", Database.GetUsername());
            query.Parameters.AddWithValue("@usMod", Database.GetUsername());
            query.Parameters.AddWithValue("@com", newClient.Comentario);
            
            try {
                query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
            }
            catch (MySqlException e) {
                MessageBox.Show($"Error (new User) exepción: {e.ToString()}", "Error");
            }
            finally {
                connObj.Close();
            }
        }

        // ============================================
        // ------- Querys de Deudas Mayoreo -----------
        // ============================================

        /// <summary>
        /// Método para obtener todos los datos de la tabla de Deudas Mayoreo.
        /// </summary>
        /// <returns>Una lista de elementos tipo <c>Deuda</c>.</returns>
        public static List<Deuda> GetTablesFromDeudas() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = "select * from deuda_mayoreo"
            };
            var deudas = new List<Deuda>();
            using MySqlDataReader reader = query.ExecuteReader();
            while (reader.Read()) {
                var deuda = new Deuda() {
                    IdDeuda = reader.IsDBNull("id_deuda") ? 0 : reader.GetInt32("id_deuda"),
                    IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente"),
                    Debe = reader.IsDBNull("debe") ? 0 : reader.GetDouble("debe"),
                    UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                    FechaRegistra = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                    UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                    FechaModifca = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica")
                };
                deudas.Add(deuda);
            }
            connObj.Close();
            return deudas;
        }

        /// <summary>
        /// Método para actualizar los datos de las deudas del cliente.
        /// </summary>
        /// <param name="deuda"></param>
        public static void UpdateData(Deuda deuda) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update deuda_mayoreo 
                set ID_CLIENTE=@idCli, 
                    DEBE=@debe, 
                    USUARIO_REGISTRA=@usReg, 
                    FECHA_REGISTRO=@feReg, 
                    USUARIO_MODIFICA=@usMod, 
                    FECHA_MODIFICA=sysdate() 
                where ID_DEUDA=@idDeu"
            };
            query.Parameters.AddWithValue("@idCli", deuda.IdCliente.ToString());
            query.Parameters.AddWithValue("@debe", deuda.Debe.ToString());
            query.Parameters.AddWithValue("@usReg", deuda.UsuarioRegistra);
            query.Parameters.AddWithValue("@feReg", deuda.FechaRegistra);
            
            try {
                query.ExecuteReader();
            }
            catch (MySqlException e) {
                MessageBox.Show($"Error (update deuda) exepción: {e.ToString()}", "Error");
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra una nueva deuda.
        /// </summary>
        /// <param name="newDeuda"></param>
        public static void NewDeuda(Deuda newDeuda) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"
                insert into deuda_mayoreo 
                    (ID_DEUDA, ID_CLIENTE, DEBE, 
                     USUARIO_REGISTRA, FECHA_REGISTRO, 
                     USUARIO_MODIFICA, FECHA_MODIFICA) 
                values (default, @idCli, @debe, 
                        @usReg, sysdate(), 
                        @usMod, sysdate())"
            };
            query.Parameters.AddWithValue("@idCli", newDeuda.IdCliente.ToString());
            query.Parameters.AddWithValue("@debe", newDeuda.Debe.ToString());
            query.Parameters.AddWithValue("@usReg", Database.GetUsername());
            query.Parameters.AddWithValue("@usMod", Database.GetUsername());

            try {
                query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
            }
            catch (MySqlException e) {
                MessageBox.Show($"Error (new Deuda) exepción: {e.ToString()}", "Error");
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
