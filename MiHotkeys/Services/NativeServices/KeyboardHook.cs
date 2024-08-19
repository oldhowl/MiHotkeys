using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using MiHotkeys.Common;
using Timer = System.Threading.Timer;

namespace MiHotkeys.Services.NativeServices;

public class KeyboardHook : IKeyboardHook
{
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private          LowLevelKeyboardProc _proc;
    private          IntPtr               _hookId;
    private readonly Timer                _hookCheckTimer;

    private readonly HashSet<long> _pressedKeys         = new();
    private readonly List<long>    _releasedCombination = new();

    private bool _miButtonPressed = false; // Флаг для отслеживания состояния Mi-кнопки

    public event Action<long[]>? KeyCombinationPressed;

    public KeyboardHook()
    {
        _proc                         =  HookCallback;
        _hookId                       =  SetHook(_proc);
        SystemEvents.PowerModeChanged += OnPowerModeChanged;

        _hookCheckTimer = new Timer(CheckHook, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Resume)
        {
            Dispose();
            _proc   = HookCallback;
            _hookId = SetHook(_proc);
        }
    }

    private void CheckHook(object? sender)
    {
        if (_hookId == IntPtr.Zero)
        {
            _hookId = SetHook(_proc);
            Debug.WriteLine("Keyboard hook reinitialized.");
        }
    }

    protected virtual async void Dispose(bool disposing)
    {
        if (!disposing) return;

        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        await _hookCheckTimer.DisposeAsync();

        if (_hookId != IntPtr.Zero)
        {
            if (!UnhookWindowsHookEx(_hookId))
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode,
                    $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(errorCode).Message}.");
            }

            _hookId = IntPtr.Zero;
        }

        _proc -= HookCallback;
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule  = curProcess.MainModule;
        return SetWindowsHookEx(WhKeyboardLl, proc,
            GetModuleHandle(curModule?.ModuleName ?? throw new InvalidOperationException()), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return CallNextHookEx(_hookId, nCode, wParam, lParam);

        var vkCode = Marshal.ReadInt64(lParam);

        switch (wParam)
        {
            case WmKeydown:
                if (_pressedKeys.Add(vkCode))
                {
                    _releasedCombination.Add(vkCode);

                    if (vkCode == KeysConstants.MiButtonCode)
                    {
                        _miButtonPressed = true; // Mi-кнопка нажата
                    }

                    if (_miButtonPressed)
                    {
                        // Если нажата Mi-кнопка, блокируем передачу событий другим приложениям
                        return (IntPtr)1; 
                    }
                }

                break;
            case WmKeyup:
                if (_pressedKeys.Remove(vkCode))
                {
                    if (vkCode == KeysConstants.MiButtonCode)
                    {
                        _miButtonPressed = false; // Mi-кнопка отпущена
                        KeyCombinationPressed?.Invoke(_releasedCombination.ToArray());
                        _releasedCombination.Clear();
                    }

                    // Если отпущены все клавиши
                    if (_pressedKeys.Count == 0)
                    {
                        _releasedCombination.Clear();
                    }
                }
                
                if (!_miButtonPressed)
                {
                    // Если Mi-кнопка отпущена, позволяем другим приложениям обрабатывать события
                    return CallNextHookEx(_hookId, nCode, wParam, lParam);
                }
                
                break;
        }

        return _miButtonPressed ? (IntPtr)1 : CallNextHookEx(_hookId, nCode, wParam, lParam); // Блокируем остальные события, если Mi-кнопка удерживается
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~KeyboardHook()
    {
        Dispose(false);
    }

    private const int WhKeyboardLl = 13;
    private const int WmKeydown    = 0x0100;
    private const int WmKeyup      = 0x0101;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
