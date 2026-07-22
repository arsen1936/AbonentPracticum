using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Api.Services;

public class JwtDebuggerService : IUtilityService
{
    public string Endpoint => "jwt-debugger";


    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");


        var lines = input.Replace("\r", "")
                         .Split('\n', 2);


        if (lines.Length < 2)
            throw new ArgumentException("Ожидается режим и данные.");


        var mode = lines[0]
            .Trim()
            .ToLower();


        var data = lines[1].Trim();


        return mode switch
        {
            "decode" => Decode(data),
            "generate" => Generate(data),
            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }


    private string Decode(string token)
    {
        var parts = token.Split('.');


        if (parts.Length != 3)
            throw new ArgumentException("Некорректный JWT токен.");


        try
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);


            var header = JsonSerializer.Serialize(
                jwt.Header,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });


            var payload = JsonSerializer.Serialize(
                jwt.Payload,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });


            var result = new StringBuilder();


            result.AppendLine("=== HEADER ===");
            result.AppendLine(header);

            result.AppendLine();

            result.AppendLine("=== PAYLOAD ===");
            result.AppendLine(payload);


            if (jwt.Payload.TryGetValue("exp", out var expValue))
            {
                var exp = Convert.ToInt64(expValue);

                var expiration =
                    DateTimeOffset.FromUnixTimeSeconds(exp);


                var now = DateTimeOffset.UtcNow;


                if (expiration < now)
                {
                    result.AppendLine();
                    result.AppendLine("Токен истёк.");
                }
                else
                {
                    var left = expiration - now;

                    result.AppendLine();
                    result.AppendLine(
                        $"Токен НЕ истёк. Осталось: {left.Days} дней."
                    );
                }


                result.AppendLine(
                    $"Дата окончания: {expiration.LocalDateTime}"
                );
            }


            return result.ToString().TrimEnd();
        }
        catch
        {
            throw new ArgumentException("Некорректный JWT токен.");
        }
    }


    private string Generate(string json)
    {
        Dictionary<string, object>? claims;


        try
        {
            claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch
        {
            throw new ArgumentException("Некорректный JSON.");
        }


        if (claims == null)
            throw new ArgumentException("Claims не найдены.");


        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("test-secret-key-for-jwt-256-bits"));


        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);


        var jwtClaims = claims
            .Select(x =>
                new Claim(
                    x.Key,
                    x.Value.ToString() ?? string.Empty))
            .ToList();


        var token = new JwtSecurityToken(
            claims: jwtClaims,
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}