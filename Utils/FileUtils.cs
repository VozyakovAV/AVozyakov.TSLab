namespace AVozyakov
{
    public static class FileUtils
    {
        /// <summary>Создать папку если ее нет</summary>
        public static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        /// <summary>Создать папку если ее нет</summary>
        public static void CreateFolderForFile(string file)
        {
            var dir = Path.GetDirectoryName(file);
            CreateFolder(dir);
        }

        /// <summary>Удалить папку если она есть</summary>
        public static void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }

        /// <summary>Удалить, затем создать папку</summary>
        public static void ReplaceFolder(string folder)
        {
            DeleteFolder(folder);
            CreateFolder(folder);
        }

        /// <summary>
        /// Очистить папку
        /// </summary>
        public static void ClearFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder);
                foreach (var file in files)
                    File.Delete(file);
            }
        }

        /// <summary>
        /// Запись в файл.
        /// Если нет папки, то создаст.
        /// </summary>
        public static void WriteAllText(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
            }
            catch (DirectoryNotFoundException)
            {
                CreateFolderForFile(path);
                File.WriteAllText(path, contents);
            }
        }

        /// <summary>
        /// Запись в файл.
        /// Если нет папки, то создаст.
        /// </summary>
        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            try
            {
                File.WriteAllText(path, contents, encoding);
            }
            catch (DirectoryNotFoundException)
            {
                CreateFolderForFile(path);
                File.WriteAllText(path, contents, encoding);
            }
        }
    }
}
