using System.Text;

namespace WebApp.Api.Services;

public class Base64ConverterService : IUtilityService
{
    public string Endpoint => "base64";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        string[] lines = input.Split(
            '\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (lines.Length != 2)
            throw new ArgumentException("Ожидается две строки: режим и значение.");

        return Convert(lines);
    }

    private string Convert(string[] input)
    {
        return input[0] switch
        {
            "encode" => EncodeToBase64(input[1]),
            "decode" => DecodeFromBase64(input[1]),
            _ => throw new ArgumentException(
                "Неизвестный режим. Используйте 'encode' или 'decode'.")
        };
    }

    private string EncodeToBase64(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        return System.Convert.ToBase64String(bytes);
    }

    private string DecodeFromBase64(string input)
    {
        try
        {
            byte[] bytes = System.Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Некорректная строка Base64.");
        }
    }
}