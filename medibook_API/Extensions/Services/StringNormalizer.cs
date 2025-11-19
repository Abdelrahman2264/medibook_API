using System.Globalization;

namespace medibook_API.Extensions.Services
{
    public class StringNormalizer
    {
        public  string NormalizeName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string cleaned = string.Join(" ", input.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned.ToLower());
        }
    }
}
