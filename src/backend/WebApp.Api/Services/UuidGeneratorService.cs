using System.Text;

namespace WebApp.Api.Services;

public class UuidGeneratorService : IUtilityService
{
    public string Endpoint => "uuid-gen";
    
    public string Execute(string input)
    {
        try
        {
            if (!int.TryParse(input, out var uuidCount))
                throw new ArgumentException("Количество строк должно быть целым числом");

            if (uuidCount <= 0)
                throw new ArgumentException("Количество строк должно быть больше 0");

            return GenerateSeveralUuid(uuidCount);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    private string GenerateUuid()
    {
        var guid = Guid.NewGuid();
        return guid.ToString("N");
    }

    private string GenerateSeveralUuid(int number)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < number; i++)
        {
            builder.AppendLine(GenerateUuid());
        }
        return builder.ToString().TrimEnd();
    }
}