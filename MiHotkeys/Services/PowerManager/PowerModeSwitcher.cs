using System.Runtime.InteropServices;

namespace MiHotkeys.Services.PowerManager
{
    public class PowerModeSwitcher
    {
        private readonly IPowerModeProvider _powerModeProvider;
        public           PowerMode          CurrentMode;


        public PowerModeSwitcher(IPowerModeProvider powerModeProvider)
        {
            _powerModeProvider = powerModeProvider;
            RequestPowerMode();
        }

        public void SetNextPowerMode()
        {
            if (CurrentMode == PowerMode.Pending)
                return;

            var powerMode = (int)CurrentMode;

            if (CurrentMode == PowerMode.Turbo)
                powerMode = (int)PowerMode.Silence;
            else
            {
                powerMode += 1;
            }

            _powerModeProvider.SetPowerMode(powerMode);
            _powerModeProvider.RequestCurrentPowerMode();
        }

        private void RequestPowerMode()
        {
            CurrentMode = PowerMode.Pending;
            _powerModeProvider.RequestCurrentPowerMode();
        }

        public void SetPowerMode(int powerMode)
        {
            CurrentMode = (PowerMode)powerMode;
        }

        
    }
}