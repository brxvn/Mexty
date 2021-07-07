using System;
using System.Data;
using System.Linq;
using Mexty.MVVM.Model.DataTypes;
using FluentValidation;

namespace Mexty.MVVM.Model.Validations {

    /// <summary>
    /// Clase Para validación de datos de el objeto usuario.
    /// </summary>
    public class UserValidation : AbstractValidator<Usuario> {
        public UserValidation() {
            RuleFor(usuario => usuario.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .Length(3, 50).WithMessage("El nombre debe de tener entre 30 y 50 carácteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(usuario => usuario.ApPaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El Apellido Paterno no puede estar vacío")
                .Length(3, 50).WithMessage("El Apellido Paterno debe de tener entre 3 y 50 carácteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido paterno válido.");
  
            RuleFor(usuario => usuario.ApMaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El Apellido Materno no puede estar vacío")
                .Length(3, 50).WithMessage("El Apellido Materno debe tener entre 3 y 50 carácteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido materno válido.");

            RuleFor(expression: usuario => usuario.Telefono)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de teléfono no puede esar vacío.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(Validations.BeAValidNumber).WithMessage("No es un número de teléfono válido.");

            RuleFor(usuario => usuario.Domicilio)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicilio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio debe de tener entre 3 y 50 carácteres.")
                .Must(Validations.BeAValidText).WithMessage("El domicio no es valido o tiene caracteres prohibidos.");

            RuleFor(usuario => usuario.Contraseña)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La contraseña no puede estar vacia.")
                .Length(4, 8).WithMessage("La contraseña debe tener entre 4 y 8 carácteres.")
                .Must(Validations.BeAValidText).WithMessage("La contraseña debe tiene carácteres prohibidos.");
        }

    }
}