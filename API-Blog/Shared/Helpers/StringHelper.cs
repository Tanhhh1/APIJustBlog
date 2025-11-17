using System.Text.RegularExpressions;

namespace Shared.Helpers;

public static class StringHelper
{
    public static string RandomString(int length = 32)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string NormalizeString(this string input, string replacement = " ")
    {
        return string.IsNullOrWhiteSpace(input) ? "" : Regex.Replace(input.Trim(), @"\s+", replacement);
    }
}