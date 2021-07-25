using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using MySql.Data.MySqlClient;
using Mexty.MVVM.Model.DataTypes;
using System.Windows;
using System.Windows.Documents;
using log4net;
using Org.BouncyCastle.Ocsp;
using MySql.Data.MySqlClient; 

namespace Mexty.MVVM.Model {
    /// <summary>
    /// La clase principal de base de datos.
    /// Contiene todos los métodos necesarios para la conección y uso de la Base de datos.
    /// </summary>
    // TODO: Hacer la clase Database estacica y re implementar login.
    public class Database {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
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
        /// ID de la tienda actual.
        /// </summary>
        private static int IdTienda { get; set; }

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

            GetIdTiendaActual();
        }

        /// <summary>
        /// Método que Lee el contenido del archivo ini para la connección.
        /// </summary>
        /// <returns>String con la información de log-in a la base de datos</returns>
        private static string ConnectionInfo() {
            try {
                var myIni = new IniFile(@"C:\Mexty\Settings.ini");
                var user = myIni.Read("DbUser");
                var pass = myIni.Read("DbPass");
                var connString = $"server=localhost; database = mexty; Uid={user}; pwd ={pass}";
                Log.Debug("Se han leido las credenciales del ini exitosamente.");
                return connString;
            }
            catch (Exception e) {
                Log.Error($"Error al leer del ini {e.Message}");
                return "";
                //throw;
                // TODO: probablemente sacar un anuncio de error diciendo que algo anda mal con el ini.
            }
        }

        /// <summary>
        /// Método que obtiene el Id de la tienda en la que se ejecuta el programa.
        /// </summary>
        /// <returns></returns>
        private static void GetIdTiendaActual() {
            try {
                var myIni = new IniFile(@"C:\Mexty\Settings.ini");
                var sucursal = myIni.Read("IdTienda");
                IdTienda = int.Parse(sucursal);
                Log.Info("Se ha leido exitosamente el id de la tienda del ini.");
                
                var connObj = new MySqlConnection(ConnectionInfo());
                connObj.Open();
                var query = new MySqlCommand() {
                    Connection = connObj,
                    CommandText = "select nombre_tienda from cat_tienda where ID_TIENDA=@id"
                };
                query.Parameters.AddWithValue("@id", sucursal);
                var res = query.ExecuteReader();
                if (res.Read().ToString().ToLower() == "false") {
                    Log.Error("No se ha podido validar el id de la tienda escrito en el ini.");
                    throw new Exception();
                } 
            }

            catch (Exception e) {
                Log.Error("Ha ocurrido un error al leer el Id de la tienda en el ini.");
                Log.Error($"Error: {e.Message}");
                // TODO: manerjar mejor el error.
            }
        }

