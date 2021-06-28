﻿using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    /// <summary>
    /// Clase para la validación de datos de le objeto usuario.
    /// </summary>
    public class ClientValidation : AbstractValidator<Cliente> {
        public ClientValidation() {
            RuleFor(cliente => cliente.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre no puede estar vacio.")
                .Length(3, 50).WithMessage("El nombre es muy largo o muy corto.")
                .Must(Validations.BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(cliente => cliente.ApPaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido Paterno no puede estar vacio")
                .Length(3, 50).WithMessage("El apellido paterno es muy largo o muy corto.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido paterno válido.");

            RuleFor(cliente => cliente.ApMaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido paterno no puede estar vacio")
                .Length(3, 50).WithMessage("El apellido paterno es muy largo o muy corto.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido paterno válido.");

            RuleFor(expression: cliente => cliente.Telefono.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de teléfono no puede esar vacio.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(Validations.BeAValidNumber).WithMessage("No es un número de teléfono valido.");

            RuleFor(cliente => cliente.Domicilio)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio es muy corto o largo par ser válido.")
                .Must(Validations.BeAValidText).WithMessage("El domicio no es valido o tiene caracteres prohibidos.");
        }
        
        
    }
}