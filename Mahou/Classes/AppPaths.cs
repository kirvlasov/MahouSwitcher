using System;
using System.IO;

namespace Mahou
{
    internal static class AppPaths
    {
        private const string AppDataFolderName = "MahouSwitcher";

        public static readonly string LegacyDirectory = AppContext.BaseDirectory;
        public static readonly string DataDirectory = GetDataDirectory();
        public static readonly string LogDirectory = Path.Combine(DataDirectory, "logs");

        public static string ConfigFile
        {
            get { return Path.Combine(DataDirectory, "Mahou.ini"); }
        }

        public static string SnippetsFile
        {
            get { return Path.Combine(DataDirectory, "snippets.txt"); }
        }

        public static void EnsureDataDirectory()
        {
            Directory.CreateDirectory(DataDirectory);
        }

        public static void EnsureLogDirectory()
        {
            Directory.CreateDirectory(LogDirectory);
        }

        public static void MigrateLegacyFile(string fileName, string destinationPath)
        {
            EnsureDataDirectory();

            var legacyPath = Path.Combine(LegacyDirectory, fileName);
            if (!File.Exists(destinationPath) && File.Exists(legacyPath)) {
                File.Copy(legacyPath, destinationPath);
            }
        }

        private static string GetDataDirectory()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (String.IsNullOrEmpty(localAppData)) {
                return LegacyDirectory;
            }

            return Path.Combine(localAppData, AppDataFolderName);
        }
    }
}
