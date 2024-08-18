namespace MiHotkeys.Services.MiDevice.Commands;

public class GetChargingStateQuery : IMiCommand
{
    public string  Method => CommandsList.GetChargingState;
    public object? Params { get; }= null;
}