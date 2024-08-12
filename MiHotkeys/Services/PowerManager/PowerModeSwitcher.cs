using System.Runtime.InteropServices;

namespace MiHotkeys.Services.PowerManager
{
    public class PowerModeSwitcher
    {
        public           PowerMode                   CurrentMode;
        private readonly Dictionary<PowerMode, Guid> _powerModeGuids;

        public PowerModeSwitcher(Guid silenceMode, Guid balancedMode, Guid maxPowerMode)
        {
            _powerModeGuids = new Dictionary<PowerMode, Guid>()
            {
                [PowerMode.Silence]  = silenceMode,
                [PowerMode.Balance]  = balancedMode,
                [PowerMode.MaxPower] = maxPowerMode
            };

            if (_powerModeGuids.Count < 3)
            {
                throw new ArgumentException("The dictionary must contain at least three power modes.");
            }

            CurrentMode = GetCurrentPowerMode();
        }

        public PowerMode SetNextPowerMode()
        {
            CurrentMode = CurrentMode switch
            {
                PowerMode.Silence  => PowerMode.Balance,
                PowerMode.Balance  => PowerMode.MaxPower,
                PowerMode.MaxPower => PowerMode.Silence,
                _                  => throw new ArgumentOutOfRangeException()
            };

            if (_powerModeGuids.TryGetValue(CurrentMode, out var modeGuid))
            {
                var result = PowerSetActiveOverlayScheme(modeGuid);
                if (result != 0)
                {
                    throw new InvalidOperationException($"Failed to set power mode. Error code: {result}");
                }

                return CurrentMode;
            }
            else
            {
                throw new InvalidOperationException("Unknown power mode GUID.");
            }
        }

        private PowerMode GetCurrentPowerMode()
        {
            var result = PowerGetActualOverlayScheme(out var actualSchemeGuid);

            if (result != 0)
            {
                throw new InvalidOperationException($"Failed to get current power mode. Error code: {result}");
            }

            foreach (var mode in _powerModeGuids)
            {
                if (mode.Value == actualSchemeGuid)
                {
                    return mode.Key;
                }
            }

            throw new InvalidOperationException("Unknown current power mode.");
        }

        [DllImport("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        private static extern uint PowerSetActiveOverlayScheme(Guid overlaySchemeGuid);

        [DllImport("powrprof.dll", EntryPoint = "PowerGetActualOverlayScheme")]
        private static extern int PowerGetActualOverlayScheme(out Guid actualOverlayGuid);
    }
}