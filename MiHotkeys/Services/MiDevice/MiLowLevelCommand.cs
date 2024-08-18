using Newtonsoft.Json;

namespace MiHotkeys.Services.MiDevice;

public class MiLowLevelCommand
{
    [JsonProperty("method")] public string Method { get; set; }
    [JsonProperty("params")] public object Params { get; set; }
}