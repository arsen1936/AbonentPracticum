namespace WebApp.Api.Services;

public class StringSorterService : IUtilityService
{
    public string Endpoint => "string-sort";

    public string Execute(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length >= 2)
        {
            return GetSortedLines(lines[0], lines[1..]);
        }
        else
        {
            throw new Exception("Input string was not in a correct format");
        }
    }

    private string GetSortedLines(string sortingOrder, string[] lines)
    {
        string sortedLines = sortingOrder.ToLower() switch
        {
            "asc" => getASCLines(lines),
            "desc" => getDECLines(lines),
            _ => throw new Exception("Неправильно указан способ сортировки")
        };
        return sortedLines;
    }

    private string getASCLines(string[] lines)
    {
        var asc = new SortedSet<string>();
        foreach (var line in lines)
        {
            if (line.Any(char.IsWhiteSpace))
            {
                throw new Exception("Строка должна содержать только одно слово.");
            }
            else
            {
                asc.Add(line);
            }
        }

        return string.Join(", ", asc);
    }

    private string getDECLines(string[] lines)
    {
        var desc = new SortedSet<string>(
            Comparer<string>.Create((a, b) => string.Compare(b, a)));
        foreach (var line in lines)
        {
            if (line.Any(char.IsWhiteSpace))
            {
                throw new Exception("Строка должна содержать только одно слово.");
            }
            else
            {
                desc.Add(line);
            }
        }

        return string.Join(", ", desc);
    }
}