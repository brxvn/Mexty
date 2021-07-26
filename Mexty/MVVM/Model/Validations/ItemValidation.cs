using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class ItemValidation : AbstractValidator<ItemInventario> {
        public ItemValidation() {
            CascadeMode = CascadeMode.Stop;

            RuleFor(item => item.Cantidad.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La cantidad no puede estar vacia, en todo caso poner 0.")
                .Length(0, 10).WithMessage("La cantidad es demasiado grande para ser valida.")
                .Must(Validations.BeAValidNumber).WithMessage("No es una cantidad valida");

            RuleFor(item => item.Piezas.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Las piezas no pueden estar vacias, en todo caso poner 0.")
                .Length(0, 10).WithMessage("La cantidad de piezas es demasiado grande para ser valida.")
                .Must(Validations.BeAValidNumber).WithMessage("No es una cantidad valida");

            RuleFor(item => item.Comentario)
                .Cascade(CascadeMode.Stop)
                .Length(0, 100).WithMessage("El comentario es demasiado largo tiene {TotalLength} caracteres y debe tener no más de 100.")
                .Must(Validations.BeAValidText).WithMessage("El comentario tiene caracteres invalidos.");
        }
    }
}