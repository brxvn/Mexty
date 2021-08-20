using System;

namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase usada para guardar los elementos tipo Log de movimientos de cliente.
    /// </summary>
    public class LogCliente {

        /// <summary>
        /// ID del registro.
        /// </summary>
        public int IdRegistro { get; set; }

        /// <summary>
        /// Id del cliente al que pertenece este registro.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Mensaje del log.
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// El usuario que hizo este registro.
        /// </summary>
        public string UsuarioRegistra { get; set; }

        /// <summary>
        /// Fecha del registro
        /// </summary>
        public DateTime FechaRegistro { get; set; }
    }
}