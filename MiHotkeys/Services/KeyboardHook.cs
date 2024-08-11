using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MiHotkeys.Services;

public class KeyboardHook :  IKeyboardHook
{
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private readonly IntPtr _hookId;

    private readonly HashSet<long> _pressedKeys         = new();
    private readonly List<long>    _releasedCombination = new();

    public event Action<long[]>? KeyCombinationPressed;

    public KeyboardHook()
    {
        _hookId = SetHook(HookCallback);
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
        if (nCode >= 0)
        {
            var vkCode = Marshal.ReadInt64(lParam);

            if (wParam == WmKeydown)
            {
                if (_pressedKeys.Add(vkCode))
                {
                    _releasedCombination.Add(vkCode);
                }
            }
            else if (wParam == WmKeyup)
            {
                if (_pressedKeys.Remove(vkCode))
                {
                    if (_pressedKeys.Count == 0)
                    {
                        if (_releasedCombination.Count > 0)
                        {
                            KeyCombinationPressed?.Invoke(_releasedCombination.ToArray());
                            _releasedCombination.Clear();
                        }
                    }
                }
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }


    public void Dispose()
    {
        UnhookWindowsHookEx(_hookId);
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