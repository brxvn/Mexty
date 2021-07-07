using System;
using System.Data;
using System.Linq;
using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class ProductValidation : AbstractValidator<Producto> {
        public ProductValidation() {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(producto => producto.NombreProducto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El Nombre del producto no puede estar vacio.")
                .Length(4, 15).WithMessage("El Nombre del producto debe de tener entre 4 y 15 caracteres")
                .Must(Validations.BeAValidName).WithMessage("El Nombre del producto no puede tener caracteres prohibidos");

            RuleFor(producto => producto.PrecioMenudeo.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(
                    "El {ProertyName} No puede estar vacio, puede tener un 0 en el caso de que no exista")
                .Length(1, 10)
                .WithMessage("El {PropertyName} tiene {TotalLength} y debe de tener entre 1 y 10 caracteres")
                .Must(Validations.BeAValidNumber).WithMessage("El {PropertyName} Debe de contener solo números.");

            RuleFor(producto => producto.PrecioMayoreo.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(
                    "El {ProertyName} No puede estar vacio, puede tener un 0 en el caso de que no exista")
                .Length(1, 10)
                .WithMessage("El {PropertyName} tiene {TotalLength} y debe de tener entre 1 y 10 caracteres")
                .Must(Validations.BeAValidNumber).WithMessage("El {PropertyName} Debe de contener solo números.");

            RuleFor(producto => producto.DetallesProducto)
                .Cascade(CascadeMode.Stop)
                .Length(0, 50)
                .WithMessage("La descripción del produto tiene {TotalLenght} y debe de tener entre 0 y 50 caracteres.")
                .Must(Validations.BeAValidText).WithMessage("La descripción del producto tiene caracteres prohibidos.");
        }

    }
}