using MiHotkeys.Services;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Forms
{
    public class TrayMenu : IDisposable
    {
        private const    string     StartupMenuItemText = "Run on start";
        private const    string     ExitMenuItemText    = "Exit";
        private const    string     ShortcutName        = "MiHotkeys.lnk";
        private const    string     FreepikLicense      = "Icons by Freepik.com";
        private readonly NotifyIcon _notifyTrayIcon;

        public TrayMenu()
        {
            _notifyTrayIcon = new NotifyIcon
            {
                Visible          = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            var contextMenu = new ContextMenuStrip();

            var autoStartMenuItem = new ToolStripMenuItem(StartupMenuItemText)
                { Checked = AutoStartManager.IsInStartup(ShortcutName) };

            var exitNotifyIconMenuItem = new ToolStripMenuItem(ExitMenuItemText);

            autoStartMenuItem.Click      += (s, _) => AutoStartMenuItem_Click(s, ShortcutName);
            exitNotifyIconMenuItem.Click += ExitMenuItem_Click;

            contextMenu.Items.Add(new ToolStripMenuItem(FreepikLicense)
            {
                Enabled = false,
            });


            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(autoStartMenuItem);
            contextMenu.Items.Add(exitNotifyIconMenuItem);
            _notifyTrayIcon.ContextMenuStrip = contextMenu;
        }


        public void UpdateStatusToolTip(PowerMode powerMode, string refreshRate)
        {
            _notifyTrayIcon.Icon = GetIconByState(powerMode);

            _notifyTrayIcon.Text = $"Power Mode: {powerMode.ToString()}\nRefresh Rate: {refreshRate}";
        }

        private Icon GetIconByState(PowerMode state)
        {
            return new Icon(state switch
            {
                PowerMode.Silence  => "Resources/low.ico",
                PowerMode.Balance  => "Resources/mid.ico",
                PowerMode.MaxPower => "Resources/high.ico",
                _                  => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            });
        }

        private void AutoStartMenuItem_Click(object? sender, string shortcutName)
        {
            if (sender is not ToolStripMenuItem menuItem) return;

            switch (menuItem.Checked)
            {
                case true:
                    AutoStartManager.RemoveFromStartup(shortcutName);
                    menuItem.Checked = false;
                    break;
                default:
                    AutoStartManager.AddToStartup(shortcutName);
                    menuItem.Checked = true;
                    break;
            }
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            _notifyTrayIcon.Dispose();
            Application.Exit();
        }

        public void Dispose()
        {
            _notifyTrayIcon.Dispose();
        }
    }
}