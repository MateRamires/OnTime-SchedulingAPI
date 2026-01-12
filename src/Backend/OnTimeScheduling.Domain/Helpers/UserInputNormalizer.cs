using System.Text.RegularExpressions;

namespace OnTimeScheduling.Domain.Helpers;

public static class UserInputNormalizer
{
    public static string? NormalizeEmailForStorage(string? email)
    {
        if (email is null) return null;

        email = email.Trim();

        return email.ToLowerInvariant();
    }

    public static string? NormalizeNameForStorage(string? name)
    {
        if (name is null) return null;

        name = name.Trim();

        name = Regex.Replace(name, @"\s+", " ");

        return name;
    }
}
