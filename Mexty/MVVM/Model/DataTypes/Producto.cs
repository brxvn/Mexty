using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase principal de productos.
    /// </summary>
    public class Producto {
        private string _nombreProducto;
        private string _medidaProducto;

        /// <summary>
        /// Variable estatica que tiene los tipos de venta, sin id.
        /// </summary>
        public static readonly string[] TiposVentaTexto = { "General", "Mayoreo", "Menudeo" };

        /// <summary>
        /// Variable que obtiene los tipos de producto.
        /// </summary>
        // TODO: probablemente leerlos del ini.
        private static readonly string[] _tipoProductoText = { "Paleta Agua", "Paleta Leche", "Paleta Fruta", "Helado", "Agua", "Extras", "Otros" };

        /// <summary>
        /// Variable que obtiene los tipos de medida.
        /// </summary>
        // TODO: probablemente leerlos del ini.
        private static readonly string[] TiposMedida = { "pieza", "bolsa", "caja", "tarro", "litro", };


        /// <summary>
        /// Método estatico para obtener Los tipos de producto que hay.
        /// </summary>
        /// <returns></returns>
        public static string[] GetTiposProducto() {
            return _tipoProductoText;
        }

        /// <summary>
        /// Método estatico para obtener los tipos de medidas que hay.
        /// </summary>
        /// <returns></returns>
        public static string[] GetTiposMedida(int cant = 0, int salto = 0) {
            if (cant > 0) {
                var nuevos = TiposMedida.Skip(salto).Take(cant).ToArray();
                return nuevos;
            }
            return TiposMedida;
        }

        /// <summary>
        /// Id del producto.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string NombreProducto {
            get => _nombreProducto;
            set => _nombreProducto = value.ToLower().Trim();
        }

        /// <summary>
        /// Nombre de la medida del producto.
        /// </summary>
        public string MedidaProducto {
            get => _medidaProducto;
            set => _medidaProducto = value.ToLower();
        }

        /// <summary>
        /// Cantidad de piezas del producto.
        /// </summary>
        public int Piezas { get; set; }

        /// <summary>
        /// Tipo de producto.
        /// </summary>
        public string TipoProducto { get; set; }

        /// <summary>
        /// Clave del tipo de venta.
        /// </summary>
        public int TipoVenta { get; set; }

        /// <summary>
        /// Obtiene el tipo de venta en formato leible de la instancia.
        /// </summary>
        public string TipoVentaNombre => TiposVentaTexto[TipoVenta];

        /// <summary>
        /// Precio del producto en venta mayoreo.
        /// </summary>
        public decimal PrecioMayoreo { get; set; }

        /// <summary>
        /// Precio del producto en venta menudeo.
        /// </summary>
        public decimal PrecioMenudeo { get; set; }

        /// <summary>
        /// Detalles/descripción del producto.
        /// </summary>
        public string DetallesProducto { get; set; }

        /// <summary>
        /// Indica si el producto esta activo o no.
        /// </summary>
        public int Activo { get; set; }

        /// <summary>
        /// Canitdad de cada producto.
        /// </summary>
        public int CantidadDependencia { get; set; }

        /// <summary>
        /// Precio de venta final (solo para usarse en ventas).
        /// </summary>
        public decimal PrecioVenta { get; set; }

        /// <summary>
        /// La lista de dependencias codificada que se guarda en la base de datos.
        /// </summary>
        public string DependenciasText { get; set; }

        /// <summary>
        /// Lista que contiene todas las dependencias
        /// </summary>
        public List<Producto> Dependencias { get; set; }

        /// <summary>
        /// Método que convierte una lista de dependencias a string para ser guardada en la base de datos.
        /// </summary>
        /// <param name="listaDepend"> Una lista de objetos tipo <c>Producto</c>.</param>
        /// <returns>Un diccionario tipo string codificado donde : separa al id y la cantidad y , separa los elementos.</returns>
        public static string DependenciasToString(List<Producto> listaDepend) {
            var cadena = "";
            for (var index = 0; index < listaDepend.Count; index++) {
                var producto = listaDepend[index];
                cadena += $"{producto.IdProducto.ToString()}:{producto.CantidadDependencia.ToString()},";
            }

            return cadena.TrimEnd(',');
        }

        /// <summary>
        /// Método que recibe un string codificaddo con la información de las dependencias
        /// </summary>
        /// <param name="depend"><c>string</c> codificado con <c>DepenendciasToString</c>.</param>
        /// <returns>Lista de Objetos tipo Producto con solo los campos IdProducto y CantidadDependencia llenos.</returns>
        public static List<Producto> DependenciasToList(string depend) {
            var items = depend.Split(',');
            var dependencias = new List<Producto>();

            for (var index = 0; index < items.Length; index++) {
                var dependencia = items[index];
                var valores = dependencia.Split(':');

                var producto = new Producto {
                    IdProducto = int.Parse(valores[0]),
                    CantidadDependencia = int.Parse(valores[1])
                };
                dependencias.Add(producto);
            }

            return dependencias;
        }
    }
}