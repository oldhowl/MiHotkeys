namespace MiHotkeys.Common;

public class ResourcesConstants
{
    public static string FullResourceFilePath(string resourceName) => Path.Combine(ResourcesPath, resourceName);
    public const  string ResourcesPath      = "Resources";
    public const  string MiFontFileName     = "MiSans.ttf";
    public const  string LowIcoFileName     = "low.ico";
    public const  string MidIcoFileName     = "mid.ico";
    public const  string HighIcoFileName    = "high.ico";
    public const  string FireIcoFileName    = "fire.ico";
    public const  string RequestIcoFileName = "request.ico";

    public const string SplashScreenLogo = "splash_screen_logo.png";
}