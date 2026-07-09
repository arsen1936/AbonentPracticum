using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

public class RegularExpressionBuilderService : IUtilityService
{
    public string Endpoint => "regex-tester";
    public string Execute(string input)
    {
        RegularExpression? request;
        try
        {
            request = JsonSerializer.Deserialize<RegularExpression>(input); 
            return CheckRegularExpression(request.Pattern, request.Flags, request.Text); 
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Некорректный JSON.", ex);
        }
    }

    private string CheckRegularExpression(string requestPattern, string requestFlags, string requestText)
    {
        try
        {
            var options = ParseFlags(requestFlags);
            var regex = new Regex(requestPattern, options);
            var matches = regex.Matches(requestText);

            if (matches.Count == 0)
                return "Совпадений не найдено.";

            var sb = new StringBuilder();
            sb.AppendLine($"Найдено {matches.Count} совпадения:");
            var index = 1;
            foreach (Match match in matches)
            {
                sb.AppendLine($"{index}. \"{match.Value}\"");

                if (match.Groups.Count > 1)
                {
                    sb.AppendLine("   Группы:");
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        sb.AppendLine($"      {i}: \"{match.Groups[i].Value}\"");
                    }
                }
                index++;
            }
            return sb.ToString();
        }
        catch (ArgumentException ex)
        {
            return $"Ошибка регулярного выражения: {ex.Message}";
        }
    }
    
    private RegexOptions ParseFlags(string flags)
    {
        var options = RegexOptions.None;

        foreach (char flag in flags)
        {
            switch (flag)
            {
                case 'i':
                    options |= RegexOptions.IgnoreCase;
                    break;

                case 'm':
                    options |= RegexOptions.Multiline;
                    break;

                case 's':
                    options |= RegexOptions.Singleline;
                    break;

                case 'x':
                    options |= RegexOptions.IgnorePatternWhitespace;
                    break;

                case 'g':
                    break;

                default:
                    throw new ArgumentException($"Неизвестный флаг '{flag}'.");
            }
        }
        return options;
    }
}

public class RegularExpression
{
    [JsonPropertyName("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [JsonPropertyName("flags")]
    public string Flags { get; set; } = string.Empty;
    
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}