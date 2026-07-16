using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace WebApp.Api.Services;


/// <summary>
/// Предоставляет утилиту для одновременной замены нескольких подстрок в тексте.
/// </summary>
/// <remarks>
/// Утилита принимает JSON-объект со следующей структурой:
/// <code>
/// {
///   "text": "Hello, world!",
///   "replacements": {
///     "Hello": "Hi",
///     "world": "World"
///   }
/// }
/// </code>
/// Все замены выполняются за один проход по тексту. Если ключи пересекаются,
/// сначала сопоставляются более длинные строки, что предотвращает частичную
/// замену более длинных совпадений.
/// </remarks>
public class MultipleSubstitutionTextService : IUtilityService
{
    /// <summary>
    /// Идентификатор конечной точки утилиты.
    /// </summary>
    public string Endpoint => "multi-replace";
    
    
    /// <summary>
    /// Выполняет множественную замену подстрок в тексте.
    /// </summary>
    /// <param name="input">
    /// JSON-строка с полями:
    /// <list type="bullet">
    /// <item><description><c>text</c> — исходный текст.</description></item>
    /// <item><description><c>replacements</c> — словарь замен, где ключ — искомая строка, значение — строка для замены.</description></item>
    /// </list>
    /// </param>
    /// <returns>Текст после применения всех замен.</returns>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если:
    /// <list type="bullet">
    /// <item><description>входные данные не являются корректным JSON;</description></item>
    /// <item><description>отсутствует поле <c>text</c>;</description></item>
    /// <item><description>отсутствует поле <c>replacements</c>;</description></item>
    /// <item><description>словарь замен пуст.</description></item>
    /// </list>
    /// </exception>
    public string Execute(string input)
    {
        SubstitutionTextRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<SubstitutionTextRequest>(input);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Некорректный JSON.", ex);
        }

        if (request == null)
            throw new ArgumentException("Некорректный JSON.");

        if (request.Replacements == null)
            throw new ArgumentException("Поле replacements обязательно.");

        if (request.Replacements.Count == 0)
            throw new ArgumentException("Добавьте хотя бы одну замену.");

        if (string.IsNullOrEmpty(request.Text))
            throw new ArgumentException("Поле text обязательно.");

        return GetReplacementString(request.Text, request.Replacements);
    }
    
    /// <summary>
    /// Выполняет замену всех указанных подстрок в исходном тексте.
    /// </summary>
    /// <param name="input">Исходный текст.</param>
    /// <param name="replacements">
    /// Словарь замен, где ключ — заменяемая строка, значение — строка-замена.
    /// </param>
    /// <returns>Текст после применения всех замен.</returns>
    /// <remarks>
    /// Для корректной обработки пересекающихся ключей поиск выполняется по регулярному
    /// выражению, сформированному из ключей словаря, отсортированных по убыванию длины.
    /// Это гарантирует, что более длинные совпадения будут заменены раньше коротких.
    /// </remarks>
    private string GetReplacementString(string input, Dictionary<string, string> replacements)
    {
        var pattern = string.Join(
            "|",
            replacements.Keys
                .OrderByDescending(k => k.Length)
                .Select(Regex.Escape));

        return Regex.Replace(input, pattern, match => replacements[match.Value]);
    }
}

/// <summary>
/// Запрос на выполнение множественной замены текста.
/// </summary>
public class SubstitutionTextRequest
{
    /// <summary>
    /// Исходный текст, в котором требуется выполнить замену.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Словарь замен, где ключ — искомая строка, а значение — строка, на которую необходимо заменить найденное совпадение.
    /// </summary>
    [JsonPropertyName("replacements")]
    public Dictionary<string, string> Replacements { get; set; } = new();
}