namespace WebApp.Api.Services;

public class TextToListService : IUtilityService
{
    public string Endpoint => "text-to-list";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var lines = input.Split('\n', 2, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 2)
        {
            return GetGeneratedList(lines);
        }
        else
        {
            throw new Exception("Invalid input");
        }
    }

    private string GetGeneratedList(string[] lines)
    {  
        var result = lines[0].ToLower() switch
        {
            "java" => ToJavaList(lines[1]),
            "csharp" => ToCSList(lines[1]),
            "js" => ToJavaScriptArray(lines[1]),
            "python" => ToPythonList(lines[1]),
            "sql" => ToSqlInList(lines[1]),
            _ => throw new Exception("Неправильный язык")
        };
        return result;
    }

    private string ToJavaList(string input)
    {
        string list = "List<String> items = new ArrayList<>(Arrays.asList(\n";
        foreach (string line in input.Split("\n"))
        {
            list = list + "\"" + line + "\",\n";
        }

        list = list.Substring(0, list.Length - 2);
        list += "));";
        return list;
    }

    private string ToCSList(string input)
    {
        string list = "var items = new List<string>\n{";
        foreach (string line in input.Split("\n"))
        {
            list = list + "\"" + line + "\",\n";
        }

        list = list.Substring(0, list.Length - 2);
        list += "};";
        return list;
    }

    private string ToJavaScriptArray(string input)
    {
        string array = "const items = [\n";
        foreach (string line in input.Split("\n"))
        {
            array += $"\"{line}\",\n";
        }

        array = array.Substring(0, array.Length - 2);
        array += "\n];";
        return array;
    }

    private string ToPythonList(string input)
    {
        string list = "items = [\n";
        foreach (string line in input.Split("\n"))
        {
            list += $"    \"{line}\",\n";
        }

        list = list.Substring(0, list.Length - 2);
        list += "\n]";
        return list;
    }

    private string ToSqlInList(string input)
    {
        string list = "IN (\n";
        foreach (string line in input.Split("\n"))
        {
            list += $"    '{line}',\n";
        }

        list = list.Substring(0, list.Length - 2);
        list += "\n)";
        return list;
    }
}