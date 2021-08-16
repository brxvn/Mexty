namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase principal los items del inventario.
    /// </summary>
    public class ItemInventario {

        /// <summary>
        /// Id del item del inventario.
        /// </summary>
        public int IdRegistro { get; set; }

        /// <summary>
        /// Id del producto.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// ID de la tienda en donde esta este producto.
        /// </summary>
        public int IdTienda { get; set; }

        /// <summary>
        /// Tipo de producto.
        /// </summary>
        public string TipoProducto { get; set; }


        /// <summary>
        /// EL nombre del producto.
        /// </summary>
        public string NombreProducto { get; set; }

        /// <summary>
        /// Medida del producto.
        /// </summary>
        public string Medida { get; set; } 

        /// <summary>
        /// Cantidad en existencia del producto.
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Lista codificada de dependecias del producto.
        /// </summary>
        public string Dependencias { get; set; }

        /// <summary>
        /// Comentario del producto.
        /// </summary>
        public string Comentario { get; set; }

        /// <summary>
        /// Usuario que registro este item.
        /// </summary>
        public string UsuarioRegistra { get; set; }

        /// <summary>
        /// Fecha en la que se registro este item.
        /// </summary>
        public string FechaRegistro { get; set; }

        /// <summary>
        /// Usuario que hizo la ultima modificación a este item.
        /// </summary>
        public string UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de la última modificación de este item.
        /// </summary>
        public string FechaModifica { get; set; }
        
        /// <summary>
        /// Precio de cada producto (solo se usa en ventas)
        /// </summary>
        public decimal PrecioMayoreo { get; set; }

        /// <summary>
        /// Precio de cada producto (solo se usa en ventas)
        /// </summary>
        public decimal PrecioMenudeo { get; set; }
        /// <summary>
        /// Dependencias del prodcuto
        /// </summary>
        public int CantidadDependencias { get; set; }
        /// <summary>
        /// Precio de venta final (solo para usarse en ventas).
        /// </summary>
        public decimal PrecioVenta { get; set; }
    }
}