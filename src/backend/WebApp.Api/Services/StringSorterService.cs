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
            throw new Exception("Неккоректный формат ввелённой строки");//перепроверить исключения, проверить ранние утилиты
        }
    }

    private string GetSortedLines(string sortingOrder, string[] lines)
    {
        string sortedLines = sortingOrder.ToLower() switch
        {
            "asc" => GetASCLines(lines),
            "desc" => GetDescLines(lines),
            _ => throw new Exception("Неправильно указан способ сортировки")
        };
        return sortedLines;
    }

    //обхединить в один метод
    private string GetASCLines(string[] lines)
    {
        var asc = new SortedSet<string>();
        foreach (var line in lines)
        {
            asc.Add(line);
        }

        return string.Join(", ", asc);
    }

    private string GetDescLines(string[] lines)
    {
        var desc = new SortedSet<string>(
            Comparer<string>.Create((a, b) => string.Compare(b, a)));
        foreach (var line in lines)
        {
            desc.Add(line);
        }

        return string.Join(", ", desc);
    }
}