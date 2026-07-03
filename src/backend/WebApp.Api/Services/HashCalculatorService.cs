using System.Text;
using System.Security.Cryptography;

namespace WebApp.Api.Services;

public class HashCalculatorService : IUtilityService {
    public string Endpoint => "hash-calc";

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
            throw new Exception("Invalid input");
        }
    }

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