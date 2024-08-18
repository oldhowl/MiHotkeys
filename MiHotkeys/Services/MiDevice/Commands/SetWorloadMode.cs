namespace MiHotkeys.Services.MiDevice.Commands;

public class SetWorkLoadMode : IMiCommand
{
    public string  Method => CommandsList.SetWorkLoadMode;
    public object? Params { get; }


    public SetWorkLoadMode(int mode)
    {
        Params = new Dictionary<string, object>()
        {
            ["mode"] = mode
        };
    }
}