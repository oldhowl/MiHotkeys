@echo off

echo [DEBUG] Checking for administrator privileges...
net session >nul 2>&1
echo [DEBUG] Privilege check errorLevel: %errorLevel%
if %errorLevel% neq 0 (
    echo [DEBUG] Requesting administrator privileges...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit
)
:: Set variables
set ServiceName=MiDeviceService
set ServiceDisplayName=MiDeviceService
set ServiceExecutablePath=%~dp0miService\MiDeviceService.exe

:: Check if service exists
powershell -Command "try { $service = Get-Service -Name '%ServiceName%' -ErrorAction Stop; } catch { exit /b 1 }"

if %errorlevel% neq 0 (
    echo Service does not exist, creating it...
    :: Create the service
    powershell -Command "New-Service -Name '%ServiceName%' -BinaryPathName '%ServiceExecutablePath%' -DisplayName '%ServiceDisplayName%' -StartupType Automatic"
  
    echo Service created successfully.
)

:: Check service status and start it if not running
powershell -Command "if ((Get-Service -Name '%ServiceName%').Status -ne 'Running') { Start-Service -Name '%ServiceName%'; (Get-Service -Name '%ServiceName%' | Out-Null) }"

echo Service started successfully.
