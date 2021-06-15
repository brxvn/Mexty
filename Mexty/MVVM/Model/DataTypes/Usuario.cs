﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Mexty.MVVM.Model.DataTypes
{
    /// <summary>
    /// Clase Base para objetos tipo Usuario.
    /// </summary>
    // TODO: Agregar restricciónes para modificar estos parametros aqui.
    public class Usuario {
        private string _nombre;
        private string _apPaterno;
        private string _apMaterno;
        private string _domicilio;

        /// <summary>
        /// Id del empleado.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Nombre del empleado.
        /// </summary>
        public string Nombre {
            get => _nombre;
            set => _nombre = value.ToLower();
        }

        /// <summary>
        /// Apellido Paterno.
        /// </summary>
        public string ApPaterno {
            get => _apPaterno; 
            set => _apPaterno = value.ToLower();
        }

        /// <summary>
        /// Apellido Materno.
        /// </summary>
        public string ApMaterno {
            get => _apMaterno; 
            set => _apMaterno = value.ToLower();
        }

        /// <summary>
        /// Nick del empleado (username).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contraseña del emplado.
        /// </summary>
        public string Contraseña { get; set; }

        /// <summary>
        /// Domicilio del empleado.
        /// </summary>
        public string Domicilio {
            get => _domicilio; 
            set => _domicilio = value.ToLower();
        }

        /// <summary>
        /// Teléfono del empleado.
        /// </summary>
        public int Telefono { get; set; }

        /// <summary>
        /// Indica si el usuario esta activo o no.
        /// </summary>
        public int Activo { get; set; }

        /// <summary>
        /// Id de la tienda asignada al empleado.
        /// </summary>
        public int IdTienda { get; set; }

        /// <summary>
        /// Id del rol asignado al empleado.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del Empleado que registro a este empleado.
        /// </summary>
        public string UsuraioRegistra { get; set; }

        /// <summary>
        /// Fecha de registro de este empleado.
        /// </summary>
        public string FechaRegistro { get; set; }

        /// <summary>
        /// Nombre del empleado que modifico por última vez a este empleado.
        /// </summary>
        public string UsuarioModifica { get; set; }

        /// <summary>
        /// Fecha de la última modificación a este empleado.
        /// </summary>
        public string FechaModifica { get; set; }
        
    }
}