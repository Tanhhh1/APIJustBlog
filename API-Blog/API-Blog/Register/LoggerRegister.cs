using Serilog;

namespace API.Register
{
    public static class LoggerRegister
    {
        public static void RegisterLoggerServices(this ILoggingBuilder loggingBuilder, IConfiguration Configuration)
        {
            const string logFolderName = "Logs";
            if (!Directory.Exists(logFolderName))
            {
                Directory.CreateDirectory(logFolderName);
            }

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger);
        }
    }
}
