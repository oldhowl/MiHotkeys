using System.Runtime.InteropServices;

namespace MiHotkeys.Services.DisplayManager
{
    public class DisplayModeSwitcher
    {
        private readonly RefreshRateMode[] _refreshRates = { RefreshRateMode.Hz60, RefreshRateMode.Hz120, RefreshRateMode.Hz165 };
        private int _currentIndex;
        private NativeMethods.Devmode _currentDevMode;

        public DisplayModeSwitcher()
        {
            _currentDevMode = new NativeMethods.Devmode();
            _currentDevMode.dmSize = (ushort)Marshal.SizeOf(_currentDevMode);

            if (!NativeMethods.EnumDisplaySettings(null!, NativeMethods.EnumCurrentSettings, ref _currentDevMode))
            {
                throw new Exception("Failed to get current display settings.");
            }

            var currentMode = GetCurrentRefreshRateMode();
            _currentIndex = Array.IndexOf(_refreshRates, currentMode);
        }

        public RefreshRateMode SetNextRefreshRate()
        {
            _currentIndex = (_currentIndex + 1) % _refreshRates.Length;

            ChangeDisplayFrequency((uint)_refreshRates[_currentIndex]);

            return _refreshRates[_currentIndex];
        }

        public RefreshRateMode GetCurrentRefreshRateMode()
        {
            return (RefreshRateMode)_currentDevMode.dmDisplayFrequency;
        }

        private void ChangeDisplayFrequency(uint frequency)
        {
            _currentDevMode.dmDisplayFrequency = frequency;
            _currentDevMode.dmFields = NativeMethods.DmDisplayfrequency;

            var result = NativeMethods.ChangeDisplaySettings(ref _currentDevMode, NativeMethods.CdsUpdateregistry);

            if (result != NativeMethods.DispChangeSuccessful)
            {
                throw new Exception($"Switching to {frequency} Hz failed with error code {result}. Last error: {Marshal.GetLastWin32Error()}");
            }
        }
    }
}
