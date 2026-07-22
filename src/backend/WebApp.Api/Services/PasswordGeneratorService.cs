using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class PasswordGeneratorService : IUtilityService
{
    public string Endpoint => "password-gen";

    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Symbols = "!@#$%^&*()-_=+[]{}<>?/|";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        PasswordGeneratorRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<PasswordGeneratorRequest>(input);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }

        if (request == null)
            throw new ArgumentException("Некорректный JSON.");

        if (request.Length < 4 || request.Length > 256)
            throw new ArgumentException("Длина пароля должна быть от 4 до 256 символов.");

        if (!request.UseUpper &&
            !request.UseLower &&
            !request.UseDigits &&
            !request.UseSymbols)
            throw new ArgumentException("Не выбран ни один набор символов.");

        return GeneratePassword(request);
    }

    private string GeneratePassword(PasswordGeneratorRequest request)
    {
        var allCharacters = new StringBuilder();
        var password = new List<char>();

        if (request.UseUpper)
        {
            allCharacters.Append(Upper);
            password.Add(GetRandomChar(Upper));
        }

        if (request.UseLower)
        {
            allCharacters.Append(Lower);
            password.Add(GetRandomChar(Lower));
        }

        if (request.UseDigits)
        {
            allCharacters.Append(Digits);
            password.Add(GetRandomChar(Digits));
        }

        if (request.UseSymbols)
        {
            allCharacters.Append(Symbols);
            password.Add(GetRandomChar(Symbols));
        }

        while (password.Count < request.Length)
        {
            password.Add(GetRandomChar(allCharacters.ToString()));
        }

        Shuffle(password);

        return new string(password.Take(request.Length).ToArray());
    }

    private static char GetRandomChar(string chars)
    {
        return chars[RandomNumberGenerator.GetInt32(chars.Length)];
    }

    private static void Shuffle(List<char> chars)
    {
        for (int i = chars.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }
}

public class PasswordGeneratorRequest
{
    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("useUpper")]
    public bool UseUpper { get; set; }

    [JsonPropertyName("useLower")]
    public bool UseLower { get; set; }

    [JsonPropertyName("useDigits")]
    public bool UseDigits { get; set; }

    [JsonPropertyName("useSymbols")]
    public bool UseSymbols { get; set; }
}