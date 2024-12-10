using Extension;

namespace Global
{
    public sealed class GlobalConfig
    {
        public static GlobalConfig Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<GlobalConfig> lazy = new Lazy<GlobalConfig>();

        private readonly string env;
        private readonly string _appName;
        private readonly string _appVersion;
        private readonly SSOConfig _ssoConfig;
		private readonly SSOCredential _userClient;
		private readonly SSOCredential _serviceClient;
        private readonly string _sqliteDatabaseName;
        private readonly string _sqliteConStr;
        private readonly string _logLevel;


        public GlobalConfig()
        {
         
            var builder = new ConfigurationBuilder();
           
            var appsettingFilename = "appsettings.json";
            builder.AddJsonFile(appsettingFilename, optional: false, reloadOnChange: true);

            // Build the configuration object
            var configuration = builder.Build();
            env = configuration.GetValue("AppConfig:Env", "uat");
            _sqliteDatabaseName = configuration.GetValue("AppConfig:DBFileName", "olif.sqlite");
            _sqliteConStr = $"Data Source={AppContext.BaseDirectory}{_sqliteDatabaseName};Version=3;";
            _logLevel = configuration.GetValue("AppConfig:LogLevel", "debug");
            
          
        }

        public SSOConfig SSOConfig { get { return _ssoConfig; } }
        public string Environment { get { return env; } }
        public string AppName { get { return _appName; } }
        public string AppVersion { get { return _appVersion; } }
        public SSOCredential UserClient { get { return _userClient; } }
        public SSOCredential ServiceClient { get { return _serviceClient; } }
        public string SqliteConnectionString { get { return _sqliteConStr; } }
        public string SqliteDatabaseName { get { return _sqliteDatabaseName; } }

        public string LogLevel => _logLevel;

    }


    public struct SSOConfig : ISSOConfig
    {
        public string Http { get; set; }
        public string AbsUrl { get; set; }
        public string Auth { get; set; }
        public string Introspect { get; set; }
        public string HealthCheck { get; set; }
        public string Realm { get; set; }

	}

    public struct SSOCredential
	{
        public string Client_id { get; set; }
        public string Client_secret { get; set; }
	}
}
