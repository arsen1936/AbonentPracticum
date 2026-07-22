namespace WebApp.Api.Services;

using System.Globalization;

public class SymbolInspectorService : IUtilityService
{
    public string Endpoint => "char-inspector";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        var lines = input.Replace("\r", "").Split('\n');

        if (lines.Length != 2)
            throw new ArgumentException("Ожидается две строки: режим и значение.");

        return GenerateCodeResult(lines);
    }

    private string GenerateCodeResult(string[] input)
    {
        return input[0] switch
        {
            "encode" => StringToCharCode(input[1]),
            "decode" => GetStringByCharCode(input[1]),
            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }

    private string StringToCharCode(string input)
    {
        return string.Join(" ",
            input.Select(c => $"U+{((int)c):X4}"));
    }

    private string GetStringByCharCode(string input)
    {
        var codes = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var chars = new List<char>();

        foreach (var code in codes)
        {
            if (!int.TryParse(
                    code,
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out int value))
            {
                throw new ArgumentException($"Некорректный код: {code}");
            }

            chars.Add((char)value);
        }

        return new string(chars.ToArray());
    }
}