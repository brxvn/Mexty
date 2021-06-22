namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase principal de productos.
    /// </summary>
    public class Producto {
        private string _nombreProducto;
        private string _medidaProducto;

        /// <summary>
        /// Id del producto.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string NombreProducto {
            get => _nombreProducto; 
            set => _nombreProducto = value.ToLower();
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

    }
}