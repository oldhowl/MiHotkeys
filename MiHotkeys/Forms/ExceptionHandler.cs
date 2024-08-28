namespace MiHotkeys.Forms;

public static class ExceptionHandler
{
    public static void Initialize()
    {
        Application.ThreadException += (sender, args) => { HandleException(args.Exception); };

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                                                      {
                                                          HandleException(args.ExceptionObject as Exception);
                                                      };

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    }

    private static void HandleException(Exception? ex)
    {
        if (ex == null) return;
        LogException(ex);
    }

    private static void LogException(Exception ex)
    {
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
        File.AppendAllText(logPath, $"{DateTime.Now}: {ex}\n");
    }
}