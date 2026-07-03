using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class NumberConverterService : IUtilityService
{
    private static readonly Dictionary<string, int> NumbersBase = new()
    {
        ["bin"] = 2,
        ["oct"] = 8,
        ["dec"] = 10,
        ["hex"] = 16
    };

    public string Endpoint => "number-base";
    
    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        try
        {
            var request = JsonSerializer.Deserialize<NumberConverterRequest>(input);
            return ConvertNumber(
                request.Value,
                request.FromBase,
                request.ToBase);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private string ConvertNumber(string value, string fromBaseNumber, string toBaseNumber)
    {
        if (!NumbersBase.ContainsKey(toBaseNumber))
            throw new Exception($"Неизвестная система счисления: '{toBaseNumber}'");

        var decimalValue = ConvertToDecimal(value, fromBaseNumber);

        return Convert.ToString(decimalValue, NumbersBase[toBaseNumber]);
    }

    private long ConvertToDecimal(string value, string baseNumber)
    {
        if (NumberBaseRegexMatch(value, baseNumber))
        {
            return Convert.ToInt32(value, NumbersBase[baseNumber]);
        }
        else
        {
            throw new Exception("Неправильно введено число");
        }
    }

    private bool NumberBaseRegexMatch(string value, string baseNumber)
    {
        baseNumber = baseNumber.Trim().ToLowerInvariant();
        value = value.Trim().ToLowerInvariant();

        return baseNumber switch
        {
            "dec" => Regex.IsMatch(value, "^[0-9]+$"),
            "hex" => Regex.IsMatch(value, "^[0-9a-f]+$"),
            "oct" => Regex.IsMatch(value, "^[0-7]+$"),
            "bin" => Regex.IsMatch(value, "^[01]+$"),
            _ => throw new Exception($"Неизвестная система счисления: '{baseNumber}'")
        };
    }
}


public class NumberConverterRequest
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("fromBase")]
    public string FromBase { get; set; } = string.Empty;

    [JsonPropertyName("toBase")]
    public string ToBase { get; set; } = string.Empty;
}