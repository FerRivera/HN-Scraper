using HN.Scraper.Core.Services;
using HN.Scraper.Core.Scraping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddTransient<HnScraper>();
builder.Services.AddTransient<HnService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ping", () => Results.Ok(new { message = "pong" }));

app.MapGet("/hn", async (string? source, int? pageCount, int? minPoints, HnService service) =>
{
    var src = (source ?? "top").ToLowerInvariant();
    var pages = pageCount is null or < 1 ? 1 : pageCount.Value;
    var min = minPoints is null or < 0 ? 0 : minPoints.Value;
    var hnSource = src == "newest" ? HnSource.Newest : HnSource.Top;

    var items = await service.GetAsync(
        source: hnSource,
        pageCount: pages,
        takePerPage: 30,
        minPoints: min
    );

    return Results.Ok(items);
})
.WithName("GetHackerNews")
.WithOpenApi();

app.Run();