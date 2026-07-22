using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class LoremIpsumGeneratorService : IUtilityService
{
    public string Endpoint => "lorem-ipsum";

    private const string LoremText =
        "Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua Ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur Excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum";

    private static readonly string[] Words = LoremText.Split(' ');

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        LoremIpsumGeneratorRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<LoremIpsumGeneratorRequest>(input);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }

        if (request == null)
            throw new ArgumentException("Некорректный JSON.");

        if (string.IsNullOrWhiteSpace(request.Type))
            throw new ArgumentException("Не указан тип генерации.");

        if (!int.TryParse(request.Count, out int count))
            throw new ArgumentException("Количество должно быть числом.");

        if (count <= 0)
            throw new ArgumentException("Количество должно быть больше нуля.");

        return GenerateLoremIpsum(request.Type, request.Count);
    }

    private string GenerateLoremIpsum(string type, string count)
    {
        return type switch
        {
            "paragraphs" => GenerateParagraphs(count),
            "words" => GenerateWords(count),
            "chars" => GenerateChars(count),
            _ => throw new ArgumentException("Неизвестный тип генерации.")
        };
    }

    private string GenerateParagraphs(string count)
    {
        int paragraphCount = int.Parse(count);

        return string.Join(
            Environment.NewLine + Environment.NewLine,
            Enumerable.Repeat(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                paragraphCount));
    }

    private string GenerateWords(string count)
    {
        int wordCount = int.Parse(count);

        return string.Join(" ",
            Enumerable.Range(0, wordCount)
                .Select(i => Words[i % Words.Length]));
    }

    private string GenerateChars(string count)
    {
        int charCount = int.Parse(count);

        var builder = new StringBuilder();

        while (builder.Length < charCount)
        {
            builder.Append(LoremText);
            builder.Append(' ');
        }

        return builder.ToString(0, charCount);
    }
}

public class LoremIpsumGeneratorRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("count")]
    public string Count { get; set; } = string.Empty;
}