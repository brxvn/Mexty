using System.Collections.Generic;
using System.Windows.Controls;

namespace Mexty.MVVM.Model.DataTypes {
    public class Sucursal {
        private string _nombreTienda;
        private string _dirección;

        /// <summary>
        /// Id De la tienda.
        /// </summary>
        public int IdTienda { get; set; }

        /// <summary>
        /// Nombre de la tienda.
        /// </summary>
        public string NombreTienda {
            get => _nombreTienda; 
            set => _nombreTienda = value.Trim();
        }

        /// <summary>
        /// Dirección de la tienda.
        /// </summary>
        public string Dirección {
            get => _dirección;
            set => _dirección = value.Trim();
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
        /// Sobrecarga de operadores para saber si una sucursal es igual a otra, en base al nombre y la dirección.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Sucursal a, Sucursal b) {
            if (a is null || b is null) return false;
            return a.NombreTienda == b.NombreTienda &&
                   a.Dirección == b.Dirección;
        }

        public static bool operator !=(Sucursal a, Sucursal b) {
            return !(a == b);
        }

    }
}