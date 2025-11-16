# ItemEyez

ItemEyez is a WPF application for organizing items in rooms and containers. The project targets **.NET 8** and uses SQL Server Express for data storage.

## Building

1. Restore NuGet packages and build the application in Release mode:

   ```bash
   dotnet build item-eyez.sln -c Release
   ```

2. Compile the WiX installer using the WiX Toolset (v4 is recommended). Run from the repository root:

   ```bash
   candle installer/Product.wxs -o ItemEyez.wixobj
   light ItemEyez.wixobj -o ItemEyez.msi
   candle installer/Bundle.wxs -o Bundle.wixobj
   light Bundle.wixobj -o ItemEyezInstaller.exe
   ```

   These commands are wrapped in `installer/build-installer.sh` which builds the
   application and creates the installer in one step:

   ```bash
   ./installer/build-installer.sh
   ```

   The resulting `ItemEyezInstaller.exe` will install SQL Server Express if it is not present, and then install or upgrade ItemEyez.

## Installer Behavior

- Detects a SQL Server Express instance named `SQLEXPRESS`. If missing, SQL Server Express is installed silently.
- Supports upgrades when a previous version of ItemEyez is already installed.

