using MiHotkeys.Services;

namespace MiHotkeys.Forms
{
    public class TrayMenu : IDisposable
    {
        private const string StartupMenuItemText = "Run on start";
        private const string ExitMenuItemText = "Exit";
        private const string IconPath = "Resources/mi_logo.ico";
        private const string ShortcutName = "MiHotkeys.lnk";

        private NotifyIcon? _notifyTrayIcon;

        public TrayMenu()
        {
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            _notifyTrayIcon = new NotifyIcon
            {
                Icon = new Icon(IconPath),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            var contextMenu = new ContextMenuStrip();
            var autoStartMenuItem = new ToolStripMenuItem(StartupMenuItemText) { Checked = AutoStartManager.IsInStartup(ShortcutName) };
            var exitNotifyIconMenuItem = new ToolStripMenuItem(ExitMenuItemText);

            autoStartMenuItem.Click += (s, _) => AutoStartMenuItem_Click(s, ShortcutName);
            exitNotifyIconMenuItem.Click += ExitMenuItem_Click;

            contextMenu.Items.Add(autoStartMenuItem);
            contextMenu.Items.Add(exitNotifyIconMenuItem);
            _notifyTrayIcon.ContextMenuStrip = contextMenu;
        }

        private void AutoStartMenuItem_Click(object? sender, string shortcutName)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                if (menuItem.Checked)
                {
                    AutoStartManager.RemoveFromStartup(shortcutName);
                    menuItem.Checked = false;
                }
                else
                {
                    AutoStartManager.AddToStartup(shortcutName);
                    menuItem.Checked = true;
                }
            }
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            _notifyTrayIcon?.Dispose();
            Application.Exit();
        }

        public void Dispose()
        {
            _notifyTrayIcon?.Dispose();
        }
    }
}
