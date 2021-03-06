using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Google.Protobuf.WellKnownTypes;
using Mexty.MVVM.Model.DatabaseQuerys;

namespace Mexty.MVVM.Model.DataTypes
{
    /// <summary>
    /// Clase Base para objetos tipo Usuario,
    /// </summary>
    public class Usuario {
        private string _nombre;
        private string _apPaterno;
        private string _apMaterno;
        private string _domicilio;

        /// <summary>
        /// Id del empleado.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del empleado.
        /// </summary>
        public string Nombre {
            get => _nombre;
            set => _nombre = value.Trim();
        }

        /// <summary>
        /// Apellido Paterno.
        /// </summary>
        public string ApPaterno {
            get => _apPaterno; 
            set => _apPaterno = value.Trim();
        }

        /// <summary>
        /// Apellido Materno.
        /// </summary>
        public string ApMaterno {
            get => _apMaterno; 
            set => _apMaterno = value.Trim();
        }

        /// <summary>
        /// Nick del empleado (username).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Método que genera el Username usando los primeros dos digitos de el nombre y
        /// del el apellido materno junto con el apellido paterno y un número random del 1 al 10.
        /// </summary>
        /// <returns></returns>
        public static string GenUsername(Usuario usr) {
            Random random = new();
            string username = $"{usr.Nombre.ToLower()}{usr.ApPaterno[..1].ToLower()}{usr.ApMaterno[..1].ToLower()}{random.Next(1, 10)}";

            var normalizedString = username.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString) {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Contraseña del emplado.
        /// </summary>
        public string Contraseña { get; set; }

        /// <summary>
        /// Domicilio del empleado.
        /// </summary>
        public string Domicilio {
            get => _domicilio; 
            set => _domicilio = value.Trim();
        }

        /// <summary>
        /// Teléfono del empleado.
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Id de la tienda asignada al empleado.
        /// </summary>
        public int IdTienda { get; set; }

        /// <summary>
        /// Id del rol asignado al empleado.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del Empleado que registro a este empleado. Manejado por <c>Database</c>.
        /// </summary>
        public string UsuraioRegistra { get; set; }

        /// <summary>
        /// Fecha de registro de este empleado.
        /// </summary>
        public string FechaRegistro { get; set; }

        /// <summary>
        /// Nombre del empleado que modifico por última vez a este empleado. Manejado por <c>Database</c>.
        /// </summary>
        public string UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de la última modificación a este empleado, Manejado por <c>Database</c>.
        /// </summary>
        public string FechaModifica { get; set; }

        /// <summary>
        /// Lista estatica que contiene la lista de sucursales para más rapido acceso.
        /// </summary>
        private static List<Sucursal> ListaSucursal { get; set; }

        /// <summary>
        /// Actualiza la lista de sucursales.
        /// </summary>
        public void UpdateListaSucursal() {
            ListaSucursal = QuerysSucursales.GetTablesFromSucursales();
        }

        /// <summary>
        /// Campo que da el nombre de la tienda en letra, Solo Get.
        /// </summary>
        public string SucursalNombre {
            get {
                ListaSucursal ??= QuerysSucursales.GetTablesFromSucursales();
                var nombre = "";
                if (IdTienda == 0) {
                    return "general";
                }
                for (var index = 0; index < ListaSucursal.Count; index++) {
                    var sucursal = ListaSucursal[index];

                    if (IdTienda == sucursal.IdTienda) {
                        nombre = sucursal.NombreTienda;
                    }
                }

                return nombre;
            }
        }

        /// <summary>
        /// Sobrecarga de opreadores para objetos tipo Usuario, para saber si un usuario es el mismo que otro.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Usuario a, Usuario b) {
            if (a is null || b is null) return false;
            return a.Nombre == b.Nombre &&
                   a.ApPaterno == b.ApPaterno &&
                   a.ApMaterno == b.ApMaterno;
        }

        public static bool operator !=(Usuario a, Usuario b) {
            return !(a == b);
        }

        /// <summary>
        /// Escribe los campos no directamente editables del por el usuario de <c>b</c> a <c>a</c>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Un usuario con la información actualizada y en el caso de un error un valor null.</returns>
        public static Usuario operator +(Usuario a, Usuario b) {
            var resultado = new Usuario() {
                //no editables directamente
                Id = b.Id,
                UsuraioRegistra = b.UsuraioRegistra,
                FechaRegistro = b.FechaRegistro,
                // Identificadores de usuario
                Nombre = b.Nombre, 
                ApPaterno = b.ApPaterno,
                ApMaterno = b.ApMaterno,
                // Editables directamente por el usuario
                Contraseña = a.Contraseña,
                Domicilio = a.Domicilio,
                Telefono = a.Telefono,
                IdTienda = a.IdTienda,
                IdRol = a.IdRol,
            };
            
            return resultado;
        }

        /// <summary>
        /// Escribe los campos no editables Id, Activo, UsuarioRegistra y FechaRegistro de <c>b</c> a <c>a</c>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Usuario operator -(Usuario a, Usuario b) {
            var resultado = new Usuario {
                //no editables
                Id = b.Id,
                UsuraioRegistra = b.UsuraioRegistra,
                FechaRegistro = b.FechaRegistro,
                
                // editables
                Nombre = a.Nombre, 
                ApPaterno = a.ApPaterno,
                ApMaterno = a.ApMaterno,
                Contraseña = a.Contraseña,
                Domicilio = a.Domicilio,
                Telefono = a.Telefono,
                IdTienda = a.IdTienda,
                IdRol = a.IdRol,
            };
            return resultado;
        }
    }
}
