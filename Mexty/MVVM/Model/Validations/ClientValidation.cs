using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    /// <summary>
    /// Clase para la validación de datos de le objeto usuario.
    /// </summary>
    public class ClientValidation : AbstractValidator<Cliente> {
        public ClientValidation() {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(cliente => cliente.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre no puede estar vacio.")
                .Length(3, 50).WithMessage("El nombre debe de tener entre 3 y 50 caracteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(cliente => cliente.ApPaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido Paterno no puede estar vacío")
                .Length(3, 50).WithMessage("El apellido paterno debe de tener entre 3 y 50 caracteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido paterno válido.");

            RuleFor(cliente => cliente.ApMaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido materno no puede estar vacío")
                .Length(3, 50).WithMessage("El apellido materno debe de tener entre 3 y 50 caracteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un apellido materno válido.");

            RuleFor(expression: cliente => cliente.Telefono)
                .Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("El número de teléfono no puede esar vacio.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(Validations.BeAValidNumber).WithMessage("No es un número de teléfono válido.")
                .NotEqual("0000000000").WithMessage("No es un número de teléfono válido");

            RuleFor(cliente => cliente.Domicilio)
                .Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("El domicio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio debe de tener entre 5 y 90 caracteres.")
                .Must(Validations.BeAValidText).WithMessage("El domicilio no es válido o tiene caracteres prohibidos.");
            
            RuleFor(cliente => cliente.Debe.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La deuda no puede estar vacía, en caso de no tener deuda poner 0")
                .Length(1, 10).WithMessage("La deuda dada excede el largo permitido que es de 10 dígitos. {TotalLength}")
                .Must(Validations.BeAValidFloat).WithMessage("No es un número válido, tiene caracteres prohibidos.");
        }
    }
}