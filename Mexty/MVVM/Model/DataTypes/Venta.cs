using System;
using System.Windows.Controls;

namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase base para objetos tipo venta.
    /// </summary>
    public class Venta {
        private string _comentario;

        /// <summary>
        /// Id de venta.
        /// </summary>
        public int IdVenta { get; set; }

        /// <summary>
        /// Id de cliente (solo venta mayoreo).
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Detalle codificado de la venta hecha.
        /// </summary>
        public string DetalleVenta { get; set; }

        /// <summary>
        /// Cantidad total final de la venta.
        /// </summary>
        public decimal TotalVenta { get; set; }

        /// <summary>
        /// Cantidad total pagada en la venta.
        /// </summary>
        public decimal Pago { get; set; }

        /// <summary>
        /// Cambio dado.
        /// </summary>
        public decimal Cambio { get; set; }

        /// <summary>
        /// Debe. (solo mayoreo).
        /// </summary>
        // TODO: ver para que es.
        public decimal Debe { get; set; }

        /// <summary>
        /// Comentarios de la venta. (solo mayoreo).
        /// </summary>
        public string Comentarios {
            get => _comentario;
            set => _comentario = value.Trim();
        }

        /// <summary>
        /// Id de la tienda donde se hizo la venta.
        /// </summary>
        public int IdTienda { get; set; }

        /// <summary>
        /// Usuario que registro la venta.
        /// </summary>
        public string UsuarioRegistra { get; set; }

        /// <summary>
        /// Fecha en la que se registro la venta.
        /// </summary>
        public DateTime FechaRegistro { get; set; }
    }
}