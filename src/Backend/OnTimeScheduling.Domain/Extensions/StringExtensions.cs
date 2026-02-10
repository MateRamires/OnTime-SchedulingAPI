using System.Globalization;
using System.Text.RegularExpressions;

namespace OnTimeScheduling.Domain.Extensions;

public static class StringExtensions
{
    public static string SanitizeEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;

        return email.Trim().ToLower();
    }

    public static string FormatName(this string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;

        var cleanName = string.Join(" ", name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanName.ToLower());
    }

    public static string RemoveNonNumeric(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return Regex.Replace(value, "[^0-9]", "");
    }
}
