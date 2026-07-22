using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace WebApp.Api.Services;

public class YamlJsonService : IUtilityService
{
    public string Endpoint => "yaml-json";


    public string Execute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Ввод не может быть пустым.");


        YamlJsonRequest? request;

        try
        {
            request = JsonSerializer.Deserialize<YamlJsonRequest>(input);
        }
        catch
        {
            throw new ArgumentException("Некорректный JSON запроса.");
        }


        if (request == null)
            throw new ArgumentException("Некорректный запрос.");


        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Контент не может быть пустым.");


        return request.Mode.ToLower() switch
        {
            "json2yaml" => JsonToYaml(request.Content),

            "yaml2json" => YamlToJson(request.Content),

            "validate" => Validate(
                request.Format,
                request.Content),

            _ => throw new ArgumentException("Неизвестный режим.")
        };
    }


    private string JsonToYaml(string json)
    {
        try
        {
            var node = JsonNode.Parse(json);

            var data = ConvertJsonNode(node);

            var serializer =
                new SerializerBuilder()
                    .WithIndentedSequences()
                    .Build();

            return serializer.Serialize(data);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException(
                $"Ошибка JSON. Позиция: {ex.BytePositionInLine}");
        }
    }


    private object? ConvertJsonNode(JsonNode? node)
    {
        if (node == null)
            return null;


        if (node is JsonObject obj)
        {
            var dictionary = new Dictionary<string, object?>();

            foreach (var item in obj)
            {
                dictionary[item.Key] = ConvertJsonNode(item.Value);
            }

            return dictionary;
        }


        if (node is JsonArray array)
        {
            var list = new List<object?>();

            foreach (var item in array)
            {
                list.Add(ConvertJsonNode(item));
            }

            return list;
        }


        if (node is JsonValue value)
        {
            if (value.TryGetValue<string>(out var str))
                return str;

            if (value.TryGetValue<int>(out var intValue))
                return intValue;

            if (value.TryGetValue<long>(out var longValue))
                return longValue;

            if (value.TryGetValue<double>(out var doubleValue))
                return doubleValue;

            if (value.TryGetValue<bool>(out var boolValue))
                return boolValue;

            return value.ToString();
        }


        return null;
    }


    private string YamlToJson(string yaml)
    {
        try
        {
            var deserializer =
                new DeserializerBuilder()
                    .Build();


            var yamlObject =
                deserializer.Deserialize<object>(yaml);


            return JsonSerializer.Serialize(
                yamlObject,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            throw new ArgumentException(
                $"Ошибка YAML. Строка: {ex.Start.Line}, позиция: {ex.Start.Column}");
        }
    }


    private string Validate(
        string format,
        string content)
    {
        try
        {
            if (format.ToLower() == "json")
            {
                JsonDocument.Parse(content);

                return "JSON корректный.";
            }


            if (format.ToLower() == "yaml")
            {
                var deserializer =
                    new DeserializerBuilder()
                        .Build();

                deserializer.Deserialize<object>(content);

                return "YAML корректный.";
            }


            throw new ArgumentException(
                "Неизвестный формат.");
        }
        catch (JsonException ex)
        {
            throw new ArgumentException(
                $"Ошибка JSON. Строка: {ex.LineNumber}, позиция: {ex.BytePositionInLine}");
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                $"Ошибка YAML: {ex.Message}");
        }
    }
}

public class YamlJsonRequest
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; } = string.Empty;


    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;


    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}