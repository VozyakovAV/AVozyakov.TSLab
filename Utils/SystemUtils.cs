namespace AVozyakov
{
    public static class SystemUtils
    {
        public const string Handler = "AVozyakov";
        public const string FolderGoogle = "AVozyakov_Google";
        public const string FileGoogle = "ExportToGoogle.exe";

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
    }
}
