using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Detalle de venta en formtato de lista de productos.
        /// </summary>
        public List<Producto> DetalleVentaList { get; set; }

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
        // TODO: Eliminarlo.
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

        /// <summary>
        /// Método que convierte una lista de productos a un string codificado.
        /// Contiene: IdProducto:CantidadProducto:TipoVenta:PrecioMayoreo:PrecioMenudeo,
        /// </summary>
        /// <param name="listaProductos">Lista de objetos tipo <c>Producto</c>.</param>
        /// <returns>Un <c>string</c> que contiene la lista codificada en un string</returns>
        public static string ListProductosToString(List<Producto> listaProductos) {
            var lista = "";
            for (var index = 0; index < listaProductos.Count; index++) {
                var producto = listaProductos[index];
                lista +=
                    $"{producto.IdProducto.ToString()}:{producto.CantidadDependencia.ToString()}:{producto.TipoVenta.ToString()}:{producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture)}:{producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture)},";
            }

            return lista.TrimEnd(',');
        }


    }
}