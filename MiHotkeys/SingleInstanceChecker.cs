namespace MiHotkeys;

public static class SingleInstanceChecker
{
    private static Mutex? _mutex;
    private const  string MutexName = "MiHotkeysAppMutex";

    public static bool IsSingleInstance()
    {
        _mutex = new Mutex(true, nameof(MiHotkeys), out var isNewInstance);
        return isNewInstance;
    }

    public static void Release()
    {
        _mutex?.ReleaseMutex();
        _mutex = null;
    }
}