using MiHotkeys.Common;
using MiHotkeys.Forms.UI;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.HotKeys;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Forms
{
    public class MainForm : Form
    {
        private readonly HotKeysService _hotKeysService;
        private readonly TrayMenu       _trayIconBehavior;


        public MainForm(HotKeysService hotKeysService)
        {
            _hotKeysService   = hotKeysService;
            _trayIconBehavior = new TrayMenu();

            _trayIconBehavior.ChargingProtectionClicked  +=  hotKeysService.SetChargingProtect;
            hotKeysService.OnChargingProtectModeRecieved = OnChargingProtectModeRecieved;

            Visible       = false;
            WindowState   = FormWindowState.Minimized;
            ShowInTaskbar = false;

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
            ExecuteOnUiThread(() => { _trayIconBehavior.ChargingProtectRecieved(isEnabled); });
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
            ExecuteOnUiThread(() =>
                              {
                                  var notificationForm = new Notification(message);
                                  notificationForm.Show();
                              });
        }


        private void ExecuteOnUiThread(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _trayIconBehavior.Dispose();
            _hotKeysService.Dispose();

            base.OnFormClosing(e);
        }
    }
}