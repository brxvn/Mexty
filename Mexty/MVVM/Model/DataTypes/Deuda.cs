namespace Mexty.MVVM.Model.DataTypes {
    /// <summary>
    /// Clase para la tabla de Deuda Mayoreo
    /// </summary>
    public class Deuda {

        /// <summary>
        /// ID de la deuda.
        /// </summary>
        public int IdDeuda { get; set; }

        /// <summary>
        /// Id del cliente dueño de la deuda.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Total de la deuda del cliente.
        /// </summary>
        public double Debe { get; set; }

        /// <summary>
        /// Usuario que registro por primera vez la deuda.
        /// </summary>
        public string UsuarioRegistra { get; set; } 

        /// <summary>
        /// Fecha de registro de la deuda por primera vez.
        /// </summary>
        public string FechaRegistra { get; set; }

        /// <summary>
        /// El último usuario en modificar la deuda.
        /// </summary>
        public string UsuarioModifica { get; set; }

        /// <summary>
        /// La fecha de la última modificación de la deuda.
        /// </summary>
        public string FechaModifca { get; set; }
    }
}