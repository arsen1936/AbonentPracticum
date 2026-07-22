using System.Text.Json;

namespace WebApp.Api.Services;

public class JsonFormatterService : IUtilityService
{
    public string Endpoint => "json-formatter";


    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");


        var lines = input.Replace("\r", "")
            .Split('\n', 2);


        if (lines.Length < 2)
            throw new ArgumentException("Ожидается режим и JSON.");


        var mode = lines[0].Trim().ToLower();
        var json = lines[1].Trim();


        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON не может быть пустым.");


        return mode switch
        {
            "pretty" => Pretty(json),
            "minify" => Minify(json),
            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }


    private string Pretty(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);

            return JsonSerializer.Serialize(
                document,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }
    }


    private string Minify(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);

            return JsonSerializer.Serialize(
                document);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }
    }
}