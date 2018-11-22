using CommandLineParser.Arguments;

namespace WireMock.Net.StandAlone
{
    public class CommandLineArguments
    {
        [ValueArgument(typeof(bool), "StartAdminInterface", DefaultValue = true, Description = "Start the AdminInterface")]
        public bool StartAdminInterface { get; set; }

        [ValueArgument(typeof(bool), "ReadStaticMappings", DefaultValue = false, Description = "Read StaticMappings")]
        public bool ReadStaticMappings { get; set; }

        [ValueArgument(typeof(bool), 'w', "WatchStaticMappings", DefaultValue = false, Description = "Watch the static mapping files + folder for changes when running.")]
        public bool WatchStaticMappings { get; set; }

        [ValueArgument(typeof(bool), 'm', "AllowPartialMapping", DefaultValue = false, Description = "Allow PartialMapping")]
        public bool AllowPartialMapping { get; set; }

        [ValueArgument(typeof(string), 'u', "AdminUsername", Description = "The username needed for __admin access.")]
        public string AdminUsername { get; set; }

        [ValueArgument(typeof(string), 'p', "AdminPassword", Description = "The password needed for __admin access.")]
        public string AdminPassword { get; set; }


        [BoundedValueArgument(typeof(int), 'o', "MaxRequestLogCount", MinValue = 0, Description = "The Maximum number of RequestLogs to keep")]
        public int MaxRequestLogCount { get; set; }

        [BoundedValueArgument(typeof(int), 'x', "RequestLogExpirationDuration", MinValue = 0, Description = "The RequestLog Expiration Duration in hours")]
        public int RequestLogExpirationDuration { get; set; }
    }
}
