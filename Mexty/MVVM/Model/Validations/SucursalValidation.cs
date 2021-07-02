using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class SucursalValidation : AbstractValidator<Sucursal> {
        public SucursalValidation() {
            RuleFor(sucursal => sucursal.NombreTienda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre no puede estar vacio.")
                .Length(3, 15).WithMessage("El nombre tiene {TotalLength} caracteres y debe tener entre 3 y 15.")
                .Must(Validations.BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(sucursal => sucursal.Telefono.ToString()) // TODO convertir este numero a varchar
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de teléfono no puede esar vacio.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(Validations.BeAValidNumber).WithMessage("No es un número de teléfono valido.");

            RuleFor(sucursal => sucursal.Dirección)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio es muy corto o largo par ser válido.")
                .Must(Validations.BeAValidText).WithMessage("El domicio no es valido o tiene caracteres prohibidos.");

            RuleFor(sucursal => sucursal.Rfc)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El RFC no puede estar vacio.")
                .Length(13).WithMessage("El RFC tiene que tener 13 caracteres")
                .Must(Validations.BeAValidText).WithMessage("");// TODO: hacer validacion de RFC.

            RuleFor(sucursal => sucursal.Facebook)
                .Cascade(CascadeMode.Stop)
                // TODO: ver si puede estar vacio.
                .Length(0, 100).WithMessage("El facebook debe de tener maximo 100 caracteres y tiene {TotalLength}.")
                .Must(Validations.BeAValidName); //TODO preguntar si va a ser el nombre o la liga o k.
    
            RuleFor(sucursal => sucursal.Instagram)
                .Cascade(CascadeMode.Stop)
                // TODO: ver si puede estar vacio.
                .Length(0, 100).WithMessage("El facebook debe de tener maximo 100 caracteres y tiene {TotalLength}.")
                .Must(Validations.BeAValidName); //TODO preguntar si va a ser el nombre o la liga o k.

            RuleFor(sucursal => sucursal.Mensaje)
                .Cascade(CascadeMode.Stop)
                // TODO ver si puede estar vacio.
                .Length(0, 150)
                .WithMessage("El Mensaje es demaciado largo, debe de tener máximo 100 caracteres y tiene {TotalLength}")
                .Must(Validations.BeAValidText).WithMessage("El mensaje tiene caracteres invalidos.");
        }
    }
}