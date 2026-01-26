# Hacker News Scraper (AngleSharp) — .NET Console App

A small, GitHub-ready .NET project that scrapes Hacker News (Top or Newest), supports pagination, applies basic filtering/sorting policies, and prints **deterministic JSON** to stdout.

This repo was built as preparation for common “scraping + refactor” interview challenges (e.g., BairesDev-style exercises).

---

## Features

- Scrapes **Title, Author, Points**
- Also extracts:
  - **CommentsCount**
  - **Age** (as displayed by HN)
  - **Page** (for pagination)
- Supports **Top** and **Newest**
- Supports **pagination** (e.g., first 2 pages)
- Filtering/sorting policy (e.g., **minPoints >= 100**, sort by points desc)
- Output as **JSON** using `System.Text.Json`
- Cleaned-up helpers for:
  - Safe text extraction
  - Digit parsing (`int.TryParse`)
  - Building absolute URLs from the `More` link

---

## Tech Stack

- .NET (Console App)
- [AngleSharp](https://anglesharp.github.io/) for HTML parsing
- `HttpClient` reused (recommended: injected into scraper/service)
- `System.Text.Json` for deterministic output

---

## Project Structure

```text
HN.Scraper.sln
README.md
src/
  HN.Scraper.Cli/
    HN.Scraper.Cli.csproj
    Program.cs
    Common/
      Helpers.cs
      HnUrls.cs
    Models/
      HnItem.cs
    Scraping/
      HnScraper.cs
    Services/
      HnService.cs

Notes:
- `HnScraper` is responsible for fetching/parsing pages and pagination.
- `HnService` is responsible for “policy” (source selection + filtering + sorting).
- `Program.cs` stays thin and prints JSON.

---

## How It Works (High Level)

1. Fetch the HTML for the selected source (Top/Newest).
2. Use `tr.athing` as the anchor row (one per item).
3. Use `NextElementSibling` to reach the metadata `<tr>`:
   - points from `.score`
   - author from `a.hnuser`
4. Extract age from `span.age a`
5. Extract comments by finding the correct `<a>` in the subtext row:
   - `"discuss"` => `0`
   - `"123 comments"` => `123`
6. For pagination, find:
   - `a.morelink` → `href`
   - Build absolute URL with `new Uri(new Uri(baseUrl), href)`
7. Append results across pages; set `Page` field.

---

## Running

From the project folder:

```bash
dotnet restore
dotnet build -c Release
dotnet run -c Release

---

JSON Output Example (Truncated)

[
  {
    "title": "Example title",
    "url": "https://example.com",
    "rank": 1,
    "itemId": 12345678,
    "author": "someone",
    "points": 245,
    "commentsCount": 58,
    "age": "3 hours ago",
    "page": 1
  }
]
