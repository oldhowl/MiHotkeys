// using System.Runtime.InteropServices;
//
// namespace MiHotkeys.Services.DisplayManager;
//
// public class NativeMethods
// {
//     #region Fields
//
//     public const  int  EnumCurrentSettings             = -1;
//     public const  int  EnumRegistrySettings            = -2;
//     public const  int  WmPowerbroadcast                 = 0x0218;
//     public static Guid GuidBatteryPercentageRemaining = new Guid("A7AD8041-B45A-4CAE-87A3-EECBB468A9E1");
//
//     public static Guid GuidMonitorPowerOn =
//         new Guid(0x02731015, 0x4510, 0x4526, 0x99, 0xE6, 0xE5, 0xA1, 0x7E, 0xBD, 0x1A, 0xEA);
//
//     public static Guid GuidAcdcPowerSource =
//         new Guid(0x5D3E9A59, 0xE9D5, 0x4B00, 0xA6, 0xBD, 0xFF, 0x34, 0xFF, 0x51, 0x65, 0x48);
//
//     public static Guid GuidPowerschemePersonality =
//         new Guid(0x245D8541, 0x3943, 0x4422, 0xB0, 0x25, 0x13, 0xA7, 0x84, 0xF6, 0x79, 0xB7);
//
//     public static Guid GuidMaxPowerSavings =
//         new Guid(0xA1841308, 0x3541, 0x4FAB, 0xBC, 0x81, 0xF7, 0x15, 0x56, 0xF2, 0x0B, 0x4A);
//
//     // No Power Savings - Almost no power savings measures are used.
//     public static Guid GuidMinPowerSavings =
//         new Guid(0x8C5E7FDA, 0xE8BF, 0x4A96, 0x9A, 0x85, 0xA6, 0xE2, 0x3A, 0x8C, 0x63, 0x5C);
//
//     // Typical Power Savings - Fairly aggressive power savings measures are used.
//     public static Guid GuidTypicalPowerSavings =
//         new Guid(0x381B4222, 0xF694, 0x41F0, 0x96, 0x85, 0xFF, 0x5B, 0xB2, 0x60, 0xDF, 0x2E);
//
//     // Win32 decls and defs
//     const        int PbtApmquerysuspend          = 0x0000;
//     const        int PbtApmquerystandby          = 0x0001;
//     const        int PbtApmquerysuspendfailed    = 0x0002;
//     const        int PbtApmquerystandbyfailed    = 0x0003;
//     const        int PbtApmsuspend               = 0x0004;
//     const        int PbtApmstandby               = 0x0005;
//     const        int PbtApmresumecritical        = 0x0006;
//     const        int PbtApmresumesuspend         = 0x0007;
//     const        int PbtApmresumestandby         = 0x0008;
//     const        int PbtApmbatterylow            = 0x0009;
//     const        int PbtApmpowerstatuschange     = 0x000A; // power status
//     const        int PbtApmoemevent              = 0x000B;
//     const        int PbtApmresumeautomatic       = 0x0012;
//     const        int PbtPowersettingchange       = 0x8013; // DPPE
//     const        int DeviceNotifyWindowHandle  = 0x00000000;
//     const        int DeviceNotifyServiceHandle = 0x00000001;
//     public const int CdsUpdateregistry           = 0x01;
//     public const int DispChangeSuccessful       = 0;
//     public const int DmDisplayfrequency          = 0x00400000;
//
//     #endregion
//
//     #region Methods
//
//     [DllImport("kernel32.dll")]
//     public static extern uint GetLastError();
//
//     [DllImport("user32.dll", CharSet = CharSet.Ansi)]
//     public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref Devmode devMode);
//
//     [DllImport("User32.dll")]
//     [return: MarshalAs(UnmanagedType.I4)]
//     public static extern int ChangeDisplaySettings([In, Out] ref                        Devmode lpDevMode,
//                                                    [param: MarshalAs(UnmanagedType.U4)] uint    dwflags);
//
//     [DllImport("kernel32.dll", SetLastError = true)]
//     public static extern IntPtr LocalFree(IntPtr hMem);
//
//     [DllImport("powrprof.dll", EntryPoint = "PowerSetActiveScheme")]
//     public static extern uint PowerSetActiveScheme(IntPtr userPowerKey, ref Guid activePolicyGuid);
//
//     [DllImport("powrprof.dll", EntryPoint = "PowerGetActiveScheme")]
//     public static extern uint PowerGetActiveScheme(IntPtr userPowerKey, out IntPtr activePolicyGuid);
//
//     [DllImport("powrprof.dll", EntryPoint = "PowerReadFriendlyName")]
//     public static extern uint PowerReadFriendlyName(IntPtr rootPowerKey,                ref Guid schemeGuid,
//                                                     IntPtr subGroupOfPowerSettingsGuid, IntPtr   powerSettingGuid,
//                                                     IntPtr bufferPtr,                   ref uint bufferSize);
//
//     [DllImport("PowrProf.dll")]
//     public static extern UInt32 PowerEnumerate(IntPtr   rootPowerKey,               IntPtr     schemeGuid,
//                                                IntPtr   subGroupOfPowerSettingGuid, UInt32     acessFlags, UInt32 index,
//                                                ref Guid buffer,                     ref UInt32 bufferSize);
//
//     #endregion
//
//     #region Structs
//
//     [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
//     public struct Devmode
//     {
//         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
//         public string dmDeviceName;
//
//         public ushort dmSpecVersion;
//         public ushort dmDriverVersion;
//         public ushort dmSize;
//         public ushort dmDriverExtra;
//         public uint   dmFields;
//         public int    dmPositionX;
//         public int    dmPositionY;
//         public uint   dmDisplayOrientation;
//         public uint   dmDisplayFixedOutput;
//         public short  dmColor;
//         public short  dmDuplex;
//         public short  dmYResolution;
//         public short  dmTTOption;
//         public short  dmCollate;
//
//         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
//         public string dmFormName;
//
//         public ushort dmLogPixels;
//         public uint   dmBitsPerPel;
//         public uint   dmPelsWidth;
//         public uint   dmPelsHeight;
//         public uint   dmDisplayFlags;
//         public uint   dmDisplayFrequency;
//         public uint   dmICMMethod;
//         public uint   dmICMIntent;
//         public uint   dmMediaType;
//         public uint   dmDitherType;
//         public uint   dmReserved1;
//         public uint   dmReserved2;
//         public uint   dmPanningWidth;
//         public uint   dmPanningHeight;
//     };
//
//     #endregion
//
//     #region Enums
//
//     public enum AccessFlags : uint
//     {
//         AccessScheme             = 16,
//         AccessSubgroup           = 17,
//         AccessIndividualSetting = 18
//     }
//
//     #endregion
// }