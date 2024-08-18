namespace MiHotkeys.Services.MiDevice.Commands;

public class GetChargingProtectQuery : IMiCommand
{
    public string  Method => CommandsList.GetChargingProtect;
    public object? Params { get; }= null;
}