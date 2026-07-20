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
            throw new Exception("Некорректный формат введённой строки");
        }
    }

    private string GetSortedLines(string sortingOrder, string[] lines)
    {
        string sortedLines = sortingOrder.ToLower() switch
        {
            "asc" => GetSortedLines(lines, true),
            "desc" => GetSortedLines(lines, false),
            _ => throw new Exception("Неправильно указан способ сортировки")
        };
        return sortedLines;
    }

    private string GetSortedLines(string[] lines, bool ascending)
    {
        var comparer = ascending
            ? Comparer<string>.Default
            : Comparer<string>.Create((a, b) => string.Compare(b, a));

        var sortedLines = new SortedSet<string>(comparer);

        foreach (var line in lines)
        {
            sortedLines.Add(line);
        }

        return string.Join(", ", sortedLines);
    }
}