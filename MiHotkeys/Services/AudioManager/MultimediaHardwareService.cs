namespace MiHotkeys.Services.AudioManager;

using NAudio.CoreAudioApi;

public class MultimediaHardwareService : IDisposable
{
    private readonly MMDeviceEnumerator _deviceEnumerator;
    private          MMDevice?          _microphone;
    private          bool               _micEnabled;

    public MultimediaHardwareService()
    {
        _deviceEnumerator = new MMDeviceEnumerator();
        InitializeMicrophone();
    }

    private void InitializeMicrophone()
    {
        _microphone = GetPrimaryMicrophone();
        if (_microphone != null)
        {
            _micEnabled                                          =  !_microphone.AudioEndpointVolume.Mute;
            _microphone.AudioEndpointVolume.OnVolumeNotification += OnVolumeNotification;
        }
    }

    private void OnVolumeNotification(AudioVolumeNotificationData data)
    {
        _micEnabled = _microphone?.AudioEndpointVolume.Mute == false;
    }

    public bool IsMicEnabled() => _microphone?.AudioEndpointVolume.Mute == false;

    public bool SwitchMicState()
    {
        if (_microphone != null)
        {
            _micEnabled                          = !_micEnabled;
            _microphone.AudioEndpointVolume.Mute = !_micEnabled;
        }

        return _micEnabled;
    }

    private MMDevice? GetPrimaryMicrophone()
    {
        try
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void Dispose()
    {
        if (_microphone != null)
        {
            _microphone.AudioEndpointVolume.OnVolumeNotification -= OnVolumeNotification;
            _microphone.Dispose();
        }

        _deviceEnumerator.Dispose();
    }
}