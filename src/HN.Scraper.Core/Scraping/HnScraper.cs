using AngleSharp;
using AngleSharp.Dom;
using HN.Scraper.Core.Common;
using HN.Scraper.Core.Models;

namespace HN.Scraper.Core.Scraping;

public class HnScraper
{
    private readonly HttpClient _http;

    public HnScraper(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<HnItem>> ScrapeAsync(string url, int pageCount = 1, int takePerPage = 30)
    {
        if (string.IsNullOrWhiteSpace(url)) return new List<HnItem>();

        var results = new List<HnItem>();

        var currentUrl = url;
        for (var page = 1; page <= pageCount; page++)
        {
            var html = await _http.GetStringAsync(currentUrl);

            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            results.AddRange(ParsePage(document, currentUrl, page, takePerPage));

            // Stop if we don’t need more pages
            if (page == pageCount) break;

            // Find More link
            var moreHref = document.QuerySelector("a.morelink")?.GetAttribute("href");
            var nextUrl = Helpers.JoinAbsUrl(currentUrl, moreHref);

            if (string.IsNullOrWhiteSpace(nextUrl))
                break;

            currentUrl = nextUrl;
        }

        return results;
    }

    private static List<HnItem> ParsePage(IDocument document, string pageUrl, int pageNumber, int takePerPage)
    {
        var items = new List<HnItem>();

        var rows = document.QuerySelectorAll("tr.athing").Take(takePerPage);
        foreach (var row in rows)
        {
            // Title + Url
            var titleAnchor = row.QuerySelector(".titleline a");
            var title = Helpers.ElementToString(titleAnchor);
            if (string.IsNullOrWhiteSpace(title)) continue;

            var href = titleAnchor?.GetAttribute("href");
            var absUrl = Helpers.JoinAbsUrl(pageUrl, href);

            // Rank + ItemId
            var rankText = Helpers.ElementToString(row.QuerySelector("span.rank"));
            var rank = Helpers.StringToInt(rankText);
            var itemId = row.GetAttribute("id") ?? "";

            // Metadata row: author, points, age, comments
            var meta = row.NextElementSibling;
            var author = Helpers.ElementToString(meta?.QuerySelector("a.hnuser"));

            var pointsText = meta?.QuerySelector("span.score")?.TextContent;
            var points = Helpers.StringToInt(pointsText);

            var age = Helpers.ElementToString(meta?.QuerySelector("span.age a"));

            var commentsCount = ExtractCommentsCount(meta);

            items.Add(new HnItem(
                Title: title,
                Url: absUrl,
                Rank: rank,
                ItemId: itemId,
                Author: author,
                Points: points,
                CommentsCount: commentsCount,
                Age: age,
                Page: pageNumber
            ));
        }

        return items;
    }

    private static int ExtractCommentsCount(IElement? metaRow)
    {
        if (metaRow is null) return 0;

        // Most robust: look for link that is "discuss" or contains "comment"
        var candidates = metaRow.QuerySelectorAll("a");

        var commentsLink = candidates.LastOrDefault(a =>
        {
            var t = (a.TextContent ?? "").Trim().ToLowerInvariant();
            return t == "discuss" || t.Contains("comment");
        });

        if (commentsLink is null) return 0;

        var text = (commentsLink.TextContent ?? "").Trim();
        if (text.Equals("discuss", StringComparison.OrdinalIgnoreCase))
            return 0;

        return Helpers.StringToInt(text);
    }
}
