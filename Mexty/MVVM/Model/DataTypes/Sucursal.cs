using System.Collections.Generic;
using System.Windows.Controls;

namespace Mexty.MVVM.Model.DataTypes {
    public class Sucursal {
        private string _nombreTienda;
        private string _dirección;
        private int _telefono;

        /// <summary>
        /// Id De la tienda.
        /// </summary>
        public int IdTienda { get; set; }

        /// <summary>
        /// Nombre de la tienda.
        /// </summary>
        public string NombreTienda {
            get => _nombreTienda; 
            set => _nombreTienda = value.ToLower().Trim();
        }

        /// <summary>
        /// Dirección de la tienda.
        /// </summary>
        public string Dirección {
            get => _dirección;
            set => _dirección = value.ToLower();
        }

        /// <summary>
        /// Teléfono de la tienda.
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// RFC de la tienda.
        /// </summary>
        public string Rfc { get; set; }

        /// <summary>
        /// Logo de la tienda.
        /// </summary>
        // TODO: Ver que onda con el objeto blob que viene de la base de datos.
        public List<byte> Logo { get; set; }

        /// <summary>
        /// Mensaje de la tienda.
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Facebook de la tienda.
        /// </summary>
        public string Facebook { get; set; }

        /// <summary>
        /// Instagram de la tienda.
        /// </summary>
        public string Instagram { get; set; }

        /// <summary>
        /// Tipo de tienda.
        /// </summary>
        public string TipoTienda { get; set; }

        /// <summary>
        /// Indica si la sucursal esta activa o no.
        /// </summary>
        public int Activo { get; set; }
    }
}