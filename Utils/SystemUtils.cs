namespace AVozyakov
{
    public static class SystemUtils
    {
        public const string HandlerName = "AVozyakov";
        public const string FolderGoogle = "AVozyakov_Google";
        public const string FileGoogle = "ExportToGoogle.exe";

        public static string FolderHandles { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"TSLab\TSLab 2.0\Handlers");
        
        private static readonly TimeZoneInfo _tmzMsc = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        /// <summary>
        /// Московское время
        /// </summary>
        public static DateTime GetTimeMsc()
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, _tmzMsc);
        }

        /// <summary>
        /// Запустить exe файл
        /// </summary>
        public static void RunProcess(string fileName, string args, out string output, out string error)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            output = process.StandardOutput.ReadToEnd();
            error = process.StandardError.ReadToEnd();
            process.WaitForExit();
        }

        /// <summary>
        /// Проверка на частое срабатываение
        /// </summary>
        public static bool CanExecuteByTime(string variableId, int seconds)
        {
            _prevExecutedDate.TryGetValue(variableId, out var dateTime);
            if (DateTime.Now < dateTime.AddSeconds(seconds))
                return false;
            _prevExecutedDate[variableId] = DateTime.Now;
            return true;
        }
        private static readonly ConcurrentDictionary<string, DateTime> _prevExecutedDate = new();
    }
}
