using System;

namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase usada para guardar los movimientos del inventario matriz.
    /// </summary>
    public class LogInventario {
        private string _mensaje;

        /// <summary>
        /// Id del elemento en el log.
        /// </summary>
        public int IdRegistro { get; set; }

        /// <summary>
        /// Mensaje del evento a guardar.
        /// </summary>
        public string Mensaje {
            get => _mensaje;
            set => _mensaje = value.Trim();
        }

        /// <summary>
        /// Usuario que registro la entrada al log.
        /// </summary>
        public string UsuarioRegistra { get; set; }

        /// <summary>
        /// Fecha de registro de la entrada al log.
        /// </summary>
        public DateTime FechaRegistro { get; set; }
    }
}