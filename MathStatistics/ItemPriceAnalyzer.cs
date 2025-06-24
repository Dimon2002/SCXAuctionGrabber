using SCXAuctionGrabber.Domain.Interfaces;

namespace SCXAuctionGrabber.MathStatistics;

public class ItemPriceAnalyzer
{
    public static double CalculateStrategicPrice(
    IEnumerable<IAuctionRecord>? records,
    double weightMeanCut = 0.6,
    double weightRecommend = 0.4)
    {
        var meanCut = MeanCut(records);
        var recommendPrice = RecommendedPrice(records);

        return (meanCut * weightMeanCut + recommendPrice * weightRecommend)
               / (weightMeanCut + weightRecommend);
    }

    public static double RecommendedPrice(IEnumerable<IAuctionRecord>? records)
    {
        if (records is null || !records.Any())
        {
            return double.NaN;
        }

        var prices = records.Select(r => r.Price);

        var filteredPrices = FilterOutliers(prices).ToList();

        return Quantile(filteredPrices, 0.30);
    }

    public static double MeanCut(IEnumerable<IAuctionRecord>? records, double alpha = 0.12)
    {
        if (records is null || !records.Any())
        {
            return double.NaN;
        }

        if (alpha < 0 || alpha >= 0.5)
        {
            throw new ArgumentOutOfRangeException(nameof(alpha));
        }

        var validValues = records.Select(v => v.Price).ToArray();
        Array.Sort(validValues);

        var count = validValues.Length;
        var k = int.Max(1, (int)(count * alpha));

        return validValues
            .Skip(k)
            .Take(count - 2 * k)
            .Average();
    }

    public static (double min, double max) GetPriceConfidenceInterval(IEnumerable<IAuctionRecord>? records, double confidenceLevel = 0.95)
    {
        if (records == null || !records.Any())
        {
            return (double.NegativeInfinity, double.PositiveInfinity);
        }

        var validPrices = records.Select(e => e.Price).ToArray();

        Array.Sort(validPrices);

        double lowerPercentile = (1 - confidenceLevel) / 2;
        double upperPercentile = 1 - lowerPercentile;

        return (
            Quantile([.. validPrices], lowerPercentile),
            Quantile([.. validPrices], upperPercentile)
        );
    }

    private static IEnumerable<double> FilterOutliers(IEnumerable<double> prices)
    {
        if (prices.Count() < 4)
        {
            return prices;
        }

        var sortedPrices = prices.OrderBy(p => p).ToList();

        double q1 = Quantile(sortedPrices, 0.25);
        double q3 = Quantile(sortedPrices, 0.75);
        double iqr = q3 - q1;

        double lowerBound = q1 - 1.5 * iqr;
        double upperBound = q3 + 1.5 * iqr;

        return [.. sortedPrices.Where(p => p >= lowerBound && p <= upperBound)];
    }

    private static double Quantile(List<double> sortedData, double percentile)
    {
        if (percentile < 0 || percentile > 1)
            throw new ArgumentException("Percentile must be between 0 and 1");

        int n = sortedData.Count;
        double index = percentile * (n - 1);
        int lowerIndex = (int)Math.Floor(index);
        int upperIndex = (int)Math.Ceiling(index);

        if (lowerIndex == upperIndex)
            return sortedData[lowerIndex];

        double fraction = index - lowerIndex;
        return sortedData[lowerIndex] + fraction * (sortedData[upperIndex] - sortedData[lowerIndex]);
    }
}