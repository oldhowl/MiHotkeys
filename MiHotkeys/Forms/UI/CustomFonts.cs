using System.Drawing.Text;
using System.Runtime.InteropServices;
using MiHotkeys.Common;

namespace MiHotkeys.Forms.UI
{
    public static class CustomFonts
    {
        private static readonly PrivateFontCollection FontCollection;

        static CustomFonts()
        {
            FontCollection = new PrivateFontCollection();
            LoadFont();
        }

        public static Font GetXiaomiFont(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FontCollection.Families[0], size, style);
        }

        private static void LoadFont()
        {
            var fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourcesConstants.ResourcesPath, ResourcesConstants.MiFontFileName);
            var fontData = File.ReadAllBytes(fontPath);

            var fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            FontCollection.AddMemoryFont(fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);
        }
    }
}