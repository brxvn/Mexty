namespace Mexty.MVVM.Model.DataTypes {
    public class Rol {
        private string _rolDescripcion;
        /// <summary>
        /// Id del rol.
        /// </summary>
        public int IdRol { get; init; }

        /// <summary>
        /// Descripción del rol.
        /// </summary>
        public string RolDescription {
            get => _rolDescripcion;
            set => _rolDescripcion = value.ToUpper();
        }

        /// <summary>
        /// Id de la tienda.
        /// </summary>
        public int IdTienda { get; set; }
    }
}