using System;
using System.Windows.Forms;
using MiHotkeys.Common;
using MiHotkeys.Forms.UI;
using MiHotkeys.Services.NativeServices;

namespace MiHotkeys.Forms
{
    public class TrayMenu : IDisposable
    {
        private const string StartupMenuItemText = "Run on start";
        private const string ExitMenuItemText = "Exit";
        private const string ShortcutName = "MiHotkeys.lnk";
        private const string FreepikLicense = "Icons by Freepik.com";
        private readonly NotifyIcon _notifyTrayIcon;

        private bool _chargingModeProtectionEnabled;
        private bool _powerLoadMonitorChecked;
        
        private ToolStripMenuItem _chargingProtectionEnabledMenuItem;
        public event Action<bool> ChargingProtectionClicked;
        public event Action<bool> PowerLoadMonitorCheckedChanged;

        public TrayMenu(bool powerLoadMonitorChecked)
        {
            _powerLoadMonitorChecked = powerLoadMonitorChecked;
            _notifyTrayIcon = new NotifyIcon
            {
                Visible = true,
                ContextMenuStrip = CreateContextMenu()
            };
        }

        #region Context menu

        private ContextMenuStrip CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            var autoStartMenuItem = AutoStartMenuItem();
            var freepikLicense = ToolStripMenuItem();
            var exitNotifyIconMenuItem = ExitNotifyIconMenuItem();
            var powerLoadMonitorMenuItem = PowerLoadMonitorMenuItem();
            _chargingProtectionEnabledMenuItem = ChargingProtectionEnabledMenuItem();

            contextMenu.Items.Add(freepikLicense);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(_chargingProtectionEnabledMenuItem);
            contextMenu.Items.Add(powerLoadMonitorMenuItem);
            contextMenu.Items.Add(autoStartMenuItem);
            contextMenu.Items.Add(exitNotifyIconMenuItem);

            return contextMenu;
        }

        private ToolStripMenuItem PowerLoadMonitorMenuItem()
        {
            var menuItem = new ToolStripMenuItem()
            {
                Checked = _powerLoadMonitorChecked,
                Enabled = true,
                Font = CustomFonts.GetXiaomiFont(10),
                Text = TextFactory.PowerLoadMonitorTrayMenuItemTitle,
            };

            menuItem.Click +=
                (_, _) =>
                {
                    _powerLoadMonitorChecked = !_powerLoadMonitorChecked;
                    menuItem.Checked = _powerLoadMonitorChecked;
                    PowerLoadMonitorCheckedChanged?.Invoke(_powerLoadMonitorChecked);
                };

            return menuItem;
        }

        private ToolStripMenuItem ChargingProtectionEnabledMenuItem()
        {
            var menuItem = new ToolStripMenuItem()
            {
                Checked = _chargingModeProtectionEnabled,
                Enabled = false,
                Font = CustomFonts.GetXiaomiFont(10),
                Text = TextFactory.ChargingProtectionTrayMenuItemTitle,
            };

            menuItem.Click +=
                (_, _) =>
                {
                    ExecuteOnUiThread(() =>
                    {
                        _chargingProtectionEnabledMenuItem.Enabled = false;
                        ChargingProtectionClicked?.Invoke(!_chargingModeProtectionEnabled);
                    });
                };

            return menuItem;
        }

        public void ChargingProtectReceived(bool isEnabled)
        {
            ExecuteOnUiThread(() =>
            {
                _chargingModeProtectionEnabled = isEnabled;
                _chargingProtectionEnabledMenuItem.Enabled = true;
                _chargingProtectionEnabledMenuItem.Checked = _chargingModeProtectionEnabled;
            });
        }

        private ToolStripMenuItem AutoStartMenuItem()
        {
            var autoStartMenuItem = new ToolStripMenuItem(StartupMenuItemText)
            {
                Checked = AutoStartManager.IsInStartup(),
                Font = CustomFonts.GetXiaomiFont(10)
            };
            autoStartMenuItem.Click += (s, _) => AutoStartMenuItem_Click(s, ShortcutName);
            return autoStartMenuItem;
        }

        private static ToolStripMenuItem ToolStripMenuItem()
        {
            return new ToolStripMenuItem(FreepikLicense)
            {
                Enabled = false,
                Font = CustomFonts.GetXiaomiFont(10)
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
            ExecuteOnUiThread(() =>
            {
                _notifyTrayIcon.Icon = icon;
                _notifyTrayIcon.Text = toolTipText;
            });
        }

        #endregion

        #region Menu event handlers

        private void AutoStartMenuItem_Click(object? sender, string shortcutName)
        {
            ExecuteOnUiThread(() =>
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
            });
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            ExecuteOnUiThread(() =>
            {
                _notifyTrayIcon.Dispose();
                Application.Exit();
            });
        }

        #endregion

        public void ExecuteOnUiThread(Action action)
        {
            if (_notifyTrayIcon.ContextMenuStrip.InvokeRequired)
            {
                _notifyTrayIcon.ContextMenuStrip.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void Dispose()
        {
            _notifyTrayIcon.Dispose();
        }
    }
}
