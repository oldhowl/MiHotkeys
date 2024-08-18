using System.Diagnostics;
using MiHotkeys.Common;
using MiHotkeys.Services.AudioManager;
using MiHotkeys.Services.BatteryInfo;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.MiDevice;
using MiHotkeys.Services.MiDevice.Commands;
using MiHotkeys.Services.MiDevice.Events;
using MiHotkeys.Services.PowerManager;
using MiHotkeys.Services.PowerMonitor;
using Timer = System.Threading.Timer;

namespace MiHotkeys.Services.HotKeys;

public class HotKeysService : IDisposable
{
    private readonly Timer                     _updateTimer;
    private          BatteryInfoService        BatteryInfoService { get; }
    private readonly MultimediaHardwareService _multimediaHardwareService;
    private readonly PowerModeSwitcher         _powerModeSwitcher;
    private readonly PowerMonitorService       _powerMonitorService;
    private readonly IMiDeviceService          _miDeviceService;
    private readonly IKeyboardHook             _keyboardHook;
    private readonly DisplayModeSwitcher       _displayModeSwitcher;
    private readonly Dictionary<long, Action>  _hotKeysRouter;

    private bool                          _chargingProtectModeEnabled;
    public  CurrentStatuses               CurrentStatuses { get; private set; }
    public event Action<PowerMode>?       OnPowerModeSwitched;
    public event Action<bool>?            OnMicSwitched;
    public event Action<RefreshRateMode>? OnDisplayRefreshRateSwitched;
    public event Action<CurrentStatuses>? OnCurrentStatusChanged;
    public Action<bool>                   OnChargingProtectModeRecieved;


    public HotKeysService(
        PowerMonitorService       powerMonitorService,
        IMiDeviceService          miDeviceService,
        BatteryInfoService        batteryInfoService,
        HotKeysOptions            hotKeysOptions,
        MultimediaHardwareService multimediaHardwareService,
        PowerModeSwitcher         powerModeSwitcher,
        IKeyboardHook             keyboardHook,
        DisplayModeSwitcher       displayModeSwitcher)
    {
        BatteryInfoService                  =  batteryInfoService;
        _powerMonitorService                =  powerMonitorService;
        _miDeviceService                    =  miDeviceService;
        _keyboardHook                       =  keyboardHook;
        _keyboardHook.KeyCombinationPressed += OnKeyCombinationPressed;

        _multimediaHardwareService = multimediaHardwareService;
        _powerModeSwitcher         = powerModeSwitcher;
        _displayModeSwitcher       = displayModeSwitcher;

        _miDeviceService.OnChargingProtectModeRecieved += ChargingProtectModeRecieved;
        _miDeviceService.OnPowerModeRecieved           += OnPowerModeRecieved;
        _powerMonitorService.PowerStatusChanged        += PowerStatusChanged;


        _hotKeysRouter = new Dictionary<long, Action>()
        {
            [hotKeysOptions.MicSwitchCombination]        = MicSwitched,
            [hotKeysOptions.PowerModeSwitchCombination]  = PowerModeSwitched,
            [hotKeysOptions.ScreenModeSwitchCombination] = ScreenRefreshRateModeSwitched,
            [hotKeysOptions.SnippingToolCombination]     = SnippingToolCombinationPressed,
            [hotKeysOptions.SystemSettingsCombination]   = SystemSettingsCombinationPressed
        };


        CurrentStatuses = CurrentStatuses.SetCurrent(
            batteryInfoService.GetPowerLoad(),
            _powerModeSwitcher.CurrentMode,
            displayModeSwitcher.CurrentRefreshRate,
            multimediaHardwareService.IsMicEnabled());

        _updateTimer = new Timer(UpdateBatteryStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    public void StartListen()
    {
        _miDeviceService.Open();
        GetChargingProtectMode();
        _miDeviceService.Execute(new GetWorkloadMode());
    }

    private void OnPowerModeRecieved(object? sender, PowerModeReceivedEventArgs e)
    {
        _powerModeSwitcher.SetPowerMode(e.Mode);
        CurrentStatuses.PowerMode = (PowerMode)e.Mode;
        OnPowerModeSwitched?.Invoke(CurrentStatuses.PowerMode);
    }

    private async void PowerStatusChanged(object? sender, PowerStatusChangedEventArgs e)
    {
        if (e.PowerSource != PowerSource.AC)
            return;

        if (_chargingProtectModeEnabled == false)
            return;

        _miDeviceService.Execute(new SetChargingProtectCommand(false));
        await Task.Delay(1000);
        _miDeviceService.Execute(new SetChargingProtectCommand(true));
    }

    private void ChargingProtectModeRecieved(object? sender, ChargingProtectModeReceivedEventArgs e)
    {
        if (e.IsEnabled && !_chargingProtectModeEnabled)
            _miDeviceService.Execute(new SetChargingProtectCommand(true));

        _chargingProtectModeEnabled = e.IsEnabled;
        OnChargingProtectModeRecieved(e.IsEnabled);
    }

    private Task GetChargingProtectMode()
    {
        _miDeviceService.Execute(new GetChargingProtectQuery());
        return Task.CompletedTask;
    }

    public void SetChargingProtect(bool isEnabled)
    {
        _miDeviceService.Execute(new SetChargingProtectCommand(isEnabled));
        GetChargingProtectMode();
    }


    private void SystemSettingsCombinationPressed() => StartWindowsNativeProcess("ms-settings:system");

    private void SnippingToolCombinationPressed() => StartWindowsNativeProcess("ms-screensketch:");

    private void StartWindowsNativeProcess(string processName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName        = "cmd.exe",
            Arguments       = $"/c start {processName}",
            UseShellExecute = false,
            CreateNoWindow  = true
        };
        Process.Start(startInfo);
    }

    private void UpdateBatteryStatus(object? state)
    {
        //TODO: Что бы не долбить WMI нужно вызывать запрос только во время открытия тултипа. но события подходящего не оказалось.
        // if (!_batteryInfoOpened)
        // return;

        CurrentStatuses.PowerLoad = BatteryInfoService.GetPowerLoad();
        OnCurrentStatusChanged?.Invoke(CurrentStatuses);
    }

    private void ScreenRefreshRateModeSwitched()
    {
        var currentRefreshRate = _displayModeSwitcher.SetNextRefreshRate();
        CurrentStatuses.RefreshRateMode = _displayModeSwitcher.CurrentRefreshRate;
        OnDisplayRefreshRateSwitched?.Invoke(currentRefreshRate);
    }

    private void PowerModeSwitched()
    {
        _powerModeSwitcher.SetNextPowerMode();
        CurrentStatuses.PowerMode = PowerMode.Pending;
    }

    private void MicSwitched()
    {
        var isEnabled = _multimediaHardwareService.SwitchMicState();
        CurrentStatuses.MicEnabled = isEnabled;
        OnMicSwitched?.Invoke(isEnabled);
    }

    public void OnKeyCombinationPressed(long[] keysPressed)
    {
        if (_hotKeysRouter.TryGetValue(keysPressed.Sum(), out var action))
            action.Invoke();
    }

    public void Dispose()
    {
        _updateTimer.Dispose();
        _multimediaHardwareService.Dispose();
        _keyboardHook.Dispose();
    }
}