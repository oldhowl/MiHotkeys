using System.Runtime.InteropServices;

namespace MiHotkeys.Services.DisplayManager
{
    public class DisplayModeSwitcher : IDisplayModeManager
    {
        private const string TargetDeviceString = "Intel(R) Arc(TM) Graphics";

        private readonly RefreshRateMode[] _refreshRates =
            [RefreshRateMode.Hz60, RefreshRateMode.Hz120, RefreshRateMode.Hz165];

        private          int                          _currentIndex;
        private          NativeMethods.Devmode        _currentDevMode;
        private readonly NativeMethods.DISPLAY_DEVICE _targetDisplayDevice;

        public RefreshRateMode CurrentRefreshRate { get; private set; }

        public DisplayModeSwitcher()
        {
            _targetDisplayDevice = GetLaptopDisplayDevice()
                                   ?? throw new Exception(
                                       $"Failed to find the display device with TargetDeviceString {TargetDeviceString}.");

            _currentDevMode = new NativeMethods.Devmode
            {
                dmSize = (ushort)Marshal.SizeOf(typeof(NativeMethods.Devmode))
            };

            if (!NativeMethods.EnumDisplaySettings(_targetDisplayDevice.DeviceName, NativeMethods.EnumCurrentSettings,
                    ref _currentDevMode))
            {
                throw new Exception(
                    $"Failed to get current display settings for the display: {_targetDisplayDevice.DeviceName}.");
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

        private static NativeMethods.DISPLAY_DEVICE? GetLaptopDisplayDevice()
        {
            var displayDevice = new NativeMethods.DISPLAY_DEVICE
                { cb = Marshal.SizeOf<NativeMethods.DISPLAY_DEVICE>() };

            for (uint id = 0; NativeMethods.EnumDisplayDevices(null, id, ref displayDevice, 0); id++)
            {
                if (displayDevice.DeviceString.Equals(TargetDeviceString))
                {
                    return displayDevice;
                }
            }

            return null;
        }

        private void ChangeDisplayFrequency(uint frequency)
        {
            _currentDevMode.dmDisplayFrequency = frequency;
            _currentDevMode.dmFields           = NativeMethods.DmDisplayfrequency;

            var result = NativeMethods.ChangeDisplaySettingsEx(_targetDisplayDevice.DeviceName, ref _currentDevMode,
                IntPtr.Zero, NativeMethods.CdsUpdateregistry, IntPtr.Zero);

            if (result != NativeMethods.DispChangeSuccessful)
            {
                throw new Exception(
                    $"Switching to {frequency} Hz failed with error code {result}. Last error: {Marshal.GetLastWin32Error()}");
            }
        }
    }
}