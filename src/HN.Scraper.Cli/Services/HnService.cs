using HN.Scraper.Cli.Common;
using HN.Scraper.Cli.Models;
using HN.Scraper.Cli.Scraping;

namespace HN.Scraper.Cli.Services;

public enum HnSource
{
    Top,
    Newest
}

public class HnService
{
    private readonly HnScraper _scraper;

    public HnService(HnScraper scraper)
    {
        _scraper = scraper;
    }

    public async Task<List<HnItem>> GetAsync(
        HnSource source,
        int pageCount = 1,
        int takePerPage = 30,
        int minPoints = 0)
    {
        var url = source == HnSource.Newest ? HnUrls.Newest : HnUrls.Top;

        var items = await _scraper.ScrapeAsync(url, pageCount, takePerPage);

        if (minPoints > 0)
            items = items.Where(x => x.Points >= minPoints).ToList();

        items = items
            .OrderByDescending(x => x.Points)
            .ThenBy(x => x.Title)
            .ToList();

        return items;
    }
}
