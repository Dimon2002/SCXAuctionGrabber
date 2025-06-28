using SCXAuctionGrabber.Domain.DataStructures;
using SCXAuctionGrabber.IO.Interfaces;

namespace SCXAuctionGrabber.IO.Readers;

public class ItemParamsReader : IReader
{
    public IEnumerable<ItemRequest> Parse(string path)
    {
        using var reader = new StreamReader(path);
        ItemCategory? currentCategory = null;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var cleanLine = line.Split('#')[0].Trim();
            if (string.IsNullOrEmpty(cleanLine))
            {
                continue;
            }

            if (cleanLine.Contains(':') || cleanLine.EndsWith(':'))
            {
                var colonIndex = cleanLine.IndexOf(':');
                var categoryPart = colonIndex >= 0
                    ? cleanLine[..colonIndex].Trim()
                    : cleanLine.TrimEnd(':').Trim();

                if (Enum.TryParse(categoryPart, true, out ItemCategory category))
                {
                    currentCategory = category;

                    if (colonIndex >= 0 && colonIndex < cleanLine.Length - 1)
                    {
                        var idsPart = cleanLine[(colonIndex + 1)..].Trim();
                        foreach (var item in ProcessIds(idsPart, category))
                        {
                            yield return item;
                        }
                    }

                    continue;
                }

                currentCategory = null;
                continue;
            }

            if (currentCategory.HasValue)
            {
                foreach (var item in ProcessIds(cleanLine, currentCategory.Value))
                {
                    yield return item;
                }
            }
        }
    }

    private static IEnumerable<ItemRequest> ProcessIds(string input, ItemCategory category)
    {
        var items = input.Split([','], StringSplitOptions.RemoveEmptyEntries);

        foreach (var item in items)
        {
            var id = item.Trim();
            if (!string.IsNullOrEmpty(id))
            {
                yield return new ItemRequest
                {
                    Category = category,
                    Id = id
                };
            }
        }
    }
}
