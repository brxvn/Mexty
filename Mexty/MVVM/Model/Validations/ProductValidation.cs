using System;
using System.Data;
using System.Linq;
using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class ProductValidation : AbstractValidator<Producto> {
        public ProductValidation() {
            RuleFor(producto => producto.NombreProducto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El Nombre del producto no puede estar vacio.")
                .Length(4, 15).WithMessage("El Nombre del producto debe de tener entre 4 y 15 caracteres")
                .Must(BeAValidName).WithMessage("El Nombre del producto no puede tener caracteres prohibidos");

            RuleFor(producto => producto.PrecioMenudeo.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(
                    "El {ProertyName} No puede estar vacio, puede tener un 0 en el caso de que no exista")
                .Length(1, 10)
                .WithMessage("El {PropertyName} tiene {TotalLength} y debe de tener entre 1 y 10 caracteres")
                .Must(BeAValidNumber).WithMessage("El {PropertyName} Debe de contener solo números.");

            RuleFor(producto => producto.PrecioMayoreo.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(
                    "El {ProertyName} No puede estar vacio, puede tener un 0 en el caso de que no exista")
                .Length(1, 10)
                .WithMessage("El {PropertyName} tiene {TotalLength} y debe de tener entre 1 y 10 caracteres")
                .Must(BeAValidNumber).WithMessage("El {PropertyName} Debe de contener solo números.");

            RuleFor(producto => producto.DetallesProducto)
                .Cascade(CascadeMode.Stop)
                .Length(0, 50)
                .WithMessage("La descripción del produto tiene {TotalLenght} y debe de tener entre 0 y 50 caracteres.")
                .Must(BeAValidText).WithMessage("La descripción del producto tiene caracteres prohibidos.");
        }

        /// <summary>
        /// Método que revisa si es un nombre valido.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><c>true</c> si es un nombre valido, si no <c>false</c> </returns>
        protected bool BeAValidName(string name) {
            name = name.Replace(" ", "");
            name = name.Replace("-", "");
            return name.All(Char.IsLetter);
        }

        /// <summary>
        /// Método que revisa si un número es valido.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        protected bool BeAValidNumber(string number) {
            number = number.Replace(" ", "");
            number = number.Replace("-", "");
            return number.All(Char.IsNumber);
        }

        /// <summary>
        /// Método que revisa si un texto es valido.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected bool BeAValidText(string text) {
            text = text.Replace(" ", "");
            text = text.Replace("-", "");
            text = text.Replace("/", "");
            text = text.Replace("#", "");
            return text.All(Char.IsLetterOrDigit);
        }
    }
}