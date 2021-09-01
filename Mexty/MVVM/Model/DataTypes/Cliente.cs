namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase para la tabla de Clientes Mayoreo.
    /// </summary>
    public class Cliente {
        private string nombre;
        private string _apPaterno;
        private string _apMaterno;
        private string _domicilio;

        /// <summary>
        /// Id de el cliente.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Nombre del cliente
        /// </summary>
        public string Nombre {
            get {
                char v = char.ToUpper(nombre[0]);
                return nombre = v + nombre.Substring(1);
            }
            set {
                nombre = value.Trim();
            }
        }

        /// <summary>
        /// Apellido paterno del cliente.
        /// </summary>
        public string ApPaterno {
            get {
                char v = char.ToUpper(_apPaterno[0]);
                return _apPaterno = v + _apPaterno.Substring(1);
            }
            set {
                _apPaterno = value.Trim();
            }
        }

        /// <summary>
        /// Apellido materno del cliente.
        /// </summary>
        public string ApMaterno {
            get {
                char v = char.ToUpper(_apMaterno[0]);
                return _apMaterno = v + _apMaterno.Substring(1);
            }
            set {
                _apMaterno = value.Trim();
            }
        }

        /// <summary>
        /// Domicilio del cliente.
        /// </summary>
        public string Domicilio {
            get => _domicilio;
            set => _domicilio = value.Trim();
        }

        /// <summary>
        /// Número de teléfono del cliente.
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Usuario que registró al cliente.
        /// </summary>
        public string UsuarioRegistra { get; set; }

        /// <summary>
        /// Fecha de registro del cliente.
        /// </summary>
        public string FechaRegistro { get; set; }

        /// <summary>
        /// El ultimo usuario que modifico a este cliente.
        /// </summary>
        public string UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de la ultima modificación a este cliente.
        /// </summary>
        public string FechaModifica { get; set; }

        /// <summary>
        /// Comentarios sobre el cliente.
        /// </summary>
        public string Comentario { get; set; }
        
        /// <summary>
        /// Monto que debe el cliente.
        /// </summary>
        public decimal Debe { get; set; }

        /// <summary>
        /// Evalua si dos clientes son el mismo, Mismo nombre y apellidos
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Cliente a, Cliente b) {
            if (a is null || b is null) return false;
            return a.Nombre == b.Nombre &&
                   a.ApPaterno == b.ApPaterno &&
                   a.ApMaterno == b.ApMaterno;
        }

        public static bool operator !=(Cliente a, Cliente b) {
            return !(a == b);
        }
    }
}