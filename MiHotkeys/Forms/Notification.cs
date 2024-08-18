using System.Drawing.Drawing2D;
using MiHotkeys.Forms.UI;

namespace MiHotkeys.Forms
{
    public sealed class Notification : Form
    {
        public Notification(string message)
        {
            Left            = 10;
            Top             = 10;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition   = FormStartPosition.Manual;
            BackColor       = Color.Black;
            ForeColor       = Color.White;
            Opacity         = 0.8;
            Width           = 100;
            Height          = 100;
            TopMost         = true;
            ShowInTaskbar   = false;

            var label = new Label
            {
                Text      = message,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = CustomFonts.GetXiaomiFont(16, FontStyle.Bold)
            };
            Controls.Add(label);
            Load += async (_, _) => await FadeOut();
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

        private async Task FadeOut()
        {
            await Task.Delay(1000);
            for (var i = 0.8; i > 0; i -= 0.05)
            {
                Opacity = i;
                await Task.Delay(50);
            }

            Close();
        }
    }
}