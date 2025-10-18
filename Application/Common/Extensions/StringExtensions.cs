using System.Globalization;
using System.Text;

namespace Application.Common.Extensions;

public static class StringExtensions
{
    public static string? ToStrWithoutDiacritics(this string? accentedStr)
    {
        if (accentedStr is null)
        {
            return null;
        }

        string normalizedString = accentedStr.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new(capacity: normalizedString.Length);

        foreach (char character in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(character);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }

    public static string? CapitalizeFirstLetter(this string? str)
    {
        if (str is null)
        {
            return null;
        }

        return str.Length > 1 ? $"{char.ToUpper(str[0])}{str[1..]}" : str.ToUpper();
    }
}