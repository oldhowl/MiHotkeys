namespace MiHotkeys.Services.MiDevice.Commands;

public class SetChargingProtectCommand : IMiCommand
{
    public bool   Enable { get; }
    public string Method => CommandsList.SetChargingProtect;
    public object Params { get; }

    public SetChargingProtectCommand(bool enable)
    {
        Enable = enable;
        Params = new Dictionary<string, object>()
        {
            ["mode"] = enable ? 1 : 0
        };
    }
}