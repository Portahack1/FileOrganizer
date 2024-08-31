namespace FileOrganizer
{
    public class Logger
    {
        private static readonly Logger instance = new();
        private readonly string LogFilePath = "log";

        public static Logger Instance { get { return instance; } }

        public void Log(string message)
        {
            DateTime currentDate = DateTime.Now;
            string todayLog = $"{LogFilePath}_{currentDate.Year}{currentDate.Month}{currentDate.Day}.log";
            if (!File.Exists(todayLog))
            {
                using FileStream fs = File.Create(todayLog);
            }

            using StreamWriter sw = new StreamWriter(todayLog, true);
            sw.WriteLine($"\n\n{DateTime.Now:yyyy-MM-dd HH:mm:ss} \n{message}");
        }
    }
}
