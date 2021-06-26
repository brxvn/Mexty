using System;
using System.Data;
using System.Linq;
using Mexty.MVVM.Model.DataTypes;
using FluentValidation;

namespace Mexty.MVVM.Model.Validations {

    /// <summary>
    /// Clase Para validación de datos de el objeto usuario.
    /// </summary>
    public class UserValidation : AbstractValidator<Usuario> {
        public UserValidation() {
            RuleFor(usuario => usuario.Nombre)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre no puede estar vacio.")
                .Length(3, 50).WithMessage("El nombre es muy largo o muy corto.")
                .Must(BeAValidName).WithMessage("No es un nombre válido.");

            RuleFor(usuario => usuario.ApPaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido Paterno no puede estar vacio")
                .Length(3, 50).WithMessage("El apellido paterno es muy largo o muy corto.")
                .Must(BeAValidName).WithMessage("No es un apellido paterno válido.");
  
            RuleFor(usuario => usuario.ApMaterno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El apellido paterno no puede estar vacio")
                .Length(3, 50).WithMessage("El apellido paterno es muy largo o muy corto.")
                .Must(BeAValidName).WithMessage("No es un apellido paterno válido.");

            RuleFor(expression: usuario => usuario.Telefono.ToString())
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El número de teléfono no puede esar vacio.")
                .Length(10).WithMessage("El número de teléfono debe de ser de 10 dígitos.")
                .Must(BeAValidNumber).WithMessage("No es un número de teléfono valido.");

            RuleFor(usuario => usuario.Domicilio)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicio no puede estar vacio.")
                .Length(5, 90).WithMessage("El domicilio es muy corto o largo par ser válido.")
                .Must(BeAValidText).WithMessage("El domicio no es valido o tiene caracteres prohibidos.");

            RuleFor(usuario => usuario.Contraseña)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El domicilio no puede estar vacio.")
                .Length(4, 8).WithMessage("La contraseña tiene {TotalLength} y debe tener entre 4 y 8 caracteres.")
                .Must(BeAValidText).WithMessage("La contraseña debe tiene caracteres prohibidos.");
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