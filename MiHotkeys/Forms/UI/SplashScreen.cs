namespace MiHotkeys.Forms.UI;

public sealed class SplashScreen : Form
{
    public SplashScreen(string imagePath)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition   = FormStartPosition.CenterScreen;
        Size            = new Size(200, 133);

        ShowInTaskbar = false;
        TopLevel      = true;

        BackColor       = Color.FromArgb(255, 255, 105, 0);
        TransparencyKey = Color.FromArgb(255, 255, 105, 0);

        var pictureBox = new PictureBox
        {
            Image     = Image.FromFile(imagePath),
            Size      = new Size(200, 133),
            Dock      = DockStyle.Fill,
            SizeMode  = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Transparent
        };
        Controls.Add(pictureBox);
    }

    public void ShowFor5SecondsAndClose()
    {
        Show();
        Task.Run(async () =>
                 {
                     await Task.Delay(5000);
                     Invoke(Close);
                 });
    }
}