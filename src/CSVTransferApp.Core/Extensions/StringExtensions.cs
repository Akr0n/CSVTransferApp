namespace CSVTransferApp.Core.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string ToSafeFileName(this string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(fileName.Where(ch => !invalidChars.Contains(ch)).ToArray());
    }

    public static string ReplaceTokens(this string template, Dictionary<string, string> tokens)
    {
        return tokens.Aggregate(template, (current, token) => 
            current.Replace($"{{{token.Key}}}", token.Value));
    }

    public static string TruncateWithEllipsis(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? string.Empty;

        return value.Substring(0, maxLength - 3) + "...";
    }
}
