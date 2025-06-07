#!/usr/bin/env bash
# Build the ItemEyez installer using the WiX Toolset.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO_ROOT"

# Build the .NET solution in Release mode
if ! dotnet build item-eyez.sln -c Release; then
  echo "dotnet build failed" >&2
  exit 1
fi

# Compile and link the MSI
candle installer/Product.wxs -o installer/ItemEyez.wixobj
light installer/ItemEyez.wixobj -o installer/ItemEyez.msi

# Compile and link the bootstrapper EXE
candle installer/Bundle.wxs -o installer/Bundle.wixobj
light installer/Bundle.wixobj -o installer/ItemEyezInstaller.exe

echo "Installer built at installer/ItemEyezInstaller.exe"
