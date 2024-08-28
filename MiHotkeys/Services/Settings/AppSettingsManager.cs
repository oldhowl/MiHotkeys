using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace MiHotkeys.Services.Settings;

public class AppSettingsManager
{
    private readonly string      _filePath;
    private readonly AppSettings _settings;

    public AppSettingsManager(string filePath = "appsettings.json")
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            _settings = new AppSettings();
            SaveSettings();
        }
        else
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(_filePath, optional: false, reloadOnChange: true);
            var configuration1 = builder.Build();

            _settings = new AppSettings();
            configuration1.Bind(_settings);
        }
    }


    public bool GetPowerLoadMonitorEnabled()
    {
        return _settings.PowerLoadMonitorEnabled;
    }

    public void SetPowerLoadMonitorEnabled(bool value)
    {
        if (value == _settings.PowerLoadMonitorEnabled)
            return;

        _settings.PowerLoadMonitorEnabled = value;
        SaveSettings();
    }

    public int GetPowerMode()
    {
        return _settings.PowerMode;
    }

    public void SetPowerMode(int value)
    {
        if (_settings.PowerMode == value)
            return;

        _settings.PowerMode = value;
        SaveSettings();
    }

    private void SaveSettings()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(_settings, jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public void SetMicrophone(bool isEnabled)
    {
        _settings.MicrophoneEnabled = isEnabled;
        SaveSettings();
    }

    public bool GetMicrophoneState()
    {
        return _settings.MicrophoneEnabled;
    }

    public bool GetChargingProtectionStatus()
    {
        return _settings.ChargingProtectionEnabled;
    }

    public void SetChargingProtectionStatus(bool value)
    {
        _settings.ChargingProtectionEnabled = value;
        SaveSettings();
    }
}