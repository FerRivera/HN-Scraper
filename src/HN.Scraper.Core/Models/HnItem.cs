using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HN.Scraper.Core.Models
{
    public sealed record HnItem(
    string Title,
    string Url,
    int Rank,
    string ItemId,
    string Author = "",
    int Points = 0,
    int CommentsCount = 0,
    string Age = "",
    int Page = 1
);
}
