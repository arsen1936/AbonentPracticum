using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace WebApp.Api.Services;

public class CsvToJsonConverterService : IUtilityService
{
    public string Endpoint => "csv-json";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        var lines = input.Replace("\r", "").Split('\n', 2);

        if (lines.Length != 2)
            throw new ArgumentException("Ожидается режим и данные.");

        return lines[0] switch
        {
            "csv-to-json" => CsvToJson(lines[1]),
            "json-to-csv" => JsonToCsv(lines[1]),
            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }

    private string CsvToJson(string csv)
    {
        var rows = ParseCsv(csv);

        if (rows.Count < 2)
            throw new ArgumentException("CSV должен содержать заголовок и хотя бы одну строку.");

        var headers = rows[0];
        var result = new List<Dictionary<string, string>>();

        foreach (var row in rows.Skip(1))
        {
            if (row.Count != headers.Count)
                throw new ArgumentException("Количество столбцов не совпадает.");

            var obj = new Dictionary<string, string>();

            for (int i = 0; i < headers.Count; i++)
                obj[headers[i]] = row[i];

            result.Add(obj);
        }
        return JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    private string JsonToCsv(string json)
    {
        JsonElement root;

        try
        {
            root = JsonDocument.Parse(json).RootElement;
        }
        catch
        {
            throw new ArgumentException("Некорректный JSON.");
        }

        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
            throw new ArgumentException("JSON должен быть непустым массивом.");

        var first = root[0];

        var headers = first.EnumerateObject()
            .Select(p => p.Name)
            .ToList();

        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", headers));

        foreach (var item in root.EnumerateArray())
        {
            var values = headers
                .Select(h => EscapeCsv(item.GetProperty(h).ToString()));

            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString().TrimEnd();
    }

    private static List<List<string>> ParseCsv(string csv)
    {
        var result = new List<List<string>>();

        using var reader = new StringReader(csv);

        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            result.Add(ParseCsvLine(line));
        }

        return result;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();

        var current = new StringBuilder();

        bool quoted = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (c == ',' && !quoted)
            {
                values.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        values.Add(current.ToString());

        return values;
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains('"'))
            value = value.Replace("\"", "\"\"");

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            value = $"\"{value}\"";

        return value;
    }
}