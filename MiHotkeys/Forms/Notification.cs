using Microsoft.Win32;
using System.Drawing.Drawing2D;
using MiHotkeys.Forms.UI;

namespace MiHotkeys.Forms
{
    public sealed class Notification : Form
    {
        private readonly Label                   _label;
        private          CancellationTokenSource _cancellationTokenSource;

        public Notification()
        {
            var displayIndex = 0;
            var screens = Screen.AllScreens;
            if (displayIndex < 0 || displayIndex >= screens.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(displayIndex), "Invalid display index");
            }
        
            var screen = screens[displayIndex];
            StartPosition   = FormStartPosition.Manual;
            Location        = new Point(screen.WorkingArea.Left + 10, screen.WorkingArea.Top + 10);
            // Left            = 10;
            // Top             = 10;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.Manual;
            Opacity         = 0.8;
            Width           = 200;
            Height          = 100;
            TopMost         = true;  
            ShowInTaskbar   = false; 

            FormBorderStyle      =  FormBorderStyle.None;
            CreateParams.ExStyle |= 0x80;

            SetTheme();

            _label = new Label
            {
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = CustomFonts.GetXiaomiFont(16, FontStyle.Bold)
            };
            Controls.Add(_label);

            _cancellationTokenSource = new CancellationTokenSource();

            // Подписка на событие изменения системной темы
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        private void SetTheme()
        {
            var isDarkMode = IsDarkThemeEnabled();

            if (isDarkMode)
            {
                BackColor = Color.Black;
                ForeColor = Color.White;
            }
            else
            {
                BackColor = Color.White;
                ForeColor = Color.Black;
            }
        }

        private static bool IsDarkThemeEnabled()
        {
            try
            {
                using RegistryKey? key =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                if (value != null && Convert.ToInt32(value) == 0)
                {
                    return true; // Dark mode is enabled
                }
            }
            catch (Exception)
            {
                // Если не удалось прочитать реестр, возвращаем значение по умолчанию (светлая тема)
            }

            return false;
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                SetTheme();
            }
        }

        public async Task DisplayMessage(string message)
        {
            // Cancel the previous fading operation if any
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Opacity     = 0.8;
            _label.Text = message;
            Show();

            try
            {
                await FadeOut(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Handle the operation being canceled if necessary
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            const int radius = 20;
            var       bounds = new Rectangle(0, 0, Width, Height);
            var       path   = CreateRoundedRectanglePath(bounds, radius);
            Region = new Region(path);
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right            - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right            - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private async Task FadeOut(CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
            for (var i = 0.8; i > 0; i -= 0.05)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Opacity = i;
                await Task.Delay(50, cancellationToken);
            }

            Opacity = -1;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
                return cp;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
                _cancellationTokenSource.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}