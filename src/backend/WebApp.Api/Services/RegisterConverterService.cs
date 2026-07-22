using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

public class RegisterConverterService : IUtilityService
{
    public string Endpoint => "case-converter";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        string[] lines = input.Split(
            '\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (lines.Length != 2)
            throw new ArgumentException("Ожидается две строки: режим и текст.");

        return ConvertRegister(lines);
    }

    private string ConvertRegister(string[] input)
    {
        return input[0] switch
        {
            "UPPER CASE" => input[1].ToUpperInvariant(),
            "lower case" => input[1].ToLowerInvariant(),
            "Title Case" => ConvertToTitleCase(input[1]),
            "camelCase" => ConvertToCamelCase(input[1]),
            "PascalCase" => ConvertToPascalCase(input[1]),
            "snake_case" => ConvertToSnakeCase(input[1]),
            "kebab-case" => ConvertToKebabCase(input[1]),
            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }

    private string ConvertToTitleCase(string input)
    {
        return string.Join(" ",
            GetWords(input).Select(Capitalize));
    }

    private string ConvertToCamelCase(string input)
    {
        var words = GetWords(input).ToList();

        if (!words.Any())
            return string.Empty;

        return words[0].ToLowerInvariant() +
               string.Concat(words.Skip(1).Select(Capitalize));
    }

    private string ConvertToPascalCase(string input)
    {
        return string.Concat(GetWords(input).Select(Capitalize));
    }

    private string ConvertToSnakeCase(string input)
    {
        return string.Join("_",
            GetWords(input).Select(w => w.ToLowerInvariant()));
    }

    private string ConvertToKebabCase(string input)
    {
        return string.Join("-",
            GetWords(input).Select(w => w.ToLowerInvariant()));
    }

    private static IEnumerable<string> GetWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Enumerable.Empty<string>();

        input = input.Replace('_', ' ')
                     .Replace('-', ' ');

        input = Regex.Replace(input, "([a-z0-9])([A-Z])", "$1 $2");

        return input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }

    private static string Capitalize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        return char.ToUpperInvariant(word[0]) +
               word[1..].ToLowerInvariant();
    }
}