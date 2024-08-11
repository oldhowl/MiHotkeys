using System.Diagnostics;
using System.Reflection;

namespace MiHotkeys.Services
{
    public static class AutoStartManager
    {
        private static readonly string StartupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        public static bool IsInStartup(string shortcutName)
        {
            var shortcutPath = Path.Combine(StartupFolder, shortcutName);
            return File.Exists(shortcutPath);
        }

        public static void AddToStartup(string shortcutName)
        {
            var shortcutPath = Path.Combine(StartupFolder, shortcutName);
            var targetPath   = Assembly.GetEntryAssembly()?.Location;

            if (targetPath != null)
            {
                var shortcutContent = $@"[InternetShortcut]
URL=file:///{targetPath}
IconIndex=0
IconFile={targetPath}";

                File.WriteAllText(shortcutPath, shortcutContent);

                Process.Start(new ProcessStartInfo
                {
                    FileName       = "cmd.exe",
                    Arguments      = $"/C attrib +h \"{shortcutPath}\"", // Set the shortcut file as hidden
                    CreateNoWindow = true,
                    WindowStyle    = ProcessWindowStyle.Hidden
                })?.WaitForExit();
            }
        }

        public static void RemoveFromStartup(string shortcutName)
        {
            var shortcutPath = Path.Combine(StartupFolder, shortcutName);
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }
        }
    }
}