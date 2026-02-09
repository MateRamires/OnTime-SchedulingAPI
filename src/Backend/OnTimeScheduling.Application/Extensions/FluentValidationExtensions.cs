using FluentValidation;
using System.Text.RegularExpressions;

namespace OnTimeScheduling.Application.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidCNPJ<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(cnpj => ValidateCnpjAlgorithm(cnpj))
            .WithMessage("Invalid CNPJ format.");
    }

    public static IRuleBuilderOptions<T, string> IsValidPhone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(phone =>
        {
            if (string.IsNullOrEmpty(phone)) return false;

            string numbers = Regex.Replace(phone, "[^0-9]", "");

            return numbers.Length == 10 || numbers.Length == 11;
        }).WithMessage("Invalid Phone Number. Use format (XX) XXXXX-XXXX or (XX) XXXX-XXXX.");
    }

    private static bool ValidateCnpjAlgorithm(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return false;

        var numbers = Regex.Replace(cnpj, "[^0-9]", "");

        if (numbers.Length != 14) return false;

        if (new string(numbers[0], 14) == numbers) return false;

        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = numbers.Substring(0, 12);
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];

        int remainder = (sum % 11);
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        string digit = remainder.ToString();
        tempCnpj += digit;
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];

        remainder = (sum % 11);
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit += remainder.ToString();

        return numbers.EndsWith(digit);
    }
}
