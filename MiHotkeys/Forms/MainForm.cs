using MiHotkeys.Services;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Forms
{
    public  class MainForm : Form
    {
        private readonly HotKeysService _hotKeysService;
        private readonly TrayMenu       _trayIconBehavior;

        public MainForm()
        {
            var powerModeSwitcher = new PowerModeSwitcher(
                new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a"),
                new Guid("00000000-0000-0000-0000-000000000000"),
                new Guid("ded574b5-45a0-4f42-8737-46345c09c238")
            );

            WindowState   = FormWindowState.Minimized;
            ShowInTaskbar = false;

            _hotKeysService = new HotKeysService(
                powerModeSwitcher,
                new KeyboardHook(),
                new DisplayModeSwitcher(),
                ShowNotification
            );
            _trayIconBehavior = new TrayMenu();
            
            _trayIconBehavior.UpdateStatusToolTip(
                _hotKeysService.CurrentStatuses.PowerMode,
                _hotKeysService.CurrentStatuses.RefreshRateMode.ToString()
            );
        }

        private void ShowNotification(string message)
        {
            Invoke((Action)(() =>
                            {
                                _trayIconBehavior.UpdateStatusToolTip(
                                    _hotKeysService.CurrentStatuses.PowerMode,
                                    _hotKeysService.CurrentStatuses.RefreshRateMode.ToString()
                                );
                                
                                var notificationForm = new Notification(message)
                                {
                                    StartPosition = FormStartPosition.Manual,
                                    Left          = 10,
                                    Top           = 10
                                };
                                notificationForm.Show();
                            }));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _hotKeysService.Dispose();
            _trayIconBehavior.Dispose(); 
            base.OnFormClosing(e);
        }
    }
}