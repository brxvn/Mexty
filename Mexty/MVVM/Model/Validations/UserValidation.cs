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
                .NotEmpty().WithMessage("El nombre no puede estar vacio.")
                .Length(3, 50).WithMessage("El nombre tiene {TotalLength} caracteres y debe de tener entre 3 y 50.")
                .Must(Validations.BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(usuario => usuario.ApPaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido Paterno no puede estar vacio")
                .Length(3, 50).WithMessage("El apellido paterno es muy largo o muy corto.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido paterno válido.");
  
            RuleFor(usuario => usuario.ApMaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido paterno no puede estar vacio")
                .Length(3, 50).WithMessage("El apellido paterno es muy largo o muy corto.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido paterno válido.");

            RuleFor(expression: usuario => usuario.Telefono)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de teléfono no puede esar vacio.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(Validations.BeAValidNumber).WithMessage("No es un número de teléfono valido.");

            RuleFor(usuario => usuario.Domicilio)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio es muy corto o largo par ser válido.")
                .Must(Validations.BeAValidText).WithMessage("El domicio no es valido o tiene caracteres prohibidos.");

            RuleFor(usuario => usuario.Contraseña)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicilio no puede estar vacio.")
                .Length(4, 8).WithMessage("La contraseña tiene {TotalLength} y debe tener entre 4 y 8 caracteres.")
                .Must(Validations.BeAValidText).WithMessage("La contraseña debe tiene caracteres prohibidos.");
        }

    }
}