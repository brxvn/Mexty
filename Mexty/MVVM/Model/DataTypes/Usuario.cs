using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mexty.MVVM.Model.DataTypes
{
    /// <summary>
    /// Clase Base para objetos tipo Usuarios.
    /// </summary>
    // TODO: Agregar restricciónes para modificar estos parametros aqui.
    // TODO: agregar que en el set, los campos de nombre, appaterno, apmaterno y domicilio sean minuscula y quitarlo de la clase database.
    // TODO: Refactorizar esta clase de Usuarios a Empleados idk.
    public class Usuarios {
        /// <summary>
        /// Id del empleado.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del empleado.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Apellido Paterno.
        /// </summary>
        public string ApPaterno { get; set; }
        /// <summary>
        /// Apellido Materno.
        /// </summary>
        public string ApMaterno { get; set; }
        /// <summary>
        /// Nick del empleado (username).
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Contraseña del emplado.
        /// </summary>
        public string Contraseña { get; set; }
        /// <summary>
        /// Domicilio.
        /// </summary>
        public string Domicilio { get; set; }
        /// <summary>
        /// Teléfono del empleado.
        /// </summary>
        public int Telefono { get; set; }
        /// <summary>
        /// Indica si el usuario esta activo o no.
        /// </summary>
        public int Activo { get; set; }
        /// <summary>
        /// Id de la tienda asignada al empleado.
        /// </summary>
        public int IdTienda { get; set; }
        /// <summary>
        /// Id del rol asignado al empleado.
        /// </summary>
        public int IdRol { get; set; }
        /// <summary>
        /// Nombre del Empleado que registro a este empleado.
        /// </summary>
        public string UsuraioRegistra  { get; set; }
        /// <summary>
        /// Fecha de registro de este empleado.
        /// </summary>
        public string FechaRegistro  { get; set; }
        /// <summary>
        /// Nombre del empleado que modifico por última vez a este empleado.
        /// </summary>
        public string UsuarioModifica  { get; set; }
        /// <summary>
        /// Fecha de la última modificación a este empleado.
        /// </summary>
        public string FechaModifica { get; set; }
        
    }
}
