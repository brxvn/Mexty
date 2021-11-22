using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    public class ItemValidation : AbstractValidator<ItemInventario> {
        public ItemValidation() {
            CascadeMode = CascadeMode.Stop;

            RuleFor(item => item.Cantidad)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La cantidad no puede estar vacia, en todo caso poner 0.")
                .GreaterThan(0).WithMessage("La cantidad no puede ser menor a 0.")
                .LessThan(99999999).WithMessage("La cantidad es demasiado grande para ser valida");
                //.Must(Validations.BeAValidNumber).WithMessage("No es una cantidad valida");

            RuleFor(item => item.Comentario)
                .Cascade(CascadeMode.Stop)
                .Length(0, 100).WithMessage("El comentario es demasiado largo tiene {TotalLength} caracteres y debe tener no más de 100.")
                .Must(Validations.BeAValidText).WithMessage("El comentario tiene caracteres invalidos.");
        }
    }
}