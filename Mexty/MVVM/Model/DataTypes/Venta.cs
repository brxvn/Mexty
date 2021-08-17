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
        public List<ItemInventario> DetalleVentaList { get; set; }

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
        /// <param name="mayoreo">si <c>true</c> guarda en venta mayoreo.</param>
        /// <returns>Un <c>string</c> que contiene la lista codificada en un string</returns>
        public static string ListProductosToString(List<ItemInventario> listaProductos, bool mayoreo = false) {
            var lista = "";
            for (var index = 0; index < listaProductos.Count; index++) {
                var producto = listaProductos[index];
                lista +=
                    $"{producto.IdProducto.ToString()}:{producto.CantidadDependencias.ToString()}:{producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture)}:{producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture)},";
            }

            return lista.TrimEnd(',');
        }

        /// <summary>
        /// Método que convierte un string codificado de una lista de productos y lo transforma a una lista de objetos producto.
        /// </summary>
        /// <param name="listaProductos"><c>string</c> codificado de productos.</param>
        /// <returns>Una lista de objetos tipo producto.</returns>
        public static List<ItemInventario> StringProductosToList(string listaProductos) {
            var items = listaProductos.Split(',');
            var productos = new List<ItemInventario>();

            for (var index = 0; index < items.Length; index++) {
                var item = items[index];
                var valores = item.Split(':');

                var producto = new ItemInventario() {
                    IdProducto = int.Parse(valores[0]),
                    CantidadDependencias = int.Parse(valores[1]),
                    //TipoVenta = int.Parse(valores[2]),
                    PrecioMayoreo = decimal.Parse(valores[3]),
                    PrecioMenudeo = decimal.Parse(valores[4])
                };
                productos.Add(producto);
            }

            return productos;
        }

    }
}