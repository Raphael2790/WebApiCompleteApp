namespace RSS.WebApi.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int ExpirationMinutes { get; set; }
        public string EmittedBy { get; set; }
        public string ValidOn { get; set; }
    }
}
