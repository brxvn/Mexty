using FluentValidation;
using Mexty.MVVM.Model.DataTypes;

namespace Mexty.MVVM.Model.Validations {
    /// <summary>
    /// Clase para la validación de datos de un objeto Deuda.
    /// </summary>
    public class DeudaValidation : AbstractValidator<Deuda> {
        public DeudaValidation() {
            RuleFor(deuda => deuda.Debe.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La deuda no puede estar vacia, en caso de no tener deuda poner 0")
                .Length(1, 5).WithMessage("La deuda dada exede el largo permitido que es de 5 dijitos.")
                .Must(Validations.BeAValidFloat).WithMessage("No es un número valido, tiene caracteres prohibidos.");
        }
    }
}