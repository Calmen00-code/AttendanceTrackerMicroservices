namespace AttendanceTrackerMicroservices.Utility
{
    public class SD
    {
        public static string AuthAPIBase { get; set; }

        public const string TOKEN_COOKIE = "JWT_TOKEN";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