        /// <summary>
        /// Inicializa los campos estaticos de la clase.
        /// </summary>
        private static void InitializeFields() {
            try {
                _firstQuery.Read();
                Username= _firstQuery.GetString("usuario");
                Rol = int.Parse(_firstQuery.GetString("id_rol"));
                Password = _firstQuery.GetString("contrasenia");
                if (Username != "" && Password != "") {
                    Log.Debug("Campos estaticos de database inicializados con exito.");
                    ConnectionSuccess = true;
                }
            }
            catch (Exception e) {
                Log.Error($"No se han podido inicializar los los campos necesarios para el logIn {e.Message}");
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
        /// Método que retorna el Id de la tienda actual, leido por el ini.
        /// </summary>
        /// <returns></returns>
        public static int GetIdTienda() {
            return IdTienda;
        }

        /// <summary>
        /// Método que obtiene la hora actual en formato Msql friendly
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentTimeNDate() {
            var currentTime = DateTime.Now;
            return currentTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Método que cierra la conección con la base de datos.
        /// </summary>
        public static void CloseConnection() {
            try {
                _sqlSession.Close();
                Username = null;
                Password = null;
                Rol = 0;
                ConnectionSuccess = false;
                Log.Info("Conección con base de datos cerrada con exito.");
            }
            catch (Exception e) {
                Log.Error("Hubo un problema al cerrar la conección y borrar los campos estaticos de base de dato.");
                Log.Error($"Excepción: {e.Message}");
            }
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
                Log.Debug("Se han obtenido con exito los datos de la tabla usuarios");
                connObj.Close();
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un problema al obtener los datos de la tabla usuarios.");
                Log.Error($"Excepción: {e.Message}");
            }

            return users;
        }

        /// <summary>
        /// Método para actualziar los datos de Usuario
        /// </summary>
        public static int UpdateData(Usuario usuario) {
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
                        FECHA_MODIFICA=@date
                    where ID_USUARIO=@ID"
            };

            query.Parameters.AddWithValue("@nomUsr", usuario.Nombre);
            query.Parameters.AddWithValue("@apPat", usuario.ApPaterno);
            query.Parameters.AddWithValue("@apMat", usuario.ApMaterno);
            query.Parameters.AddWithValue("@idTi",usuario.IdTienda.ToString()); // evitamos boxing//evitamos boxing.
            query.Parameters.AddWithValue("@dom", usuario.Domicilio);
            query.Parameters.AddWithValue("@pass", usuario.Contraseña);
            query.Parameters.AddWithValue("@tel", usuario.Telefono);
            query.Parameters.AddWithValue("@ID", usuario.Id.ToString());
            query.Parameters.AddWithValue("@act", usuario.Activo.ToString());
            query.Parameters.AddWithValue("@idRo",usuario.IdRol.ToString());
            query.Parameters.AddWithValue("@uMod", GetUsername());
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha actualizado el usuario exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar el usuario");
                Log.Error($"Error: {e.Message}");
                return 0;
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
                            @date, 
                            @usrMod, 
                            @date)"
            };

