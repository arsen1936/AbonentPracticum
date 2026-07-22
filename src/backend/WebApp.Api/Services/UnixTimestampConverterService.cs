using System.Globalization;

namespace WebApp.Api.Services;

public class UnixTimestampConverterService : IUtilityService
{
    public string Endpoint => "unix-time";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        string[] lines = input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (lines.Length != 2)
            throw new ArgumentException("Ожидается две строки: режим и значение.");

        return CalculateTime(lines);
    }

    private string CalculateTime(string[] input)
    {
        return input[0] switch
        {
            "to-date" => ToDate(input[1]),
            "to-timestamp" => ToTimestamp(input[1]),
            _ => throw new ArgumentException(
                "Неизвестный режим. Используйте 'to-date' или 'to-timestamp'.")
        };
    }

    private string ToTimestamp(string input)
    {
        if (!DateTimeOffset.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var dateTimeOffset))
        {
            throw new ArgumentException("Некорректная дата.");
        }

        return dateTimeOffset.ToUnixTimeSeconds().ToString();
    }

    private string ToDate(string input)
    {
        if (!long.TryParse(input, out long unixSeconds))
            throw new ArgumentException("Unix timestamp должен быть целым числом.");

        try
        {
            DateTimeOffset dateTimeOffset =
                DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

            return dateTimeOffset.UtcDateTime.ToString(
                "dd.MM.yyyy HH:mm:ss '(UTC)'",
                CultureInfo.InvariantCulture);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentException("Unix timestamp вне допустимого диапазона.");
        }
    }
}