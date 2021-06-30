﻿namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase para la tabla de Clientes Mayoreo.
    /// </summary>
    public class Cliente {
        private string _nombre;
        private string _apPaterno;
        private string _apMaterno;
        private string _domicilio;
        private string _username;

        /// <summary>
        /// Id de el cliente.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Nombre del cliente
        /// </summary>
        public string Nombre {
            get => _nombre;
            set => _nombre = value.ToLower(); // TODO: quitar espacios al final.
        }

        /// <summary>
        /// Apellido paterno del cliente.
        /// </summary>
        public string ApPaterno {
            get => _apPaterno; 
            set => _apPaterno = value.ToLower().Replace(" ", "");
        }

        /// <summary>
        /// Apellido materno del cliente.
        /// </summary>
        public string ApMaterno {
            get => _apMaterno; 
            set => _apMaterno = value.ToLower().Replace(" ", "");
        }

        /// <summary>
        /// Domicilio del cliente.
        /// </summary>
        public string Domicilio {
            get => _domicilio; 
            set => _domicilio = value.ToLower();
        }

        /// <summary>
        /// Número de teléfono del cliente.
        /// </summary>
        public int Telefono { get; set; }

        /// <summary>
        /// Indica si el cliente esta activo.
        /// </summary>
        public int Activo { get; set; }

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
    }
}