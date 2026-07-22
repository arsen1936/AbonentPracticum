using System.Text.Json;
using System.Net;
using System.Text.Encodings.Web;

namespace WebApp.Api.Services;

public class EscapingStringsService : IUtilityService
{
    public string Endpoint => "string-escape";


    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");


        var lines = input.Replace("\r", "")
                         .Split('\n', 2);


        if (lines.Length < 2)
            throw new ArgumentException("Ожидается режим и строка.");


        var mode = lines[0]
            .Trim()
            .ToLower();


        var text = lines[1];


        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Строка не может быть пустой.");


        var parts = mode.Split(':');


        if (parts.Length != 2)
            throw new ArgumentException("Некорректный формат режима.");


        var action = parts[0];
        var context = parts[1];


        return action switch
        {
            "escape" => Escape(text, context),
            "unescape" => Unescape(text, context),
            _ => throw new ArgumentException("Неизвестное действие.")
        };
    }


    private string Escape(string text, string context)
    {
        return context switch
        {
            "html" => WebUtility.HtmlEncode(text),

            "json" => JsonSerializer.Serialize(
                    text,
                    new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    })
                [1..^1],

            "sql" => text.Replace("'", "''"),

            "url" => Uri.EscapeDataString(text),

            _ => throw new ArgumentException("Неизвестный контекст.")
        };
    }


    private string Unescape(string text, string context)
    {
        return context switch
        {
            "html" => WebUtility.HtmlDecode(text),

            "json" => JsonSerializer.Deserialize<string>(
                $"\"{text}\""
            ) ?? string.Empty,

            "sql" => text.Replace("''", "'"),

            "url" => Uri.UnescapeDataString(text),

            _ => throw new ArgumentException("Неизвестный контекст.")
        };
    }
}