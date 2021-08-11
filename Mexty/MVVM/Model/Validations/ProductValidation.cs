using System;
using System.Data;
using System.Globalization;
using System.Linq;
using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class ProductValidation : AbstractValidator<Producto> {
        public ProductValidation() {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(producto => producto.NombreProducto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del producto no puede estar vacío.")
                .Length(4, 30).WithMessage("El nombre del producto debe de tener entre 4 y 15 caracteres")
                .Must(Validations.BeAValidName).WithMessage("El nombre del producto no puede tener caracteres prohibidos");

            RuleFor(producto => producto.PrecioMenudeo.ToString(CultureInfo.InvariantCulture))
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(
                    "El {ProertyName} No puede estar vacío, puede tener un 0 en el caso de que no exista")
                .Length(1, 10)
                .WithMessage("El {PropertyName} debe de tener entre 1 y 10 caracteres")
                .Must(Validations.BeAValidFloat).WithMessage("El {PropertyName} debe de contener sólo números.");

            RuleFor(producto => producto.PrecioMayoreo.ToString(CultureInfo.InvariantCulture))
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(
                    "El {ProertyName} no puede estar vacío, puede tener un 0 en el caso de que no exista")
                .Length(1, 10)
                .WithMessage("El {PropertyName} debe de tener entre 1 y 10 caracteres")
                .Must(Validations.BeAValidFloat).WithMessage("El {PropertyName} debe de contener sólo números.");

            RuleFor(producto => producto.DetallesProducto)
                .Cascade(CascadeMode.Stop)
                .Length(0, 50)
                .WithMessage("La descripción del produto tiene {TotalLength} y debe de tener entre 0 y 50 caracteres.")
                .Must(Validations.BeAValidText).WithMessage("La descripción del producto tiene caracteres prohibidos.");

            RuleFor(producto => producto.Piezas.ToString())
                .Cascade(CascadeMode.Stop)
                .Length(0, 6)
                .WithMessage("La cantidad de piezas del produto tiene {TotalLength} y debe de tener entre 0 y 6 caracteres.")
                .Must(Validations.BeAValidNumber).WithMessage("La cantidad del producto tiene caracteres prohibidos.");
        }
    }
}