            query.Parameters.AddWithValue("@nombre", newUser.Nombre);
            query.Parameters.AddWithValue("@apPat", newUser.ApPaterno);
            query.Parameters.AddWithValue("@apMat", newUser.ApMaterno);
            query.Parameters.AddWithValue("@usr", newUser.Username);
            query.Parameters.AddWithValue("@pass", newUser.Contraseña);
            query.Parameters.AddWithValue("@dom", newUser.Domicilio);
            query.Parameters.AddWithValue("@tel", newUser.Telefono);
            query.Parameters.AddWithValue("@act", newUser.Activo.ToString());//evitamos boxing
            query.Parameters.AddWithValue("@idT", newUser.IdTienda.ToString());
            query.Parameters.AddWithValue("@idR", newUser.IdRol.ToString());
            query.Parameters.AddWithValue("@usrReg", GetUsername());
            query.Parameters.AddWithValue("@usrMod", GetUsername());
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha creado un nuevo usuario exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta un un nuevo usuario.");
                Log.Error($"Error: {e.Message}");
                return 0;
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
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var sucursal = new Sucursal {
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                        NombreTienda = reader.IsDBNull("nombre_tienda") ? "" : reader.GetString("nombre_tienda"),
                        Dirección = reader.IsDBNull("direccion") ? "" : reader.GetString("direccion"),
                        Telefono = reader.IsDBNull("telefono") ? "" : reader.GetString("telefono"),
                        Rfc = reader.IsDBNull("rfc") ? "" : reader.GetString("rfc"),
                        Mensaje = reader.IsDBNull("mensaje") ? "" : reader.GetString("mensaje"),
                        Facebook = reader.IsDBNull("facebook") ? "" : reader.GetString("facebook"),
                        Instagram = reader.IsDBNull("instagram") ? "" : reader.GetString("instagram"),
                        TipoTienda = reader.IsDBNull("tipo_tienda") ? "" : reader.GetString("tipo_tienda"),
                        Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo")
                    };
                    sucursales.Add(sucursal);
                    Log.Debug("Se han obtenido con exito las tablas de sucursales.");
                }
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de surcursales.");
                Log.Error($"Error: {e.Message}");
            }
            finally {
                connObj.Close();
            }
            
            return sucursales;
        }

        
        /// <summary>
        /// Método que actualiza una sucursal en la base de datos
        /// </summary>
        /// <param name="sucursal"></param>
        public static int UpdateData(Sucursal sucursal) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cat_tienda 
                set NOMBRE_TIENDA=@nom, 
                    DIRECCION=@dir, 
                    TELEFONO=@tel, 
                    RFC=@rfc, 
                    MENSAJE=@msg, 
                    FACEBOOK=@face, 
                    INSTAGRAM=@inst,
                    TIPO_TIENDA=@suc,
                    ACTIVO=@act
                where ID_TIENDA=@id"
            };
            
            query.Parameters.AddWithValue("@nom", sucursal.NombreTienda);
            query.Parameters.AddWithValue("@dir", sucursal.Dirección);
            query.Parameters.AddWithValue("@tel", sucursal.Telefono);
            query.Parameters.AddWithValue("@rfc", sucursal.Rfc);
            query.Parameters.AddWithValue("@msg", sucursal.Mensaje);
            query.Parameters.AddWithValue("@face", sucursal.Facebook);
            query.Parameters.AddWithValue("@inst", sucursal.Instagram);
            query.Parameters.AddWithValue("@suc", sucursal.TipoTienda);
            query.Parameters.AddWithValue("@act", sucursal.Activo.ToString());
            query.Parameters.AddWithValue("@id", sucursal.IdTienda.ToString());
            
            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha actualizado la sucursal exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar la sucursal.");
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra una nueva sucursal.
        /// </summary>
        /// <param name="newSucursal"></param>
        public static int NewSucursal(Sucursal newSucursal) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                insert into cat_tienda 
                    (ID_TIENDA, NOMBRE_TIENDA, DIRECCION, TELEFONO, 
                     RFC, 
                     MENSAJE, 
                     FACEBOOK, INSTAGRAM, TIPO_TIENDA, ACTIVO) 
                values (default, @nom, @dir, @tel, 
                        @rfc, 
                        @msg, 
                        @face, @insta, @tTienda, @act)"
            };
            query.Parameters.AddWithValue("@nom", newSucursal.NombreTienda);
            query.Parameters.AddWithValue("@dir", newSucursal.Dirección);
            query.Parameters.AddWithValue("@tel", newSucursal.Telefono);
            query.Parameters.AddWithValue("@rfc", newSucursal.Rfc);
            query.Parameters.AddWithValue("@msg", newSucursal.Mensaje);
            query.Parameters.AddWithValue("@face", newSucursal.Facebook);
            query.Parameters.AddWithValue("@insta", newSucursal.Instagram);
            query.Parameters.AddWithValue("@tTienda", newSucursal.TipoTienda);
            query.Parameters.AddWithValue("@act", 1.ToString());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
                Log.Info("Se ha creado una nueva sucursal exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta una nueva sucursal.");
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
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
            try {
                using var reader = querry.ExecuteReader();
                while (reader.Read()) {
                    var rol = new Rol() {
                        IdRol = reader.IsDBNull("id_rol") ? 0 : reader.GetInt32("id_rol"),
                        RolDescription = reader.IsDBNull("desc_rol") ? "" : reader.GetString("desc_rol"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda")
                    };
                    roles.Add(rol);
                }

                Log.Debug("Se han obtenido con exito las tablas de roles.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de roles.");
                Log.Error($"Error: {e.Message}");
            }
            finally {
                connObj.Close();
            }

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
            try {
                using var reader = querry.ExecuteReader();
                while (reader.Read()) {
                    var producto = new Producto {
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        MedidaProducto = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        TipoVenta = reader.IsDBNull("tipo_venta") ? 0 : reader.GetInt32("tipo_venta"),
                        PrecioMayoreo = reader.IsDBNull("precio_mayoreo") ? 0 : reader.GetFloat("precio_mayoreo"),
                        PrecioMenudeo = reader.IsDBNull("precio_menudeo") ? 0 : reader.GetFloat("precio_menudeo"),
                        DetallesProducto = 
                            reader.IsDBNull("especificacion_producto") ? "" : reader.GetString("especificacion_producto"),
                        Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo"),
                    };
                    productos.Add(producto);
                }

                Log.Debug("Se han obtenido con exito las tablas de productos.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de productos.");
                Log.Error($"Error: {e.Message}");
            }
            finally {
                connObj.Close();
            }

            return productos;
        }

        /// <summary>
        /// Método que actualiza un producto en la base de datos.
        /// </summary>
        /// <param name="producto"></param>
        public static int UpdateData(Producto producto) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cat_producto 
                set NOMBRE_PRODUCTO=@nom, 
                    TIPO_PRODUCTO=@tipoP, 
                    MEDIDA=@med,
                    PIEZAS=@piezas,
                    TIPO_VENTA=@tipoV, 
                    PRECIO_MAYOREO=@pMayo, 
                    PRECIO_MENUDEO=@pMenu, 
                    ESPECIFICACION_PRODUCTO=@esp, 
                    ACTIVO=@act
                where ID_PRODUCTO=@id"
            };

            query.Parameters.AddWithValue("@nom", producto.NombreProducto);
            query.Parameters.AddWithValue("@tipoP", producto.TipoProducto);
            query.Parameters.AddWithValue("@med", producto.MedidaProducto);
            query.Parameters.AddWithValue("@piezas", producto.Piezas.ToString());
            query.Parameters.AddWithValue("@tipoV", producto.TipoVenta.ToString());
            query.Parameters.AddWithValue("@pMayo", producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@pMenu", producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@esp", producto.DetallesProducto);
            query.Parameters.AddWithValue("@id", producto.IdProducto.ToString());
            query.Parameters.AddWithValue("@act", producto.Activo.ToString());

            try {
                var cmd = query.CommandText;
                for (var index = 0; index < query.Parameters.Count; index++) {
                    var queryParameter = query.Parameters[index];
                    cmd = cmd.Replace(
                        queryParameter.ParameterName,
                        queryParameter.ParameterName is "@pMayo" or "@pMenu" ? "0" : queryParameter.Value?.ToString());
                }
                Log.Debug("Se ha obtenido la querry.");

                var exit = SaveQuery(cmd);
                if (exit == 0) throw new Exception();
                Log.Debug("Se ha guardado la querry en sincroniza.");

                var res = query.ExecuteNonQuery();
                Log.Info("Se han actualizado los datos de producto exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar los datos en la tabla producto.");
                Log.Error($"Error: {e.Message}");
                throw;
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra un nuevo producto.
        /// </summary>
        /// <param name="newProduct">Objeto tipo <c>Producto</c>.</param>
        public static int NewProduct(Producto newProduct) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                insert into cat_producto 
                    (ID_PRODUCTO, NOMBRE_PRODUCTO, MEDIDA, TIPO_PRODUCTO, PIEZAS,
                     TIPO_VENTA, 
                     PRECIO_MAYOREO, PRECIO_MENUDEO, 
                     ESPECIFICACION_PRODUCTO, ACTIVO) 
                values (default, @nom, @medida, @tipoP, @piezas,
                        @tipoV, 
                        @pMayo, @pMenu, 
                        @esp, @act)"
            };

            query.Parameters.AddWithValue("@nom", newProduct.NombreProducto);
            query.Parameters.AddWithValue("@medida", newProduct.MedidaProducto);
            query.Parameters.AddWithValue("@piezas", newProduct.Piezas.ToString());
            query.Parameters.AddWithValue("@tipoP", newProduct.TipoProducto);
            query.Parameters.AddWithValue("@tipoV", newProduct.TipoVenta.ToString());
            query.Parameters.AddWithValue("@pMayo", newProduct.PrecioMayoreo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@pMenu", newProduct.PrecioMenudeo.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@esp", newProduct.DetallesProducto);
            query.Parameters.AddWithValue("@act", 1.ToString());

            try {
                var cmd = query.CommandText;
                for (var index = 0; index < query.Parameters.Count; index++) {
                    var queryParameter = query.Parameters[index];
                    cmd = cmd.Replace(
                        queryParameter.ParameterName,
                        queryParameter.ParameterName is "@pMayo" or "@pMenu" ? "0" : queryParameter.Value?.ToString());
                }
                Log.Debug("Se ha obtenido la querry.");

                var exit = SaveQuery(cmd);
                if (exit == 0) throw new Exception();
                Log.Debug("Se ha guardado la querry en sincroniza.");

                var res = query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
                Log.Info("Se ha dado de alta un nuevo produto exitosamente.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo producto.");
                Log.Error($"Error: {e.Message}");
                return 0;
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
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var cliente = new Cliente() {
                        IdCliente = reader.IsDBNull("id_cliente") ? 0 : reader.GetInt32("id_cliente"),
                        Nombre = reader.IsDBNull("nombre_cliente") ? "" : reader.GetString("nombre_cliente"),
                        ApPaterno = reader.IsDBNull("ap_paterno") ? "" : reader.GetString("ap_paterno"),
                        ApMaterno = reader.IsDBNull("ap_materno") ? "" : reader.GetString("ap_materno"),
                        Domicilio = reader.IsDBNull("domicilio") ? "" : reader.GetString("domicilio"),
                        Telefono = reader.IsDBNull("telefono") ? "" : reader.GetString("telefono"),
                        Activo = reader.IsDBNull("activo") ? 0 : reader.GetInt32("activo"),
                        UsuarioRegistra = reader.IsDBNull("usuario_registra") ? "" : reader.GetString("usuario_registra"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                        UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                        FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        Debe = reader.IsDBNull("debe") ? 0 : reader.GetFloat("debe")
                    };
                    clientes.Add(cliente);
                }
                Log.Debug("Se han obtenido con exito las tablas de clientes.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de clientes.");
                Log.Error($"Error: {e.Message}");
            }
            finally{
                connObj.Close();
            }
            return clientes;
        }

        /// <summary>
        /// Método para actualizar los datos del Cliente.
        /// </summary>
        /// <param name="cliente"></param>
        public static int UpdateData(Cliente cliente) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update cliente_mayoreo 
                set NOMBRE_CLIENTE=@nom, AP_PATERNO=@apP, AP_MATERNO=@apM, 
                    DOMICILIO=@dom, TELEFONO=@tel, ACTIVO=@act, 
                    USUARIO_MODIFICA=@usMod, FECHA_MODIFICA=@date, 
                    COMENTARIO=@com, DEBE=@debe
                where ID_CLIENTE=@id"
            };
            query.Parameters.AddWithValue("@nom", cliente.Nombre);
            query.Parameters.AddWithValue("@apP", cliente.ApPaterno);
            query.Parameters.AddWithValue("@apM", cliente.ApMaterno);
            query.Parameters.AddWithValue("@dom", cliente.Domicilio);
            query.Parameters.AddWithValue("@tel", cliente.Telefono);
            query.Parameters.AddWithValue("@act", cliente.Activo.ToString());
            query.Parameters.AddWithValue("@usMod", GetUsername());
            query.Parameters.AddWithValue("@id", cliente.IdCliente.ToString());
            query.Parameters.AddWithValue("@com", cliente.Comentario);
            query.Parameters.AddWithValue("@debe", cliente.Debe.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se han actualizado los datos de cliente de manera exitosa.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al actualizar los datos de cliente.");
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra un nuevo cliente.
        /// </summary>
        /// <param name="newClient"></param>
        public static int NewClient(Cliente newClient) {
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
                     comentario, DEBE) 
                values (default, @nom, @apP, @apM, 
                        @dom, @tel, @act, 
                        @usReg, @date, 
                        @usMod, @date, 
                        @com, @debe)"
            };
            query.Parameters.AddWithValue("@nom", newClient.Nombre);
            query.Parameters.AddWithValue("@apP", newClient.ApPaterno);
            query.Parameters.AddWithValue("@apM", newClient.ApMaterno);
            query.Parameters.AddWithValue("@dom", newClient.Domicilio);
            query.Parameters.AddWithValue("@tel", newClient.Telefono);
            query.Parameters.AddWithValue("@act", 1.ToString());
            query.Parameters.AddWithValue("@usReg", GetUsername());
            query.Parameters.AddWithValue("@usMod", GetUsername());
            query.Parameters.AddWithValue("@com", newClient.Comentario);
            query.Parameters.AddWithValue("@debe", newClient.Debe.ToString(CultureInfo.InvariantCulture));
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery(); // retorna el número de columnas cambiadas.
                Log.Info("Se ha dado de alta un nuevo cliente de manera exitosa.");
                return res;
            }
            catch (MySqlException e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo cliente.");
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
        }

        // ==============================================
        // ------- Querrys de Inventario  ---------------
        // ==============================================


        /// <summary>
        /// Método para optener las tablas de inventario.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInventario> GetTablesFromInventario() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();

            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"select * from inventario where ID_TIENDA=@id"
            };
            query.Parameters.AddWithValue("@id", IdTienda.ToString());

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        UsuarioRegistra = reader.IsDBNull("USUARIO_REGISTRA") ? "" : reader.GetString("USUARIO_REGISTRA"),
                        FechaRegistro = reader.IsDBNull("fecha_registro") ? "" : reader.GetString("fecha_registro"),
                        UsuarioModifica = reader.IsDBNull("usuario_modifica") ? "" : reader.GetString("usuario_modifica"),
                        FechaModifica = reader.IsDBNull("fecha_modifica") ? "" : reader.GetString("fecha_modifica")
                    };
                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario.");
                Log.Error($"Error: {e.Message}");
            }
            finally {
                connObj.Close();
            }

            return items;
        }

        /// <summary>
        /// Método para obtener la información conjunta de inventario y de cat_producto.
        /// </summary>
        /// <returns></returns>
        public static List<ItemInventario> GetItemsFromInventario() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                SELECT p.id_producto,
                       p.tipo_producto,
                       p.nombre_producto,
                       p.medida,
                       p.piezas AS MAX_PIEZAS,
                       i.ID_REGISTRO,
                       i.piezas,
                       i.cantidad,
                       i.comentario,
                       i.ID_TIENDA
                FROM   cat_producto p,
                       inventario i
                WHERE  p.id_producto = i.id_producto
                       and i.ID_TIENDA=@idT"
            };
            query.Parameters.AddWithValue("@idT", IdTienda.ToString());

            var items = new List<ItemInventario>();
            try {
                using var reader = query.ExecuteReader();
                while (reader.Read()) {
                    var item = new ItemInventario() {
                        IdRegistro = reader.IsDBNull("id_registro") ? 0 : reader.GetInt32("id_registro"),
                        IdProducto = reader.IsDBNull("id_producto") ? 0 : reader.GetInt32("id_producto"),
                        TipoProducto = reader.IsDBNull("tipo_producto") ? "" : reader.GetString("tipo_producto"),
                        NombreProducto = reader.IsDBNull("nombre_producto") ? "" : reader.GetString("nombre_producto"),
                        Medida = reader.IsDBNull("medida") ? "" : reader.GetString("medida"),
                        Cantidad = reader.IsDBNull("cantidad") ? 0 : reader.GetInt32("cantidad"),
                        Piezas = reader.IsDBNull("piezas") ? 0 : reader.GetInt32("piezas"),
                        MaxPiezas = reader.IsDBNull("max_piezas") ? 0 : reader.GetInt32("max_piezas"),
                        Comentario = reader.IsDBNull("comentario") ? "" : reader.GetString("comentario"),
                        IdTienda = reader.IsDBNull("id_tienda") ? 0 : reader.GetInt32("id_tienda"),
                    };
                    items.Add(item);
                }

                Log.Debug("Se han obtenido con exito las tablas de inventario-productos.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener las tablas de inventario-productos.");
                Log.Error($"Error: {e.Message}");
            }
            finally {
                connObj.Close();
            }

            return items;
        }

        /// <summary>
        /// Método para actualizar los datos de la tabla inventario-general.
        /// </summary>
        /// <param name="item"></param>
        public static int UpdateData(ItemInventario item) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();
            var query = new MySqlCommand() {
                Connection = connObj,
                CommandText = @"
                update inventario
                set ID_PRODUCTO=@idP,
                    CANTIDAD=@can, PIEZAS=@pieza,
                    COMENTARIO=@comentario, ID_TIENDA=@idT,
                    USUARIO_MODIFICA=@usrM, FECHA_MODIFICA=@date
                where ID_REGISTRO=@idR"
            };

            query.Parameters.AddWithValue("@idR", item.IdRegistro.ToString());
            query.Parameters.AddWithValue("@idP", item.IdProducto.ToString());
            query.Parameters.AddWithValue("@tipo", item.TipoProducto);
            query.Parameters.AddWithValue("@med", item.Medida);
            query.Parameters.AddWithValue("@can", item.Cantidad.ToString());
            query.Parameters.AddWithValue("@pieza", item.Piezas.ToString());
            query.Parameters.AddWithValue("@comentario", item.Comentario);
            query.Parameters.AddWithValue("@idT", IdTienda.ToString());
            query.Parameters.AddWithValue("@usrM", GetUsername());
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se han actualizado los datos de la tabla inventario-general de manera exitosa.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al actualizar los datos de inventario-general.");
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
        }

        /// <summary>
        /// Método que registra un nuevo item en la tabla de inventario-general.
        /// </summary>
        /// <param name="newItem"></param>
        public static int NewItem(ItemInventario newItem) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"
                insert into inventario
                    (ID_REGISTRO, ID_PRODUCTO,
                     CANTIDAD, PIEZAS,
                     COMENTARIO, ID_TIENDA,
                     usuario_registra, fecha_registro, 
                     usuario_modifica, fecha_modifica) 
                values (default, @idP,
                        @cant, @piezas,
                        @comentario, @idT,
                        @usReg, @date, 
                        @usMod, @date)"
            };

            query.Parameters.AddWithValue("@idP", newItem.IdProducto.ToString());
            query.Parameters.AddWithValue("@cant", newItem.Cantidad.ToString());
            query.Parameters.AddWithValue("@piezas", newItem.Piezas.ToString());
            query.Parameters.AddWithValue("@comentario", newItem.Comentario);
            query.Parameters.AddWithValue("@idT", IdTienda.ToString());
            query.Parameters.AddWithValue("@usReg", GetUsername());
            query.Parameters.AddWithValue("@usMod", GetUsername());
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                ProcessQuery(query);

                var res = query.ExecuteNonQuery();
                Log.Info("Se ha dado de alta un nuevo item en el inventario-general de manera exitosa.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al dar de alta un nuevo item en el inventario-general."); 
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
        }


        // =========================================================
        // ------- Querrys De modulo Base de datos  ---------------
        // =========================================================

        /// <summary>
        /// Método que vacia los contenidos de la tabla control_sincbd a un script sql para ser aplicados en otra base de datos.
        /// </summary>
        public static void DumpDeltas() {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();

            // Obtenemos la fecha del primer cambio guardado
            // Igual y no es necesaria
            MySqlCommand query0 = new() {
                Connection = connObj,
                CommandText = @"select fecha_sinc from control_sincbd where ID_REGISTRO=1;"
            };

            // Obtenemos las querys y las fechas de cada una
            // La idea es crear una lista de objetos tipo dump e ir escribiendo.
            MySqlCommand query1 = new() {
                Connection = connObj,
                CommandText = @"select QUERY, fecha_sinc from control_sincbd;"
            };

            // Vaciamos la tabla.
            MySqlCommand query2 = new() {
                Connection = connObj,
                CommandText = @"truncate table control_sincbd;"
            };

            // Reseteamos el auto increment del id
            MySqlCommand query3 = new() {
                Connection = connObj,
                CommandText = @"ALTER TABLE control_sincbd AUTO_INCREMENT = 1"
            };

            var time = DateTime.Today;
            //TODO: agregar la fecha del dia del dump y la fecha de la primera sincronizacion;
            var fileName = $"DBChangesFrom{time:dd-MM-yy}.sql";
            const string path = @"C:\Mexty\Backups\";
            Directory.CreateDirectory(path);
            var file = $"{path}{fileName}";

        }


        /// <summary>
        /// Método que recibe un objeto tipo MySqlCommand, obtiene la querry y remplaza los parametros para guardarse.
        /// </summary>
        /// <param name="query">Objeto tipo <c>MySqlCommand</c>.</param>
        /// <exception cref="Exception"></exception>
        private static void ProcessQuery(MySqlCommand query) {
            try {
                var cmd = query.CommandText;
                for (var index = 0; index < query.Parameters.Count; index++) {
                    var queryParameter = query.Parameters[index];
                    cmd = cmd.Replace(queryParameter.ParameterName, queryParameter.Value?.ToString());
                }
                Log.Debug("Se ha obtenido la querry.");

                var exit = SaveQuery(cmd);
                if (exit == 0) throw new Exception();
                Log.Debug("Se ha guardado la querry en sincroniza.");
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al obtener la querry y replazar los parametros de esta.");
                Log.Error($"Error: {e.Message}");
            }
        }


        /// <summary>
        /// Método que guarda la querry dada en la tabla de control_sincbd
        /// </summary>
        /// <returns></returns>
        private static int SaveQuery(string cmd) {
            var connObj = new MySqlConnection(ConnectionInfo());
            connObj.Open();

            MySqlCommand query = new() {
                Connection = connObj,
                CommandText = @"insert into control_sincbd
                                values (default, @query, @date)"
            };

            // String sanitization
            var strReader = new StringReader(cmd);
            var cmdNew = "";
            while (true) {
                var line = strReader.ReadLine();
                if (line != null) {
                    cmdNew = $"{cmdNew}{line.TrimStart()}";
                }
                else {
                    cmdNew = $"{cmdNew};";
                    break;
                }
            }

            query.Parameters.AddWithValue("@query", cmdNew);
            query.Parameters.AddWithValue("@date", GetCurrentTimeNDate());

            try {
                var res = query.ExecuteNonQuery();
                Log.Info("Se ha guardado la query exitosamente.");
                return res;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al guardar la query.");
                Log.Error($"Error: {e.Message}");
                return 0;
            }
            finally {
                connObj.Close();
            }
        }


        /// <summary>
        /// Metodo a usar para exportar toda la base de datos.
        /// </summary>
        // info: https://github.com/MySqlBackupNET/MySqlBackup.Net/wiki
        // TODO: quizá agregar un combo para controlar si exportar toda la base de datos o solo la estructura.
        public static bool BackUp() {
            Log.Info("Se ha empezado el backUp de la base de datos.");
            try {
                var time = DateTime.Today;
                var fileName = $"FullBackupBD{time:dd-MM-yy}.sql";
                const string path = @"C:\Mexty\Backups\FullBackUp";
                Directory.CreateDirectory(path);
                var file = $"{path}{fileName}";
                using (MySqlConnection conn = new MySqlConnection(ConnectionInfo())) {
                    using (MySqlCommand cmd = new MySqlCommand()) {
                        using (MySqlBackup mb = new MySqlBackup(cmd)) {
                            cmd.Connection = conn;
                            conn.Open();
                            //mb.ExportInfo.ExcludeTables = new List<string>() {"inventario", "export"};
                            //mb.ExportInfo.ExportRows = false; // Exporta solo la estructura.
                            mb.ExportToFile(file);
                            Log.Debug("Se ha exportado el archivo exitosamente.");
                            conn.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al intentar exportar la base de datos.");
                Log.Error($"Error: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Importa un archivo SQL para ejecutarlo
        /// </summary>
        public static bool Import(string file) {
            Log.Info("Se ha empezado el proceso de Importar un archivo SQL.");
            try {
                using (MySqlConnection conn = new MySqlConnection(ConnectionInfo())) {
                    using (MySqlCommand cmd = new MySqlCommand()) {
                        using (MySqlBackup mb = new MySqlBackup(cmd)) {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ImportFromFile(file);
                            Log.Debug("Se ha importado el archivo Exitosamente.");
                            conn.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception e) {
                Log.Error("Ha ocurrido un error al intentar importar y ejecutar el archivo SQL.");
                Log.Error($"Error: {e.Message}");
                throw;
            }
        }


        // ============================================
        // ------- Métodos De la clase ----------------
        // ============================================

    }
}
