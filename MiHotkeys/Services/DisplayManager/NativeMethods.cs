using System.Runtime.InteropServices;

namespace MiHotkeys.Services.DisplayManager;

public static class NativeMethods
{
    public const int EnumCurrentSettings  = -1;
    public const int CdsUpdateregistry    = 0x01;
    public const int DispChangeSuccessful = 0;
    public const int DmDisplayfrequency   = 0x00400000;

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref Devmode devMode);

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref Devmode lpDevMode, IntPtr hwnd,
                                                     uint   dwflags,        IntPtr      lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
                                                 uint   dwFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Devmode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public uint   dmFields;
        public int    dmPositionX;
        public int    dmPositionY;
        public uint   dmDisplayOrientation;
        public uint   dmDisplayFixedOutput;
        public short  dmColor;
        public short  dmDuplex;
        public short  dmYResolution;
        public short  dmTTOption;
        public short  dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public ushort dmLogPixels;
        public uint   dmBitsPerPel;
        public uint   dmPelsWidth;
        public uint   dmPelsHeight;
        public uint   dmDisplayFlags;
        public uint   dmDisplayFrequency;
        public uint   dmICMMethod;
        public uint   dmICMIntent;
        public uint   dmMediaType;
        public uint   dmDitherType;
        public uint   dmReserved1;
        public uint   dmReserved2;
        public uint   dmPanningWidth;
        public uint   dmPanningHeight;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)] public int cb;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;

        [MarshalAs(UnmanagedType.U4)] public DisplayDeviceStateFlags StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    [Flags]
    public enum DisplayDeviceStateFlags
    {
        AttachedToDesktop = 0x1,
        MultiDriver       = 0x2,
        PrimaryDevice     = 0x4,
        MirroringDriver   = 0x8,
        VGACompatible     = 0x10,
        Removable         = 0x20,
        ModesPruned       = 0x8000000,
        Remote            = 0x4000000,
        Disconnect        = 0x2000000
    }
}