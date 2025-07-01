using System.ComponentModel.DataAnnotations;

namespace SCXAuctionGrabber.AuctionGrabberService;

public class AppSettings
{
    [Required(ErrorMessage = "BasePath is required")]
    public string BasePath { get; set; } = string.Empty;

    [Required(ErrorMessage = "InputFolder is required")]
    public string InputFolder { get; set; } = string.Empty;

    [Required(ErrorMessage = "OutputFolder is required")]
    public string OutputFolder { get; set; } = string.Empty;

    [Required(ErrorMessage = "RecommendedPriceFile is required")]
    public string RecommendedPriceFile { get; set; } = string.Empty;

    [Range(1, 1440, ErrorMessage = "IntervalMinutes must be between 1 and 1440 (24 hours)")]
    public int IntervalMinutes { get; set; } = 60;
}