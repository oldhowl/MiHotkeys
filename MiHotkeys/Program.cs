using MiHotkeys.Common;
using MiHotkeys.Forms;
using MiHotkeys.Services;
using MiHotkeys.Services.AudioManager;
using MiHotkeys.Services.BatteryInfo;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.HotKeys;
using MiHotkeys.Services.MiDevice;
using MiHotkeys.Services.NativeServices;
using MiHotkeys.Services.PowerManager;
using MiHotkeys.Services.PowerMonitor;

namespace MiHotkeys;

public class Program
{
    private static HotKeysService? _hotKeysService;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        var miDeviceService = new MiDeviceEventBus();

        _hotKeysService = new HotKeysService(
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

       
        Application.Run(new MainForm(_hotKeysService));
    }
}