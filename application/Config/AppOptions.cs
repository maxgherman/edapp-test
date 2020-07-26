namespace EdAppTest.Constants
{
    public class AppOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public int TokenExpirationMinutes { get; set; }

        public int RefreshTokenExpirationMinutes { get; set; }
    }
}