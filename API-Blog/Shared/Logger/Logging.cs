using Serilog;

namespace Shared.Logger
{
    public static class Logging
    {
        public static void Info(string message, params object[] args)
            => Log.Information(message, args);

        public static void Debug(string message, params object[] args)
            => Log.Debug(message, args);

        public static void Warning(string message, params object[] args)
            => Log.Warning(message, args);

        public static void Error(string message, params object[] args)
            => Log.Error(message, args);
        public static void Error(Exception ex, string message = "An error occurred", params object[] args)
            => Log.Error(ex, message, args);
    }
}
