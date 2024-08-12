using MiHotkeys.Forms;
using MiHotkeys.Services;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys;

public class Program 
{
    private static HotKeysService? _hotKeysService;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        _hotKeysService = new HotKeysService(
            new PowerModeSwitcher(
                new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a"),
                new Guid("00000000-0000-0000-0000-000000000000"),
                new Guid("ded574b5-45a0-4f42-8737-46345c09c238")
            ),
            new KeyboardHook(),
            new DisplayModeSwitcher()
        );

        Application.Run(new MainForm(_hotKeysService));
    }

 
}