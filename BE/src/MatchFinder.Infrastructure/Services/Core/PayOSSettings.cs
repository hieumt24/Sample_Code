namespace MatchFinder.Infrastructure.Services.Core
{
    public class PayOSSettings
    {
        public static string ConfigName => "PayOS";
        public string ClientId { get; set; } = String.Empty;
        public string ApiKey { get; set; } = String.Empty;
        public string ChecksumKey { get; set; } = String.Empty;
        public string CancelUrl { get; set; } = String.Empty;
        public string ReturnUrl { get; set; } = String.Empty;
    }
}