using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MiHotkeys.Services;

public class KeyboardHook : IKeyboardHook
{
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private LowLevelKeyboardProc _proc;
    private IntPtr               _hookId;

    private readonly HashSet<long> _pressedKeys         = new();
    private readonly List<long>    _releasedCombination = new();

    public event Action<long[]>? KeyCombinationPressed;

    public KeyboardHook()
    {
        _proc   = HookCallback;
        _hookId = SetHook(_proc);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_hookId != IntPtr.Zero)
            {
                if (!UnhookWindowsHookEx(_hookId))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode,
                        $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(errorCode).Message}.");
                }

                _hookId = IntPtr.Zero;
            }

            _proc -= HookCallback;
        }
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
                    _releasedCombination.Add(vkCode);
                break;
            case WmKeyup:
                if (_pressedKeys.Remove(vkCode))
                {
                    if (_pressedKeys.Count == 0 && _releasedCombination.Count > 0)
                    {
                        var combination = _releasedCombination.ToArray();
                        _releasedCombination.Clear();
                        Task.Run(() => KeyCombinationPressed?.Invoke(combination));
                    }
                }

                break;
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
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
