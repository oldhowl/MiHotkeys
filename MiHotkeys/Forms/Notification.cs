namespace MiHotkeys.Forms;

public sealed class Notification : Form
{
    public Notification(string message)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.Manual;
        BackColor       = Color.Black;
        ForeColor       = Color.White;
        Opacity         = 0.8;
        Width           = 100;
        Height          = 100;
        TopMost         = true;
        ShowInTaskbar   = false;

        Label label = new Label
        {
            Text      = message,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font      = new Font("Segoe UI", 16, FontStyle.Bold)
        };
        Controls.Add(label);
        Load += async (_, _) => await FadeOut();
    }

    private async Task FadeOut()
    {
        await Task.Delay(1000);
        for (double i = 0.8; i > 0; i -= 0.05)
        {
            Opacity = i;
            await Task.Delay(50);
        }
        Close();
    }
}
