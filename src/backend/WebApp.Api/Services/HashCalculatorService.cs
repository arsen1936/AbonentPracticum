using System.Text;
using System.Security.Cryptography;

namespace WebApp.Api.Services;

/// <summary>
/// Предоставляет утилиту для вычисления криптографического хэша строки.
/// </summary>
/// <remarks>
/// Входные данные должны состоять из двух строк:
/// <list type="number">
/// <item>
/// <description>Название алгоритма хэширования (<c>md5</c>, <c>sha1</c>, <c>sha256</c> или <c>sha512</c>).</description>
/// </item>
/// <item>
/// <description>Строка, для которой необходимо вычислить хэш.</description>
/// </item>
/// </list>
/// Результатом является шестнадцатеричное представление вычисленного хэша в верхнем регистре.
/// </remarks>
public class HashCalculatorService : IUtilityService {
    /// <summary>
    /// Идентификатор конечной точки утилиты.
    /// </summary>
    public string Endpoint => "hash-calc";

    /// <summary>
    /// Вычисляет хэш переданной строки с использованием указанного алгоритма.
    /// </summary>
    /// <param name="input">
    /// Входные данные в формате:
    /// <code>
    /// &lt;алгоритм&gt;
    /// &lt;текст&gt;
    /// </code>
    /// Например:
    /// <code>
    /// sha256
    /// Hello, world!
    /// </code>
    /// </param>
    /// <returns>
    /// Шестнадцатеричная строка, содержащая вычисленный хэш.
    /// Если входная строка пуста или содержит только пробельные символы, возвращается <c>"0"</c>.
    /// </returns>
    /// <exception cref="Exception">
    /// Выбрасывается, если входные данные имеют неверный формат
    /// или указан неподдерживаемый алгоритм хэширования.
    /// </exception>
    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "0";

        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length == 2)
        {
            return CalculateHash(lines);
        }
        else
        {
            throw new Exception("Некорректный ввод");
        }
    }

    /// <summary>
    /// Вычисляет хэш строки с использованием указанного алгоритма.
    /// </summary>
    /// <param name="lines">
    /// Массив из двух элементов:
    /// <list type="bullet">
    /// <item><description><c>lines[0]</c> — название алгоритма.</description></item>
    /// <item><description><c>lines[1]</c> — исходная строка.</description></item>
    /// </list>
    /// </param>
    /// <returns>Хэш в шестнадцатеричном формате (верхний регистр).</returns>
    /// <exception cref="Exception">
    /// Выбрасывается при указании неподдерживаемого алгоритма.
    /// </exception>
    private string CalculateHash(string[] lines)
    {
        var method =  lines[0].ToLower();
        var key =  Encoding.UTF8.GetBytes(lines[1]);
        var hash = method switch
        {
            "md5" => MD5.HashData(key),
            "sha1" => SHA1.HashData(key),
            "sha256" => SHA256.HashData(key),
            "sha512" => SHA512.HashData(key),
            _ => throw new Exception()
        };
        return Convert.ToHexString(hash);
    }
}