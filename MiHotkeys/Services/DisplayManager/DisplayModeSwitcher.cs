using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MiHotkeys.Services.DisplayManager
{
    public class DisplayModeSwitcher
    {
        private readonly RefreshRateMode[] _refreshRates =
            { RefreshRateMode.Hz60, RefreshRateMode.Hz120, RefreshRateMode.Hz165 };

        private int                   _currentIndex;
        private NativeMethods.Devmode _currentDevMode;
        public  RefreshRateMode       CurrentRefreshRate { get; private set; }

        private readonly string _targetDisplayDeviceName;

        // Укажите ID оборудования дисплея в этой константе
        private const string TargetHardwareId = "MONITOR\\TMA2022";

        public DisplayModeSwitcher()
        {
            _targetDisplayDeviceName = FindDisplayDeviceByHardwareId(TargetHardwareId);
            if (string.IsNullOrEmpty(_targetDisplayDeviceName))
            {
                throw new Exception($"Failed to find the display device with hardware ID {TargetHardwareId}.");
            }

            _currentDevMode        = new NativeMethods.Devmode();
            _currentDevMode.dmSize = (ushort)Marshal.SizeOf(_currentDevMode);

            if (!NativeMethods.EnumDisplaySettings(_targetDisplayDeviceName, NativeMethods.EnumCurrentSettings, ref _currentDevMode))
            {
                throw new Exception($"Failed to get current display settings for the display: {_targetDisplayDeviceName}.");
            }

            CurrentRefreshRate = GetCurrentRefreshRateMode();
            _currentIndex      = Array.IndexOf(_refreshRates, CurrentRefreshRate);
        }

        public RefreshRateMode SetNextRefreshRate()
        {
            _currentIndex = (_currentIndex + 1) % _refreshRates.Length;

            ChangeDisplayFrequency((uint)_refreshRates[_currentIndex]);
            CurrentRefreshRate = GetCurrentRefreshRateMode();
            return _refreshRates[_currentIndex];
        }

        public RefreshRateMode GetCurrentRefreshRateMode()
        {
            return (RefreshRateMode)_currentDevMode.dmDisplayFrequency;
        }

        private void ChangeDisplayFrequency(uint frequency)
        {
            _currentDevMode.dmDisplayFrequency = frequency;
            _currentDevMode.dmFields           = NativeMethods.DmDisplayfrequency;

            var result = NativeMethods.ChangeDisplaySettings(ref _currentDevMode, NativeMethods.CdsUpdateregistry);

            if (result != NativeMethods.DispChangeSuccessful)
            {
                throw new Exception(
                    $"Switching to {frequency} Hz failed with error code {result}. Last error: {Marshal.GetLastWin32Error()}");
            }
        }

        private string FindDisplayDeviceByHardwareId(string hardwareId)
        {
            NativeMethods.DisplayDevice displayDevice = new NativeMethods.DisplayDevice();
            displayDevice.cb = Marshal.SizeOf(displayDevice);

            int deviceIndex = 0;
            while (NativeMethods.EnumDisplayDevices(null, deviceIndex, ref displayDevice, 0))
            {
                if (IsMatchingHardwareId(displayDevice.DeviceID, hardwareId))
                {
                    return displayDevice.DeviceName; // Вернет что-то вроде "DISPLAY1"
                }

                deviceIndex++;
            }

            return null!;
        }

        private bool IsMatchingHardwareId(string deviceInstanceId, string targetHardwareId)
        {
            // Открываем путь к устройствам в реестре
            string registryPath = $@"SYSTEM\CurrentControlSet\Enum\{deviceInstanceId}";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    foreach (string subKey in key.GetSubKeyNames())
                    {
                        using (RegistryKey subKeyHandle = key.OpenSubKey(subKey))
                        {
                            object? value = subKeyHandle?.GetValue("HardwareID");
                            if (value != null)
                            {
                                string[] hardwareIds = (string[])value;
                                foreach (string id in hardwareIds)
                                {
                                    if (id.Equals(targetHardwareId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}

namespace MiHotkeys.Services.DisplayManager
{
    public static class NativeMethods
    {
        public const int EnumCurrentSettings  = -1;
        public const int CdsUpdateregistry    = 0x01;
        public const int DispChangeSuccessful = 0;
        public const int DmDisplayfrequency   = 0x00400000;

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref Devmode devMode);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings([In, Out] ref Devmode lpDevMode, uint dwflags);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern bool EnumDisplayDevices(string lpDevice, int iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Devmode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint   dmFields;
            public int    dmPositionX;
            public int    dmPositionY;
            public uint   dmDisplayOrientation;
            public uint   dmDisplayFixedOutput;
            public short  dmColor;
            public short  dmDuplex;
            public short  dmYResolution;
            public short  dmTTOption;
            public short  dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public ushort dmLogPixels;
            public uint   dmBitsPerPel;
            public uint   dmPelsWidth;
            public uint   dmPelsHeight;
            public uint   dmDisplayFlags;
            public uint   dmDisplayFrequency;
            public uint   dmICMMethod;
            public uint   dmICMIntent;
            public uint   dmMediaType;
            public uint   dmDitherType;
            public uint   dmReserved1;
            public uint   dmReserved2;
            public uint   dmPanningWidth;
            public uint   dmPanningHeight;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DisplayDevice
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;

            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            AttachedToDesktop = 0x1,
            MultiDriver       = 0x2,
            PrimaryDevice     = 0x4,
            MirroringDriver   = 0x8,
            VGACompatible     = 0x10,
            Removable         = 0x20,
            ModesPruned       = 0x8000000,
            Remote            = 0x4000000,
            Disconnect        = 0x2000000
        }
    }
}
