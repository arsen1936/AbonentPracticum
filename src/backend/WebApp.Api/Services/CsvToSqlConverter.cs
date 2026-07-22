using System.Globalization;
using System.Text;

namespace WebApp.Api.Services;

public class CsvToSqlConverter : IUtilityService
{
    public string Endpoint => "csv-to-sql";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        var parts = input.Replace("\r", "").Split('\n', 2);

        if (parts.Length != 2)
            throw new ArgumentException("Ожидается имя таблицы и CSV.");

        var tableName = parts[0].Trim();

        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Не указано имя таблицы.");

        return ConvertToSql(tableName, parts[1]);
    }

    private string ConvertToSql(string tableName, string csv)
    {
        var rows = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                      .Select(r => r.Split(','))
                      .ToList();

        if (rows.Count < 2)
            throw new ArgumentException("CSV должен содержать заголовок и хотя бы одну строку.");

        var headers = rows[0];

        foreach (var row in rows.Skip(1))
        {
            if (row.Length != headers.Length)
                throw new ArgumentException("Количество столбцов не совпадает.");
        }

        var types = DetermineTypes(rows.Skip(1).ToList());

        var sb = new StringBuilder();

        sb.AppendLine($"CREATE TEMP TABLE {tableName} (");

        for (int i = 0; i < headers.Length; i++)
        {
            sb.Append($"    {headers[i]} {types[i]}");

            if (i < headers.Length - 1)
                sb.Append(",");

            sb.AppendLine();
        }

        sb.AppendLine(");");

        foreach (var row in rows.Skip(1))
        {
            sb.Append($"INSERT INTO {tableName} VALUES (");

            for (int i = 0; i < row.Length; i++)
            {
                sb.Append(ConvertValue(row[i], types[i]));

                if (i < row.Length - 1)
                    sb.Append(", ");
            }

            sb.AppendLine(");");
        }

        return sb.ToString().TrimEnd();
    }

    private static List<string> DetermineTypes(List<string[]> rows)
    {
        var types = new List<string>();

        for (int col = 0; col < rows[0].Length; col++)
        {
            bool isInt = true;
            bool isReal = true;

            foreach (var row in rows)
            {
                var value = row[col];

                if (!long.TryParse(value, out _))
                    isInt = false;

                if (!double.TryParse(
                        value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out _))
                    isReal = false;
            }

            if (isInt)
                types.Add("INT");
            else if (isReal)
                types.Add("REAL");
            else
                types.Add("TEXT");
        }

        return types;
    }

    private static string ConvertValue(string value, string type)
    {
        if (type == "TEXT")
            return $"'{value.Replace("'", "''")}'";

        return value;
    }
}