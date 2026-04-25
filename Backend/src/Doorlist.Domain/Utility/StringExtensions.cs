namespace Doorlist.Domain.Utility;

/// <summary>
/// Extend string functionality to capitalise. 
/// </summary>
public static class StringExtensions
{
    public static string Capitalise(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return str;
        return Char.ToUpper(str[0]) + str[1..]; // C# 8.0 means it won't error on str.Length < 2 with the range operator
    }
}