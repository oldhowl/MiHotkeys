using MiHotkeys.Common;
using MiHotkeys.Forms.UI;
using MiHotkeys.Services.NativeServices;

namespace MiHotkeys.Forms
{
    public class TrayMenu : IDisposable
    {
        private const    string     StartupMenuItemText = "Run on start";
        private const    string     ExitMenuItemText    = "Exit";
        private const    string     ShortcutName        = "MiHotkeys.lnk";
        private const    string     FreepikLicense      = "Icons by Freepik.com";
        private readonly NotifyIcon _notifyTrayIcon;

        private bool              _chargingModeProtectionEnabled;
        private ToolStripMenuItem _chargingProtectionEnabledMenuItem;
        public event Action<bool> ChargingProtectionClicked;

        public TrayMenu()
        {
            _notifyTrayIcon = new NotifyIcon
            {
                Visible          = true,
                ContextMenuStrip = CreateContextMenu()
            };
        }

        #region Context menu

        private ContextMenuStrip CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            var autoStartMenuItem      = AutoStartMenuItem();
            var freepikLicense         = ToolStripMenuItem();
            var exitNotifyIconMenuItem = ExitNotifyIconMenuItem();
            _chargingProtectionEnabledMenuItem = ChargingProtectionEnabledMenuItem();


            contextMenu.Items.Add(freepikLicense);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(_chargingProtectionEnabledMenuItem);
            contextMenu.Items.Add(autoStartMenuItem);
            contextMenu.Items.Add(exitNotifyIconMenuItem);

            return contextMenu;
        }

        private ToolStripMenuItem ChargingProtectionEnabledMenuItem()
        {
            var menuItem = new ToolStripMenuItem()
            {
                Checked = _chargingModeProtectionEnabled,
                Enabled = false,
                Font    = CustomFonts.GetXiaomiFont(10),
                Text    = TextFactory.ChargingProtectionTrayMenuItemTitle,
            };


            menuItem.Click +=
                (_, _) =>
                {
                    _chargingProtectionEnabledMenuItem.Enabled = false;
                    ChargingProtectionClicked?.Invoke(!_chargingModeProtectionEnabled);
                };

            return menuItem;
        }


        public void ChargingProtectRecieved(bool isEnabled)
        {
            _chargingProtectionEnabledMenuItem.Enabled = true;
            _chargingProtectionEnabledMenuItem.Checked = isEnabled;
            _chargingModeProtectionEnabled             = isEnabled;
        }

        private ToolStripMenuItem AutoStartMenuItem()
        {
            var autoStartMenuItem = new ToolStripMenuItem(StartupMenuItemText)
            {
                Checked = AutoStartManager.IsInStartup(),
                Font    = CustomFonts.GetXiaomiFont(10)
            };
            autoStartMenuItem.Click += (s, _) => AutoStartMenuItem_Click(s, ShortcutName);
            return autoStartMenuItem;
        }

        private static ToolStripMenuItem ToolStripMenuItem()
        {
            return new ToolStripMenuItem(FreepikLicense)
            {
                Enabled = false,
                Font    = CustomFonts.GetXiaomiFont(10)
            };
        }

        private ToolStripMenuItem ExitNotifyIconMenuItem()
        {
            var exitMenuItem = new ToolStripMenuItem(ExitMenuItemText)
            {
                Font = CustomFonts.GetXiaomiFont(10)
            };

            exitMenuItem.Click += ExitMenuItem_Click;

            return exitMenuItem;
        }

        #endregion

        #region ToolTip

        public void UpdateStatusToolTip(Icon icon, string toolTipText)
        {
            _notifyTrayIcon.Icon = icon;
            _notifyTrayIcon.Text = toolTipText;
        }

        #endregion

        #region Menu event handlers

        private void AutoStartMenuItem_Click(object? sender, string shortcutName)
        {
            if (sender is not ToolStripMenuItem menuItem) return;

            switch (menuItem.Checked)
            {
                case true:
                    AutoStartManager.RemoveFromStartup();
                    menuItem.Checked = false;
                    break;
                default:
                    AutoStartManager.AddToStartup();
                    menuItem.Checked = true;
                    break;
            }
        }


        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            _notifyTrayIcon.Dispose();
            Application.Exit();
        }

        #endregion


        public void Dispose()
        {
            _notifyTrayIcon.Dispose();
        }
    }
}