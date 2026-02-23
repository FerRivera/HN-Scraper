using System.Text.Json;
using HN.Scraper.Core.Scraping;
using HN.Scraper.Core.Services;

using var http = new HttpClient();

var scraper = new HnScraper(http);
var service = new HnService(scraper);

var pageCount = 2;
var takePerPage = 30;
var minPoints = 100;

var top = await service.GetAsync(HnSource.Top, pageCount, takePerPage, minPoints);
var newest = await service.GetAsync(HnSource.Newest, pageCount: 1, takePerPage: 30, minPoints: 0);

var options = new JsonSerializerOptions { WriteIndented = true };

Console.WriteLine($"TOP count: {top.Count}");
Console.WriteLine(JsonSerializer.Serialize(top, options));
Console.WriteLine();

Console.WriteLine($"NEWEST count: {newest.Count}");
Console.WriteLine(JsonSerializer.Serialize(newest, options));
