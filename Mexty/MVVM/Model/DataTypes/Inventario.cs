namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase principal de inventario.
    /// </summary>
    public class Inventario {

        /// <summary>
        /// Id del item del inventario.
        /// </summary>
        public int IdRegistro { get; set; }

        /// <summary>
        /// Id del producto.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Cantidad en existencia del producto.
        /// </summary>
        public int Cantidad { get; set; }

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
    }
}