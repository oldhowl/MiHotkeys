using System.ComponentModel;
using MiHotkeys.Services;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Forms
{
    public class MainForm : Form
    {
        private readonly HotKeysService _hotKeysService;
        private readonly TrayMenu        _trayIconBehavior;

        public MainForm(HotKeysService hotKeysService)
        {
            _hotKeysService   = hotKeysService;
            _trayIconBehavior = new TrayMenu();

            _hotKeysService.NotificationCallback = ShowNotification;
            
            WindowState       = FormWindowState.Minimized;
            ShowInTaskbar     = false;
            

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
            _trayIconBehavior.Dispose();
            base.OnFormClosing(e);
        }
    }
}