using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace HN.Scraper.Cli.Common;

public static class Helpers
{
    public static string ElementToString(IElement? el) =>
        el?.TextContent?.Trim() ?? "";

    public static int StringToInt(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;

        var digits = Regex.Replace(text, @"[^\d]", "");
        return int.TryParse(digits, out var n) ? n : 0;
    }

    public static string JoinAbsUrl(string baseUrl, string? href)
    {
        if (string.IsNullOrWhiteSpace(href)) return "";
        return new Uri(new Uri(baseUrl), href).ToString();
    }
}
