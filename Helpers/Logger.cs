using System;
using System.IO;
using System.Text;
using System.Web;

namespace WebAPI.Helpers
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void LogError(Exception ex, string context)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("==================================================");
                sb.AppendLine("Timestamp : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine("Level     : ERROR");
                sb.AppendLine("Context   : " + (context == null ? "" : context));
                sb.AppendLine("Message   : " + ex.Message);
                sb.AppendLine("Type      : " + ex.GetType().FullName);
                sb.AppendLine("StackTrace: " + ex.StackTrace);

                if (ex.InnerException != null)
                {
                    sb.AppendLine("Inner Msg : " + ex.InnerException.Message);
                    sb.AppendLine("Inner Type: " + ex.InnerException.GetType().FullName);
                }

                Write(sb.ToString());
            }
            catch
            {
                // Logging must never throw and break the request.
                // If the log write fails, we swallow it deliberately.
            }
        }

        public static void LogInfo(string message, string context)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Timestamp : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine("Level     : INFO");
                sb.AppendLine("Context   : " + (context == null ? "" : context));
                sb.AppendLine("Message   : " + message);
                sb.AppendLine("--------------------------------------------------");
                Write(sb.ToString());
            }
            catch { }
        }

        private static void Write(string content)
        {
            string folder = GetLogFolder();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string file = Path.Combine(folder,
                "log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            lock (_lock)
            {
                File.AppendAllText(file, content + Environment.NewLine);
            }
        }

        private static string GetLogFolder()
        {
            // Resolves to the app's /Logs folder when running under IIS.
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath("~/Logs");
            }
            // Fallback for non-web contexts.
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        }
    }
}