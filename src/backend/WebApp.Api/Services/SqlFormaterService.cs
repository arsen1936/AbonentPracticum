using System.Text;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;

public class SqlFormaterService : IUtilityService
{
    public string Endpoint => "sql-formatter";

    private static readonly string[] Keywords =
    [
        "SELECT",
        "FROM",
        "WHERE",
        "JOIN",
        "LEFT JOIN",
        "RIGHT JOIN",
        "INNER JOIN",
        "OUTER JOIN",
        "GROUP BY",
        "ORDER BY",
        "HAVING",
        "LIMIT",
        "UNION"
    ];

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        var lines = input.Replace("\r", "")
                         .Split('\n', 2);

        if (lines.Length < 2)
            throw new ArgumentException("Ожидается режим и SQL запрос.");

        var mode = lines[0].Trim().ToLower();
        var sql = lines[1].Trim();

        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("SQL запрос не может быть пустым.");

        return mode switch
        {
            "beautify" => Beautify(sql),
            "minify" => Minify(sql),
            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }


    private string Minify(string sql)
    {
        sql = Regex.Replace(sql, @"\s+", " ");

        sql = Regex.Replace(
            sql,
            @"\s*([(),;])\s*",
            "$1"
        );

        return sql.Trim();
    }


    private string Beautify(string sql)
    {
        sql = Minify(sql);

        sql = Regex.Replace(
            sql,
            @"\b(select|from|where|join|left join|right join|inner join|group by|order by|having|limit|union)\b",
            m => m.Value.ToUpper(),
            RegexOptions.IgnoreCase);


        sql = Regex.Replace(
            sql,
            @"\s*(SELECT|FROM|WHERE|JOIN|LEFT JOIN|RIGHT JOIN|INNER JOIN|GROUP BY|ORDER BY|HAVING|LIMIT|UNION)\s*",
            "\n$1 ",
            RegexOptions.IgnoreCase);


        sql = Regex.Replace(
            sql,
            @"\s*,\s*",
            ",\n"
        );


        sql = Regex.Replace(
            sql,
            @"\s*\(\s*",
            "(\n"
        );


        sql = Regex.Replace(
            sql,
            @"\s*\)\s*",
            "\n)"
        );

        sql = FormatSelectColumns(sql);
        
        return ApplyIndent(sql);
    }


    private string FormatSelectColumns(string sql)
    {
        var match = Regex.Match(
            sql,
            @"SELECT (.*?)\nFROM",
            RegexOptions.Singleline |
            RegexOptions.IgnoreCase
        );

        if (!match.Success)
            return sql;
        
        var columns = match.Groups[1]
            .Value
            .Split(',');
        
        var builder = new StringBuilder();

        builder.AppendLine("SELECT");

        for (int i = 0; i < columns.Length; i++)
        {
            builder.Append("    ");
            builder.Append(columns[i].Trim());

            if (i < columns.Length - 1)
                builder.Append(",");

            builder.Append('\n');
        }

        builder.Append("FROM");
        
        return sql.Replace(
            "SELECT\n" + match.Groups[1].Value + "\nFROM",
            builder.ToString()
        );
    }
    
    private string ApplyIndent(string sql)
    {
        var lines = sql.Split('\n');

        var builder = new StringBuilder();

        int indent = 0;

        foreach (var item in lines)
        {
            var line = item.Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;


            if (line.StartsWith(")"))
                indent--;


            builder.Append(new string(' ', Math.Max(indent, 0) * 4));
            builder.Append(line);
            builder.Append('\n');


            if (line.EndsWith("("))
                indent++;

            if (line.Contains("(") && !line.EndsWith("("))
                indent++;
        }

        return builder.ToString().TrimEnd();
    }
}