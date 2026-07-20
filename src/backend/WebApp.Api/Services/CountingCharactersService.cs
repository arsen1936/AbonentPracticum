using System.Text;

namespace WebApp.Api.Services;

public class CountingCharactersService : IUtilityService
{
    public string Endpoint => "text-stats";
    
    public string Execute(string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Пустая строка");
        }
        return GenerateOutputString(input);
    }
    
    private string GenerateOutputString(string input)
    {
        StringBuilder output = new();
        output.Append($"Символов (с пробелами): {GetCharactersNumberWithSpaces(input)}\n");
        output.Append($"Символов (без пробелов): {GetCharactersNumberWithoutSpaces(input)}\n");
        output.Append($"Слов: {GetWordsNumber(input)}\n");
        output.Append($"Строк: {GetLinesNumber(input)}\n");
        output.Append($"Предложений: {GetSentencesNumber(input)}");
        return output.ToString();
    }

    private int GetCharactersNumberWithSpaces(string input)
    {
        return  input.Length;
    }
    
    private int GetCharactersNumberWithoutSpaces(string input)
    {
        return  input.Replace(" ", "").Length;
    }

    private int GetWordsNumber(string input)
    {
        return input.Split(
            [' ', '\t', '\r', '\n'],
            StringSplitOptions.RemoveEmptyEntries
        ).Length;
    }

    private int GetLinesNumber(string input)
    {
        string[] lines = input.Split("\n");
        return lines.Length;
    }

    private int GetSentencesNumber(string input)
    {
        string[] sentences = input.Replace("!", ".").Replace("?", ".").Split('.').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        return sentences.Length;
    }
}