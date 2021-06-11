using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mexty.MVVM.Model.DataTypes
{
    public class Usuarios {
        /// <summary>
        /// Propiedades para acceder a los campos de la clase.
        /// Si necesitamos lógica para modifcar algun campo aqui se hace.
        /// </summary>
        public int Id { get; set; }
        public string Username { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public string Domicilio { get; set; }
        public int Telefono { get; set; }
        public int Activo { get; set; }
        public int IdTienda { get; set; }
        public int IdRol { get; set; }
        public string UsuraioRegistra  { get; set; }
        public string FechaRegistro  { get; set; }
        public string UsuarioModifica  { get; set; }
        public string FechaModifica { get; set; }
        
        /// <summary>
        /// Constructor principal de la clase Usuarios
        /// </summary>
        /// <param name="id">ID usuario</param>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="apPaterno">Apellido Paterno</param>
        /// <param name="apMaterno">Apellido Materno</param>
        /// <param name="usuario">Usuario Que registró</param>
        /// <param name="contraseña">Contraseña del usuario</param>
        /// <param name="domincio">Domicio del usuario</param>
        /// <param name="telefono">Telefono del usuario</param>
        /// <param name="activo">Usuario activo</param>
        /// <param name="idTienda">Tienda asignada</param>
        /// <param name="idRol">ID de rol del usuario</param>
        /// <param name="usuarioRegistra">Usuario que registo</param>
        /// <param name="fechaRegistro">Fecha de registro del usuario</param>
        /// <param name="usuarioModifica">Fecha de la última modificación al usuario</param>
        public Usuarios() {
            // this.Id = id;
            // this.Username = username;
            // this.ApMaterno = apMaterno;
            // this.ApPaterno = apPaterno;
            // this.Usuario = usuario;
            // this.Contraseña = contraseña;
            // this.Domicilio = domincio;
            // this.Telefono = telefono;
            // this.Activo = activo;
            // this.IdTienda = idTienda;
            // this.IdRol = idRol;
            // this.UsuraioRegistra = usuarioRegistra;
            // this.FechaRegistro = fechaRegistro;
            // this.UsuarioModifica = usuarioModifica;
        }
    }
}
