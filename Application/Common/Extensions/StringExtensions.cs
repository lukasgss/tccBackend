using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Application.Common.Extensions;

public static class StringExtensions
{
    extension(string? accentedStr)
    {
        [return: NotNullIfNotNull(nameof(accentedStr))]
        public string? ToStrWithoutDiacritics()
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

        [return: NotNullIfNotNull(nameof(accentedStr))]
        public string? CapitalizeFirstLetter()
        {
            if (accentedStr is null)
            {
                return null;
            }

            return accentedStr.Length > 1 ? $"{char.ToUpper(accentedStr[0])}{accentedStr[1..]}" : accentedStr.ToUpper();
        }
    }
}