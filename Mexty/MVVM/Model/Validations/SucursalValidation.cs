using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class SucursalValidation : AbstractValidator<Sucursal> {
        public SucursalValidation() {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(sucursal => sucursal.NombreTienda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .Length(3, 15).WithMessage("El nombre debe tener entre 3 y 15 caracteres.")
                .Must(Validations.BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(sucursal => sucursal.Telefono) 
                .Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("El número de teléfono no puede esar vacio.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(Validations.BeAValidNumber).WithMessage("No es un número de teléfono válido.")
                .NotEqual("0000000000").WithMessage("No es un númro de teléfono válido");

            RuleFor(sucursal => sucursal.Dirección)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Domicilio Nulo.")
                //.NotEmpty().WithMessage("El domicio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio debe de tener entre 5 y 90 caracteres.")
                .Must(Validations.BeAValidText).WithMessage("El domicio no es válido o tiene caracteres prohibidos.");

            RuleFor(sucursal => sucursal.Rfc)
                .Cascade(CascadeMode.Stop)
                //.NotEmpty().WithMessage("El RFC no puede estar vacio.")
                .Length(12, 13).WithMessage("El RFC tiene que tener 12 o 13 carácteres")
                .Must(Validations.BeAValidRFC).WithMessage("No es un RFC válido");

            RuleFor(sucursal => sucursal.Facebook)
                .Cascade(CascadeMode.Stop)
                .Length(0, 100).WithMessage("El facebook debe de tener máximo 100 caracteres y tiene {TotalLength}.")
                .Must(Validations.BeAValidLink);
    
            RuleFor(sucursal => sucursal.Instagram)
                .Cascade(CascadeMode.Stop)
                .Length(0, 100).WithMessage("El facebook debe de tener máximo 100 caracteres y tiene {TotalLength}.");

            RuleFor(sucursal => sucursal.Mensaje)
                .Cascade(CascadeMode.Stop)
                .Length(0, 150).WithMessage("El Mensaje es demasiado largo, debe de tener máximo 100 carácteres.")
                .Must(Validations.BeAValidText).WithMessage("El mensaje tiene caracteres inválidos.");
        }
    }
}