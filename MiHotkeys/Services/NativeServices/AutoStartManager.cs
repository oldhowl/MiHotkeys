using System.Diagnostics;
using Microsoft.Win32;

namespace MiHotkeys.Services.NativeServices
{
    public static class AutoStartManager
    {
        private static readonly string  AppName         = "MiHotkeys";
        private static readonly string? ExecutablePath  = Process.GetCurrentProcess().MainModule?.FileName;
        private const           string  RegistryRunPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void AddToStartup()
        {
            using var key = OpenRegistryKey(true);
            key?.SetValue(AppName, $"\"{ExecutablePath}\"");
        }

        public static void RemoveFromStartup()
        {
            using var key = OpenRegistryKey(true);
            key?.DeleteValue(AppName, false);
        }

        public static bool IsInStartup()
        {
            using var key = OpenRegistryKey(false);
            return key?.GetValue(AppName) != null;
        }

        private static RegistryKey? OpenRegistryKey(bool writable)
        {
            return Registry.CurrentUser.OpenSubKey(RegistryRunPath, writable);
        }
    }
}