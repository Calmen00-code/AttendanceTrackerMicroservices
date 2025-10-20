namespace AttendanceTrackerMicroservices.Utility
{
    public class SD
    {
        public static string AuthAPIBase { get; set; }

        public const string TOKEN_COOKIE = "JWT_TOKEN";
        public const string ROLE_CUSTOMER = "CUSTOMER";
        public const string ROLE_ADMIN = "ADMIN";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
