namespace MiHotkeys.Services.MiDevice;

public class MiResponse<TResponse>
{
    public int       Code     { get; set; }
    public TResponse Data { get; set; }
    public string    Message  { get; set; }
}