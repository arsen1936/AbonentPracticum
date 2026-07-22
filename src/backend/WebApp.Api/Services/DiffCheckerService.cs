using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Api.Services;

public class DiffCheckerService : IUtilityService
{
    public string Endpoint => "text-diff";

    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");

        DiffCheсkerRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<DiffCheсkerRequest>(input);
        }
        catch (JsonException)
        {
            throw new ArgumentException("Некорректный JSON.");
        }


        if (request == null)
            throw new ArgumentException("Некорректный JSON.");

        if (string.IsNullOrWhiteSpace(request.Left) &&
            string.IsNullOrWhiteSpace(request.Right))
            throw new ArgumentException("Тексты не могут быть пустыми.");

        return CreateDiff(request.Left, request.Right);
    }


    private string CreateDiff(string left, string right)
    {
        var leftLines = left.Replace("\r", "").Split('\n');
        var rightLines = right.Replace("\r", "").Split('\n');

        var lcs = BuildLcs(leftLines, rightLines);

        var builder = new StringBuilder();

        int i = 0;
        int j = 0;

        while (i < leftLines.Length && j < rightLines.Length)
        {
            if (leftLines[i] == rightLines[j])
            {
                builder.AppendLine($"  {leftLines[i]}");

                i++;
                j++;
            }
            else if (lcs[i + 1, j] >= lcs[i, j + 1])
            {
                builder.AppendLine($"- {leftLines[i]}");

                i++;
            }
            else
            {
                builder.AppendLine($"+ {rightLines[j]}");

                j++;
            }
        }


        while (i < leftLines.Length)
        {
            builder.AppendLine($"- {leftLines[i]}");
            i++;
        }


        while (j < rightLines.Length)
        {
            builder.AppendLine($"+ {rightLines[j]}");
            j++;
        }

        return builder.ToString().TrimEnd();
    }


    private int[,] BuildLcs(string[] left, string[] right)
    {
        var table = new int[left.Length + 1, right.Length + 1];

        for (int i = left.Length - 1; i >= 0; i--)
        {
            for (int j = right.Length - 1; j >= 0; j--)
            {
                if (left[i] == right[j])
                {
                    table[i, j] = table[i + 1, j + 1] + 1;
                }
                else
                {
                    table[i, j] = Math.Max(
                        table[i + 1, j],
                        table[i, j + 1]
                    );
                }
            }
        }

        return table;
    }
}


public class DiffCheсkerRequest
{
    [JsonPropertyName("left")]
    public string Left { get; set; } = string.Empty;


    [JsonPropertyName("right")]
    public string Right { get; set; } = string.Empty;
}