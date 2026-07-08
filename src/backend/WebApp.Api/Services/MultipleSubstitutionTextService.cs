using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

public class MultipleSubstitutionTextService : IUtilityService
{
    public string Endpoint => "multi-replace";
    
    public string Execute(string input)
    {
        SubstitutionTextRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<SubstitutionTextRequest>(input);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Некорректный JSON.", ex);
        }

        if (request == null)
            throw new ArgumentException("Пустой JSON.");

        if (string.IsNullOrEmpty(request.Text))
            throw new ArgumentException("Поле text обязательно.");

        if (request.Replacements == null)
            throw new ArgumentException("Поле replacements обязательно.");

        if (request.Replacements.Keys.Any(string.IsNullOrEmpty))
            throw new ArgumentException("Ключи replacements не могут быть пустыми.");

        return GetReplacementString(request.Text, request.Replacements);
    }
    
    private string GetReplacementString(string input, Dictionary<string, string> replacements)
    {
        var pattern = string.Join(
            "|",
            replacements.Keys
                .OrderByDescending(k => k.Length)
                .Select(Regex.Escape));

        return Regex.Replace(input, pattern, match => replacements[match.Value]);
    }
}

public class SubstitutionTextRequest
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("replacements")]
    public Dictionary<string, string> Replacements { get; set; } = new();
}