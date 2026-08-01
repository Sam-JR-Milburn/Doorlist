namespace Doorlist.Domain.Utility.Extensions;

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

    public static string GetLowercaseAlphabet()
    {
        return new string(Enumerable.Range('a', 26).Select(n => (char)n).ToArray());
    }
    
    public static string GetUppercaseAlphabet()
    {
        return new string(Enumerable.Range('A', 26).Select(n => (char)n).ToArray());
    }

    public static string GetDigits()
    {
        return new string("0123456789");
    }
}