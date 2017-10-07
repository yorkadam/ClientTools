using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace ClientTools
{
    class Program
    {
        private static HttpClient Client = new HttpClient();
        public static string ApplicationEnvrionment { get; set; }
        public static ILogger<Program> PlainLogger { get; set; }
        public static LogProviders.IFileLogService FileLogger { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static DocumentComponents Components { get; set; }


        /*
        To run any of these examples you must change the code (and configuration) to meet your specific use-case / conditions.
        Here are the key points you will need to change.
            0. Change application configuation settings (in appsettings.json files)
            1. Change the "example number" to call the expected method.
            2. Set the environment to DEV or PROD (DO NOT POINT AT YOUR PROD ENVRIONMENT UNLESS YOU KNOW WHAT YOU ARE DOING!!!)
            3. Change method parameters to suit your needs
            4. Change method parameter values as expected
            5. Change data method(s) SQL query strings, columns etc ... to match your database
        */

        static void Main(string[] args) // Note we are not using args at all but left here as a point of reference
        {
            foreach (string argument in args) { }

            SetEnvironmentConfiguration("DEV");  // Values: DEV or PROD (see code below)

            // Example To Run
            int exampleNumber = 1;
            switch (exampleNumber)
            {
                case 1:
                    ExtractLinksAndPageText("http://www.somesite.com");
                    Finish();
                    break;
                case 2:
                    HttpGetExample("http://www.somesite.com", "http://www.somesite.com", "https://www.somesite.com");
                    // uri1 = any uri that is not expecting parameters
                    // uri2 = any uri that EXPECTS parameters where parameters are set in the example method
                    // uri3 = same as 2 but ignoring certificates
                    // Note: Apply HTTPS vs HTTPS as expected. Meaning, certificate testing should use HTTPS in uri.
                    break;
                case 3:
                    HttpPostExample("https://www.somesite.com","https://www.somesite.com");
                    // uri1 = any uri that default certificate check is expected
                    // uri2 = any uri where certificate checking is expected to be ignored (should be HTTPS)
                    // Note: Apply HTTPS vs HTTPS as expected. Meaning, certificate testing should use HTTPS in uri.
                    break;
                case 4:
                    GetRecordsPostgreSql();
                    break;
                case 5:
                    GetRecordsSqlServer();
                    break;
                case 6:
                    GetRecordsSQLite();
                    break;
                case 7:
                    ReadEnvironmentVariables();
                    break;
                case 8:
                    WriteToLogs();
                    break;
            }
        }

        private static void Finish()
        {
            Console.WriteLine("press any key to exit");
            Console.Read();
            return;
        }

        private static void ExtractLinksAndPageText(string uri)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> mediaHeaders = new List<string>();
            HtmlDocument document = new HtmlDocument();
            Uri pageUri = new Uri(uri);
            string documentText = string.Empty;

            Console.WriteLine("Getting page links and text.");

            headers.Add("User-Agent", Components.UserAgentHeader);
            mediaHeaders.Add("html/text");

            // This setting causes the form tags to be removed. See HtmlAgilityPack documents for more details.
            HtmlNode.ElementsFlags.Remove("form");

            // HttpResponseMessage response = Getter.Get(pageUri, headers, mediaHeaders, true).Content.ReadAsStringAsync().Result;
            HttpResponseMessage response = Getter.Get(pageUri, headers, mediaHeaders, true);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<Uri> links = new List<Uri>();
                document.LoadHtml(response.Content.ReadAsStringAsync().Result);
                links = UriExtractor.GetLinks(document, pageUri, enums.LinkLocation.Internal);
                documentText = HtmlContentExtractor.GetDocumentText(document, Components);

                foreach (Uri link in links)
                {
                    Console.WriteLine(link.AbsoluteUri);
                }

                Console.WriteLine(" ");
                Console.Write(documentText);
            }
        }

        private static void HttpGetExample(string uri1, string uri2, string uri3)
        {
            List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> paramFirstName = new KeyValuePair<string, string>("firstName", "Joe");
            KeyValuePair<string, string> paramLastName = new KeyValuePair<string, string>("LastName", "Smith");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> mediaHeaders = new List<string>();

            queryParams.Add(paramFirstName);
            queryParams.Add(paramLastName);
            headers.Add("User-Agent", Components.UserAgentHeader);
            mediaHeaders.Add("html/text");

            // Getter example without query string parameters
            var response_WITHOUT_params = Getter.Get(new Uri(uri1), headers, mediaHeaders);
            var resultContent1 = response_WITHOUT_params.Content.ReadAsStringAsync().Result;

            // Getter example WITH query string parameters
            var response_WITH_params = Getter.Get(new Uri(uri2), headers, mediaHeaders, queryParams);
            var resultContent2 = response_WITH_params.Content.ReadAsStringAsync().Result;

            // Getter example of ignoring invalid certificates (Assumes you are pointing to a site with self signed or invalid certificates)
            var response_IGNORE_Cert = Getter.Get(new Uri(uri3), headers, mediaHeaders, true, queryParams);
            var resultContent3 = response_IGNORE_Cert.Content.ReadAsStringAsync().Result;
        }

        private static void HttpPostExample(string uri1, string uri2)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            List<string> mediaHeaders = new List<string>();
            List<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> fieldName = new KeyValuePair<string, string>("name", "joesmith");
            KeyValuePair<string, string> fieldPassword = new KeyValuePair<string, string>("password", "apassword");

            headers.Add("User-Agent", Components.UserAgentHeader);
            mediaHeaders.Add("html/text");

            fields.Add(fieldName);
            fields.Add(fieldPassword);

            // Get a response that defaults to always check certificate
            var responseDefaultCert = Poster.Post(fields, new Uri(uri1), headers, mediaHeaders);
            var resultContent1 = responseDefaultCert.Content.ReadAsStringAsync().Result;

            // Get a repsonse that ignores certificates (Assumes you are pointing to a site with self signed or invalid certificates)
            var responseIgnoreCert = Poster.Post(fields, new Uri(uri2), true, headers, mediaHeaders);
            var resultContent2 = responseIgnoreCert.Content.ReadAsStringAsync().Result;
        }

        private static void GetRecordsPostgreSql()
        {
            /*
            This example assumes you have a SQL table named "exclusions" with
            a column named "urldomain" containing domains to exclude from URL processing.
             
            Important Points
                With Postgres the parameter name must match (exactly) the table column name
                The "Like" clause requires percent (%) symbols to be passed as part of the value
                to work correctly. This is specific to Postgres.
                Semi-Colon placement matters in statements.
             */

            string queryStatement = @"Select urldomain from exclusions WHERE urldomain like @urldomain ;";
            SqlQuery sqlQuery = new SqlQuery();
            sqlQuery.Statement = queryStatement;
            sqlQuery.Parameters.Add("urldomain", "%cnn%");
            var data = Data.PostgreSQLEngine.GetData(sqlQuery);

            foreach (Dictionary<string, string> record in data)
            {
                // get field values directly
                var urldomain = record["urldomain"];

                // Iterate over fields in record instance to get values
                foreach (KeyValuePair<string, string> fields in record)
                {
                    var fieldName = fields.Key;
                    var fieldValue = fields.Value;
                }
            }
        }

        private static void GetRecordsSqlServer()
        {
            /*
             This example assumes you will repalce table name, column(s) name, parameter name which represent
             objects in your database.
             */

            string queryStatement = @"Select top 10 * from TABLENAME WHERE ColumnName like '%'+@ParameterName+'%';";
            SqlQuery sqlQuery = new SqlQuery();
            sqlQuery.Statement = queryStatement;
            sqlQuery.Parameters.Add("ParameterName", "parameterValue");

            var data = Data.SqlServerEngine.GetData(sqlQuery);

            foreach (Dictionary<string, string> record in data)
            {
                // get field values directly
                var fieldValue1 = record["ColumnName1"];
                var fieldValue2 = record["ColumnName2"];
                var fieldValue3 = record["ColumnName3"];

                // Iterate over fields in record instance to get values
                foreach (KeyValuePair<string, string> fields in record)
                {
                    var fieldName = fields.Key;
                    var fieldValue = fields.Value;
                }
            }
        }

        private static void GetRecordsSQLite()
        {
            /*
                This example assumes you have a SQL table named "contacts" which contains
                columns named: name, phone, email, ip_address

                Important Notes:
                    Parameters not needed if not using a where clause with parameterized values.
                    LIKE clauses require operators passed in as value because '%' + @var + '%' does not work like it does wih MSFT SQL Server
            */

            string queryStatement = @"select name, phone, email, ip_address from contacts where name like @name;";
            SqlQuery sqlQuery = new SqlQuery();
            sqlQuery.Statement = queryStatement;
            sqlQuery.Parameters.Add("@name", "%someonesname%");

            var data = Data.SQLiteEngine.GetData(sqlQuery);

            foreach (Dictionary<string, string> record in data)
            {
                // get field values directly
                var name = record["name"];
                var phone = record["phone"];
                var email = record["email"];

                // Iterate over fields in record instance to get values
                foreach (KeyValuePair<string, string> fields in record)
                {
                    var fieldName = fields.Key;
                    var fieldValue = fields.Value;
                }
            }
        }

        private static void ReadEnvironmentVariables()
        {
            // NOTE: Env Vars are named differently on Linux / mac vs Windows
            var environmentVars = Environment.GetEnvironmentVariables();
            var localAppData = environmentVars["LOCALAPPDATA"];
            var appData = environmentVars["APPDATA"];
            var os = environmentVars["OS"];
            var userName = environmentVars["USERNAME"];
            var homePath = environmentVars["HOMEPATH"];
            var userProfile = environmentVars["USERPROFILE"];
            var homeDrive = environmentVars["HOMEDRIVE"];
            var computerName = environmentVars["COMPUTERNAME"];
            var sessionName = environmentVars["SESSIONNAME"];
            var userDomainRoamingProfile = environmentVars["USERDOMAIN_ROAMINGPROFILE"];
            var pathExt = environmentVars["PATHEXT"];
            var allUsersProfile = environmentVars["ALLUSERSPROFILE"];
            var logonServer = environmentVars["LOGONSERVER"];
        }

        private static void WriteToLogs()
        {
            PlainLogger.LogInformation("Hello World from plain logger");
            PlainLogger.LogCritical("Critical Hello World");
            PlainLogger.LogDebug("DEBUG level hello world");
            PlainLogger.LogTrace("Trace Log Level hello world");
            PlainLogger.LogWarning("Warning Log Level Hello World");

            FileLogger.LogInformation("Information Log Here");
            FileLogger.LogTrace("Trace information here");
            FileLogger.LogWarning("Warning information here");
            FileLogger.LogDebug("Debug infomation here");
            FileLogger.LogError("Error information here");
            FileLogger.LogCritical("A Test message from file");
        }

        private static void SetEnvironmentConfiguration(string environment)
        {
            /*
             Notes: The documentation is not clear if "reloadOnChange" works with console
             applications like it does for web applications. I did not test it yet.
             */
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            switch (environment)
            {
                case "DEV":
                    ApplicationEnvrionment = "DEV";
                    builder.AddJsonFile("appsettings-dev.json", optional: false, reloadOnChange: true);
                    break;
                case "PROD":
                    ApplicationEnvrionment = "PROD";
                    builder.AddJsonFile("appsettings-prod.json", optional: false, reloadOnChange: true);
                    break;
            };
            Configuration = builder.Build();
            Components = new DocumentComponents(Configuration);
            ConfigureDatabaseSettings(Configuration.GetSection("ApplicationSettings:DefaultDatabaseEngine").Value);
            ConfigureLogging();
        }

        private static void ConfigureDatabaseSettings(string databaseEngine)
        {
            var pgsql = Configuration.GetSection("DatabaseSettings:PostgreSQL:HOST").Value;

            switch (databaseEngine)
            {
                case "PostgreSQLEngine":
                    Data.PostgreSQLEngine.DatabaseSettings.Host = Configuration.GetSection("DatabaseSettings:PostgreSQL:HOST").Value;
                    Data.PostgreSQLEngine.DatabaseSettings.UserName = Configuration.GetSection("DatabaseSettings:PostgreSQL:UserName").Value;
                    Data.PostgreSQLEngine.DatabaseSettings.Port = Convert.ToInt32(Configuration.GetSection("DatabaseSettings:PostgreSQL:Port").Value);
                    Data.PostgreSQLEngine.DatabaseSettings.DatabaseName = Configuration.GetSection("DatabaseSettings:PostgreSQL:DatabaseName").Value;
                    Data.PostgreSQLEngine.DatabaseSettings.Password = Configuration.GetSection("DatabaseSettings:PostgreSQL:Password").Value;
                    Data.PostgreSQLEngine.DatabaseSettings.PersistSecuritInfo = Convert.ToBoolean(Configuration.GetSection("DatabaseSettings:PostgreSQL:PersistSecuritInfo").Value);
                    Data.PostgreSQLEngine.DatabaseSettings.IntegratedSecurity = Convert.ToBoolean(Configuration.GetSection("DatabaseSettings:PostgreSQL:IntegratedSecurity").Value);
                    Data.PostgreSQLEngine.DatabaseSettings.SearchPath = Configuration.GetSection("DatabaseSettings:PostgreSQL:SearchPath").Value;
                    Data.PostgreSQLEngine.DatabaseSettings.TimeOut = Convert.ToInt32(Configuration.GetSection("DatabaseSettings:PostgreSQL:TimeOut").Value);
                    Data.PostgreSQLEngine.DatabaseSettings.ApplicationName = Configuration.GetSection("DatabaseSettings:PostgreSQL:ApplicationName").Value;
                    Data.PostgreSQLEngine.DatabaseSettings.TrustCertificate = Convert.ToBoolean(Configuration.GetSection("DatabaseSettings:PostgreSQL:TrustCertificate").Value);
                    Data.PostgreSQLEngine.DatabaseSettings.SSLMode = Convert.ToInt32(Configuration.GetSection("DatabaseSettings:PostgreSQL:SSLMode").Value);
                    break;
                case "SqlServerEngine":
                    Data.SqlServerEngine.DatabaseSettings.DataSource = Configuration.GetSection("DatabaseSettings:SqlServer:DataSource").Value;
                    Data.SqlServerEngine.DatabaseSettings.InitialCatalog = Configuration.GetSection("DatabaseSettings:SqlServer:InitialCatalog").Value;
                    Data.SqlServerEngine.DatabaseSettings.IntegratedSecurity = Convert.ToBoolean(Configuration.GetSection("DatabaseSettings:SqlServer:IntegratedSecurity").Value);
                    Data.SqlServerEngine.DatabaseSettings.UserId = Configuration.GetSection("DatabaseSettings:SqlServer:UserID").Value;
                    Data.SqlServerEngine.DatabaseSettings.Password = Configuration.GetSection("DatabaseSettings:SqlServer:Password").Value;
                    Data.SqlServerEngine.DatabaseSettings.ApplicationName = Configuration.GetSection("DatabaseSettings:SqlServer:ApplicationName").Value;
                    Data.SqlServerEngine.DatabaseSettings.TimeOut = Convert.ToInt32(Configuration.GetSection("DatabaseSettings:SqlServer:TimeOut").Value);
                    Data.SqlServerEngine.DatabaseSettings.ConnectionPooling = Convert.ToBoolean(Configuration.GetSection("DatabaseSettings:SqlServer:ConnectionPooling").Value);
                    break;
                case "SQLiteEngine":
                    Data.SQLiteEngine.DatabaseSettings.DatabaseFileName = Configuration.GetSection("DatabaseSettings:SQLite:SQLiteFileName").Value;
                    Data.SQLiteEngine.DatabaseSettings.SQLitePath = Path.Combine(Directory.GetCurrentDirectory(), Configuration.GetSection("DatabaseSettings:SQLite:SQLitePath").Value, Configuration.GetSection("DatabaseSettings:SQLite:SQLiteFileName").Value);
                    break;
            }
        }

        private static void ConfigureLogging()
        {
            Dictionary<string, string> logPaths = new Dictionary<string, string>();
            logPaths.Add("Log.Critical",
                Path.Combine(Directory.GetCurrentDirectory(),
                Configuration.GetSection("ApplicationSettings:LogFileFolder").Value,
                Configuration.GetSection("ApplicationSettings:Log.Critical").Value));
            logPaths.Add("Log.Debug",
                Path.Combine(Directory.GetCurrentDirectory(),
                Configuration.GetSection("ApplicationSettings:LogFileFolder").Value,
                Configuration.GetSection("ApplicationSettings:Log.Debug").Value));
            logPaths.Add("Log.Error",
                Path.Combine(Directory.GetCurrentDirectory(),
                Configuration.GetSection("ApplicationSettings:LogFileFolder").Value,
                Configuration.GetSection("ApplicationSettings:Log.Error").Value));
            logPaths.Add("Log.Information",
                Path.Combine(Directory.GetCurrentDirectory(),
                Configuration.GetSection("ApplicationSettings:LogFileFolder").Value,
                Configuration.GetSection("ApplicationSettings:Log.Information").Value));
            logPaths.Add("Log.Trace",
                Path.Combine(Directory.GetCurrentDirectory(),
                Configuration.GetSection("ApplicationSettings:LogFileFolder").Value,
                Configuration.GetSection("ApplicationSettings:Log.Trace").Value));
            logPaths.Add("Log.Warning",
                Path.Combine(Directory.GetCurrentDirectory(),
                Configuration.GetSection("ApplicationSettings:LogFileFolder").Value,
                Configuration.GetSection("ApplicationSettings:Log.Warning").Value));
            bool loggingIsEnabled = Convert.ToBoolean(Configuration.GetSection("ApplicationSettings:Logging.Enabled").Value);

            var serviceCollection = new ServiceCollection()
            .AddLogging()
            .AddSingleton<LogProviders.IFileLogService, LogProviders.FileLogService>()
            .BuildServiceProvider();
            serviceCollection.GetService<ILoggerFactory>().AddConsole(LogLevel.Debug);
            PlainLogger = serviceCollection.GetService<ILoggerFactory>().CreateLogger<Program>();
            FileLogger = serviceCollection.GetRequiredService<LogProviders.IFileLogService>();
            FileLogger.LogFilePaths = logPaths;
            FileLogger.IsEndabled = loggingIsEnabled;
        }
    }
}