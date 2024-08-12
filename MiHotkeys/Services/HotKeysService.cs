using MiHotkeys.Common;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Services;

public class HotKeysService : IDisposable
{
    private readonly PowerModeSwitcher   _powerModeSwitcher;
    private readonly IKeyboardHook       _keyboardHook;
    private readonly DisplayModeSwitcher _displayModeSwitcher;
    private readonly long[]              _screenModeSwitchCombination;
    public           Action<string>      NotificationCallback;

    public CurrentStatuses CurrentStatuses { get; private set; }

    public HotKeysService(PowerModeSwitcher   powerModeSwitcher, IKeyboardHook keyboardHook,
                          DisplayModeSwitcher displayModeSwitcher)
    {
        CurrentStatuses = new CurrentStatuses();
        _screenModeSwitchCombination =
            new[] { KeysConstants.MiButtonCode, KeysConstants.ScreenModeSwitchCode }.Order().ToArray();
        _powerModeSwitcher   = powerModeSwitcher;
        _keyboardHook        = keyboardHook;
        _displayModeSwitcher = displayModeSwitcher;

        _keyboardHook.KeyCombinationPressed += OnKeyCombinationPressed;

        CurrentStatuses.PowerMode       = _powerModeSwitcher.CurrentMode;
        CurrentStatuses.RefreshRateMode = _displayModeSwitcher.CurrentRefreshRate;
    }

    public void OnKeyCombinationPressed(long[] keysPressed)
    {
        if (keysPressed.Length == 1 && keysPressed.First() == KeysConstants.MiButtonCode)
        {
            var currentPowerMode = _powerModeSwitcher.SetNextPowerMode();
            CurrentStatuses.PowerMode = _powerModeSwitcher.CurrentMode;

            NotificationCallback.Invoke(currentPowerMode switch
            {
                PowerMode.Silence  => "Silence",
                PowerMode.Balance  => "Balance",
                PowerMode.MaxPower => "Max power",
                _                  => throw new ArgumentOutOfRangeException()
            });

            return;
        }

        keysPressed = keysPressed.Order().ToArray();

        if (keysPressed.SequenceEqual(_screenModeSwitchCombination))
        {
            var currentRefreshRate = _displayModeSwitcher.SetNextRefreshRate();
            CurrentStatuses.RefreshRateMode = _displayModeSwitcher.CurrentRefreshRate;
            NotificationCallback.Invoke(currentRefreshRate switch
            {
                RefreshRateMode.Hz60  => "60 Hz",
                RefreshRateMode.Hz120 => "120 Hz",
                RefreshRateMode.Hz165 => "165 Hz",
                _                     => throw new ArgumentOutOfRangeException()
            });
        }
    }

    public void Dispose()
    {
        _keyboardHook.Dispose();
    }
}