using MiHotkeys.Common;
using MiHotkeys.Forms;
using MiHotkeys.Forms.UI;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.HotKeys;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys
{
    public class App : ApplicationContext
    {
        private readonly HotKeysService          _hotKeysService;
        private readonly TrayMenu                _trayIconBehavior;
        private readonly SynchronizationContext? _syncContext;
        private readonly Notification            _notification = new();

        public App(HotKeysService hotKeysService, bool powetLoadMonitorEnabled)
        {
            _syncContext               = SynchronizationContext.Current;
            _hotKeysService            = hotKeysService;
            _trayIconBehavior          = new TrayMenu(powetLoadMonitorEnabled);

            _trayIconBehavior.ChargingProtectionClicked      += hotKeysService.SetChargingProtect;
            _trayIconBehavior.PowerLoadMonitorCheckedChanged += hotKeysService.ChangePowerLoadMonitorState;
            hotKeysService.OnChargingProtectModeRecieved     =  OnChargingProtectModeRecieved;


            _hotKeysService.OnMicSwitched                += OnMicSwitched;
            _hotKeysService.OnPowerModeSwitched          += OnPowerModeSwitched;
            _hotKeysService.OnDisplayRefreshRateSwitched += OnDisplayRefreshRateSwitched;
            _hotKeysService.OnCurrentStatusChanged       += OnCurrentStatusChanged;

            _hotKeysService.StartListen();
            UpdateTrayTooltip();
        }


        private void OnCurrentStatusChanged(CurrentStatuses obj)
        {
            UpdateTrayTooltip();
        }

        private void OnDisplayRefreshRateSwitched(RefreshRateMode rateMode)
        {
            var message = TextFactory.GetRefreshRateMessage(rateMode);
            UpdateTrayTooltip();
            ShowNotification(message);
        }

        private void OnPowerModeSwitched(PowerMode powerMode)
        {
            var message = TextFactory.GetPowerModeMessage(powerMode);
            UpdateTrayTooltip();
            ShowNotification(message);
        }

        private void OnMicSwitched(bool micEnabled)
        {
            var message = TextFactory.MicSwitched(micEnabled);
            UpdateTrayTooltip();
            ShowNotification(message);
        }

        private void OnChargingProtectModeRecieved(bool isEnabled)
        {
            ExecuteOnUiThread(() => _trayIconBehavior.ChargingProtectReceived(isEnabled));
        }

        private void UpdateTrayTooltip()
        {
            ExecuteOnUiThread(() =>
                              {
                                  _trayIconBehavior.UpdateStatusToolTip(
                                      ToolTipIconFactory.GetIconByState(_hotKeysService.CurrentStatuses.PowerMode),
                                      TextFactory.ToolTipMainText(
                                          _hotKeysService.CurrentStatuses.PowerLoad,
                                          _hotKeysService.CurrentStatuses.PowerMode,
                                          _hotKeysService.CurrentStatuses.RefreshRateMode,
                                          _hotKeysService.CurrentStatuses.MicEnabled
                                      )
                                  );
                              });
        }

        private void ShowNotification(string message)
        {
            ExecuteOnUiThread(async () => await _notification.DisplayMessage(message));
        }

        private void ExecuteOnUiThread(Action action)
        {
            if (_syncContext != null)
            {
                _syncContext.Post(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _trayIconBehavior?.Dispose();
                _hotKeysService?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}