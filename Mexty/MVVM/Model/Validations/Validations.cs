using System;
using System.Linq;

namespace Mexty.MVVM.Model.Validations {
    public static class Validations {
        /// <summary>
        /// Método que revisa si es un nombre valido.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><c>true</c> si es un nombre valido, si no <c>false</c> </returns>
        public static bool BeAValidName(string name) {
            name = name.Replace(" ", "");
            name = name.Replace("-", "");
            return name.All(Char.IsLetter);
        }

        /// <summary>
        /// Método que revisa si un número teléfonico es valido.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool BeAValidNumber(string number) {
            number = number.Replace(" ", "");
            number = number.Replace("-", "");
            return number.All(Char.IsNumber);
        }

        /// <summary>
        /// Método que revisa si un una cantidad decimal es valida.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool BeAValidFloat(string number) {
            number = number.Replace(".", "");
            return number.All(Char.IsNumber);
        }

        /// <summary>
        /// Método que revisa si un texto es valido.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool BeAValidText(string text) {
            text = text.Replace(" ", "");
            text = text.Replace("-", "");
            text = text.Replace("/", "");
            text = text.Replace("#", "");
            return text.All(Char.IsLetterOrDigit);
        }
    }
}