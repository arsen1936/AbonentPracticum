using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class DateCalculatorService : IUtilityService
{
    public string Endpoint => "date-calc";
    
    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        DateCalculatorRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<DateCalculatorRequest>(input);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }

        if (request == null)
            throw new ArgumentException("Некорректный JSON.");

        return request.Operation switch
        {
            "diff" => SubtractDate(request.Date1, request.Date2!),

            "add" => AddDate(
                request.Date1,
                request.Amount ?? throw new ArgumentException("Не указано количество."),
                request.Unit ?? throw new ArgumentException("Не указана единица измерения.")),

            _ => throw new ArgumentException("Неизвестная операция.")
        };
    }

    private string AddDate(string date, int amount, string unit)
    {
        var value = DateOnly.ParseExact(date, "yyyy-MM-dd");

        value = unit switch
        {
            "days" => value.AddDays(amount),
            "months" => value.AddMonths(amount),
            "years" => value.AddYears(amount),
            _ => throw new ArgumentException("Неизвестная единица измерения.")
        };

        return value.ToString("yyyy-MM-dd");
    }

    private string SubtractDate(string first, string second)
    {
        var date1 = DateOnly.ParseExact(first, "yyyy-MM-dd");
        var date2 = DateOnly.ParseExact(second, "yyyy-MM-dd");

        if (date2 < date1)
            (date1, date2) = (date2, date1);

        int years = 0;
        int months = 0;

        var temp = date1;

        while (temp.AddYears(1) <= date2)
        {
            temp = temp.AddYears(1);
            years++;
        }

        while (temp.AddMonths(1) <= date2)
        {
            temp = temp.AddMonths(1);
            months++;
        }

        int days = date2.DayNumber - temp.DayNumber;
        int totalDays = date2.DayNumber - date1.DayNumber;

        return $"Разница: {totalDays} дней ({years} лет, {months} месяцев, {days} дней)";
    }
}

public class DateCalculatorRequest
{
    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty;

    [JsonPropertyName("date1")]
    public string Date1 { get; set; } = string.Empty;

    [JsonPropertyName("date2")]
    public string? Date2 { get; set; }

    [JsonPropertyName("amount")]
    public int? Amount { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}