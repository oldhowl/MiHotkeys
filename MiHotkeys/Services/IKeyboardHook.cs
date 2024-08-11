namespace MiHotkeys.Services;

public interface IKeyboardHook : IDisposable
{
    event Action<long[]>? KeyCombinationPressed;
}