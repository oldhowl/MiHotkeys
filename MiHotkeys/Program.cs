using System.Management;
using MiHotkeys.Common;
using MiHotkeys.Forms;
using MiHotkeys.Forms.UI;
using MiHotkeys.Services.AudioManager;
using MiHotkeys.Services.BatteryInfo;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.HotKeys;
using MiHotkeys.Services.MiDevice;
using MiHotkeys.Services.NativeServices;
using MiHotkeys.Services.PowerManager;
using MiHotkeys.Services.PowerMonitor;
using MiHotkeys.Services.Settings;


namespace MiHotkeys;

public class Program
{
    [STAThread]
    static void Main()
    {
        ExceptionHandler.Initialize();

        
        if (!SingleInstanceChecker.IsSingleInstance())
        {
            MessageBox.Show(TextConstants.ApplicationAlreadyRunningErrorMessage, string.Empty, MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        ApplicationConfiguration.Initialize();
        ShowSplashScreen();

        var appSettingsManager = new AppSettingsManager();
        var miDeviceService    = new MiDeviceEventBus();


        var hotKeysService = new HotKeysService(
            appSettingsManager,
            new PowerMonitorService(),
            miDeviceService,
            new BatteryInfoService(),
            new HotKeysOptions(
                [KeysConstants.MiButtonCode, KeysConstants.ScreenModeSwitchCode],
                [KeysConstants.MiButtonCode],
                [KeysConstants.MiButtonCode, KeysConstants.MicSwitchButtonCode],
                [KeysConstants.MiButtonCode, KeysConstants.SnippingToolButtonCode],
                [KeysConstants.MiButtonCode, KeysConstants.SystemSettingsButtonCode]
            ),
            new MultimediaHardwareService(),
            new PowerModeSwitcher(new PowerModeProvider(miDeviceService)),
            new KeyboardHook(),
            new DisplayModeSwitcher()
        );

        Application.Run(new App(hotKeysService, appSettingsManager.GetPowerLoadMonitorEnabled()));
        SingleInstanceChecker.Release();
    }

    private static void ShowSplashScreen()
    {
        SplashScreen splashScreen = new(ResourcesConstants.FullResourceFilePath(ResourcesConstants.SplashScreenLogo));
        splashScreen.ShowFor5SecondsAndClose();
    }
}