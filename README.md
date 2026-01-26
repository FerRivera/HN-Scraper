# Hacker News Scraper (AngleSharp) — .NET Console App

A small, GitHub-ready .NET project that scrapes Hacker News (Top or Newest), supports pagination, applies basic filtering/sorting policies, and prints deterministic JSON to stdout.

This repo was built as preparation for common “scraping + refactor” interview challenges (e.g., BairesDev-style exercises).

---

## Features

- Scrapes **Title**, **Author**, **Points**
- Also extracts:
  - **CommentsCount**
  - **Age** (as displayed by HN)
  - **Page** (pagination indicator)
- Supports **Top** and **Newest**
- Supports **pagination** (e.g., first 2 pages)
- Filtering/sorting policy (e.g., `minPoints >= 100`, sort by points desc)
- Output as **JSON** using `System.Text.Json`
- Helper utilities:
  - Safe text extraction
  - Digit parsing (`int.TryParse`)
  - Building absolute URLs from the “More” link

---

## Tech Stack

- .NET (Console App)
- AngleSharp for HTML parsing
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
