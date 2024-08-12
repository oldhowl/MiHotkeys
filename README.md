
# MiHotkeys

MiHotkeys is a Windows Forms application designed for quick access to system settings using hotkeys. The app allows users to switch power modes and adjust display refresh rates via predefined keyboard shortcuts.

## Features

- **Power Mode Switching**: Toggle between Silence, Balance, and Max Power modes using the `Mi` button.
- **Display Refresh Rate Switching**: Change between 60Hz, 120Hz, and 165Hz with `Mi+F12`.
- **System Tray Integration**: Displays current settings in the system tray.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/oldhowl/MiHotkeys.git
   cd MiHotkeys
   ```
2. Build the project:
   ```bash
   dotnet build
   ```
3. Publish as a single executable:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
   ```

## Usage

- **Run on Startup**: Toggle auto-start from the system tray menu.
- **Exiting**: Right-click the tray icon and select "Exit."

## Contributions

Contributions are welcome! Open an issue or submit a pull request.

