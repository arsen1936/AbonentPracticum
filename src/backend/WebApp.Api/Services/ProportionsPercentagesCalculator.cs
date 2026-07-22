using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class ProportionsPercentagesCalculator : IUtilityService
{
    public string Endpoint => "percent-calc";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        ProportionsPercentagesCalculatorRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<ProportionsPercentagesCalculatorRequest>(input);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }

        if (request == null)
            throw new ArgumentException("Некорректный JSON.");

        if (string.IsNullOrWhiteSpace(request.Operetion))
            throw new ArgumentException("Не указана операция.");

        if (!long.TryParse(request.Value1, out var value1))
            throw new ArgumentException("value1 должно быть числом.");

        if (!long.TryParse(request.Value2, out var value2))
            throw new ArgumentException("value2 должно быть числом.");

        return Calculate(request.Operetion, request.Value1, request.Value2);
    }

    private string Calculate(string operation, string value1, string value2)
    {
        var val1 = Convert.ToInt64(value1);
        var val2 = Convert.ToInt64(value2);

        return operation switch
        {
            "percent-of" => CalculatePercentOf(val1, val2),
            "change" => CalculateChange(val1, val2),
            "proportion" => CalculateProportion(val1, val2),
            _ => throw new ArgumentException("Неизвестная операция.")
        };
    }

    private string CalculatePercentOf(long value, long total)
    {
        if (total == 0)
            throw new ArgumentException("Нельзя делить на ноль.");

        double percent = (double)value / total * 100;

        return $"{value} составляет {percent:0.##}% от {total}";
    }

    private string CalculateChange(long oldValue, long newValue)
    {
        if (oldValue == 0)
            throw new ArgumentException("Начальное значение не может быть равно нулю.");

        long difference = newValue - oldValue;
        double percent = (double)difference / oldValue * 100;

        string sign = difference >= 0 ? "+" : "";

        return $"Изменение: {sign}{percent:0.00}% ({(difference >= 0 ? "увеличение" : "уменьшение")} на {Math.Abs(difference)})";
    }

    private string CalculateProportion(long value, long percent)
    {
        double result = value * (1 + percent / 100.0);

        return $"{value}, увеличенное на {percent}% = {result:0.##}";
    }
}

public class ProportionsPercentagesCalculatorRequest
{
    [JsonPropertyName("operation")]
    public string Operetion { get; set; } = string.Empty;
    
    [JsonPropertyName("value1")]
    public string Value1 { get; set; } = string.Empty;
    
    [JsonPropertyName("value2")]
    public string Value2 { get; set; } = string.Empty;
}