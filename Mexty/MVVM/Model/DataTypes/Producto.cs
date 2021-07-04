using System.Collections.Generic;
using System.Linq;

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
        public static readonly string[] TiposVentaTexto = {"Mayoreo y Menudeo", "Mayoreo", "Menudeo"};

        /// <summary>
        /// Variable que obtiene los tipos de producto.
        /// </summary>
        // TODO: probablemente leerlos del ini.
        private static readonly string[] _tipoProductoText = {"Paleta Agua", "Paleta Leche", "Paleta Fruta", "Agua", "Helado", "Otros", "Extras"};

        /// <summary>
        /// Variable que obtiene los tipos de medida.
        /// </summary>
        // TODO: probablemente leerlos del ini.
        private static readonly string[] TiposMedida = {"pieza", "gramos", "litro", "plato"};

        
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
        public static string[] GetTiposMedida() {
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
        /// Tipo de producto.
        /// </summary>
        public string TipoProducto { get; set; } //TODO: Probablemente leerlos del ini.

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
        public int PrecioMayoreo { get; set; }

        /// <summary>
        /// Precio del producto en venta menudeo.
        /// </summary>
        public int PrecioMenudeo { get; set; }

        /// <summary>
        /// Detalles/descripción del producto.
        /// </summary>
        public string DetallesProducto { get; set; }

        /// <summary>
        /// Indica si el producto esta activo o no.
        /// </summary>
        public int Activo { get; set; }

        /// <summary>
        /// Sucursal en la que se va a vender el producto.
        /// </summary>
        public int IdSucursal { get; set; }

        /// <summary>
        /// Buffer de lista de sucursales.
        /// </summary>
        private static List<Sucursal> ListaSucursal { get; set; }
        
        /// <summary>
        /// Obtiene la sucursal por medio del nombre.
        /// </summary>
        public string GetSucursalNombre {
            get {
                ListaSucursal ??= Database.GetTablesFromSucursales();
                var nombre = "";
                for (var index = 0; index < ListaSucursal.Count; index++) {
                    var sucursal = ListaSucursal[index];
                    if (IdSucursal == sucursal.IdTienda) {
                        nombre = sucursal.NombreTienda;
                    }
                }

                return nombre;
            }
        }

    }
}