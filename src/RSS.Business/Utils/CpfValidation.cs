using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSS.Business.Utils
{
    public class CpfValidation
    {
        public const int _CPF_LENGTH = 11;

        public static bool Validate(string cpf)
        {
            var cpfNumbers = OnlyNumbers(cpf);

            if (!ValidLength(cpfNumbers)) return false;
            return !HasRepetedDigits(cpfNumbers) && HasValidDigits(cpfNumbers);
        }

        private static bool ValidLength(string value)
        {
            return value.Length == _CPF_LENGTH;
        }

        private static bool HasRepetedDigits(string value)
        {
            string[] invalidNumbers =
            {
                "00000000000",
                "11111111111",
                "22222222222",
                "33333333333",
                "44444444444",
                "55555555555",
                "66666666666",
                "77777777777",
                "88888888888",
                "99999999999"
            };

            return invalidNumbers.Contains(value);
        }

        private static bool HasValidDigits(string value)
        {
            var number = value.Substring(0, _CPF_LENGTH - 2);
            var digitoVerificador = new DigitoVerificador(number)
               .ComMultiplicadoresDeAte(2, 11)
               .Substituindo("0", 10, 11);
            var firstDigit = digitoVerificador.CalculaDigito();
            digitoVerificador.AddDigito(firstDigit);
            var secondDigit = digitoVerificador.CalculaDigito();

            return string.Concat(firstDigit, secondDigit) == value.Substring(_CPF_LENGTH - 2, 2);

        }

        private static string OnlyNumbers(string value)
        {
            var onlyNumber = "";
            foreach (var s in value)
            {
                if (char.IsDigit(s))
                {
                    onlyNumber += s;
                }
            }
            return onlyNumber.Trim();
        }
    }
}
