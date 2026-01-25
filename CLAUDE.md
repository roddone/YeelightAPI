# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

YeelightAPI is a C# (.NET) library for controlling Xiaomi Yeelight smart bulbs over LAN via TCP. It implements the "Yeelight WiFi Light Inter-Operation Specification" protocol using SSDP for device discovery.

## Build Commands

```bash
# Restore and build
dotnet restore
dotnet build

# Build release
dotnet build --configuration Release

# Run tests (requires actual Yeelight devices on network)
dotnet test

# Pack NuGet package
dotnet pack YeelightAPI/YeelightAPI.csproj --configuration Release

# Run console demo app
dotnet run --project YeelightAPI.Console/YeelightAPI.Console.csproj
```

## Architecture

### Solution Structure
- **YeelightAPI/** - Core library (NuGet package), multi-targets .NET Standard 2.0/2.1, .NET Framework 4.5-4.8, .NET Core 2.0-3.1, .NET 5.0-6.0
- **YeelightAPI.Console/** - Demo console application showing API usage
- **YeelightAPI.UnitTests/** - XUnit integration tests

### Key Classes
- `Device` - Main class for single bulb control. Manages TCP connection, command execution, and event handling. Split across partial classes: `Device.cs`, `Device.IDeviceController.cs`, `Device.IBackgroundDeviceController.cs`
- `DeviceGroup` - Extends `List<Device>` for controlling multiple devices in parallel with the same `IDeviceController` interface
- `DeviceLocator` - Static class for SSDP-based device discovery with `DiscoverAsync()` and `IProgress<Device>` support

### Protocol Mapping
- `Models/Methods.cs` - `METHODS` enum with `[RealName]` attributes mapping to device protocol commands (e.g., `[RealName("set_rgb")] SetRGBColor`)
- `Models/Properties.cs` - `PROPERTIES` enum for device state attributes
- Commands are JSON-serialized via System.Text.Json and tracked with unique IDs in a concurrent dictionary

### Color Flow System
Two approaches available:
1. Traditional: Create `ColorFlow` with `ColorFlowExpression` objects
2. Fluent: Use `device.Flow().RgbColor().Sleep().Temperature().Play()` chain via `FluentFlow` class

### Events
- `OnNotificationReceived` - Device state change notifications
- `OnError` - Error handling

## Testing

Tests are XUnit-based integration tests that require actual Yeelight devices:
- `YeelightAPI.UnitTests/DeviceTests.cs` - Control operations
- `YeelightAPI.UnitTests/DiscoveryTests.cs` - Device discovery
- Configure expected device count in `YeelightAPI.UnitTests/config.json`
- Tests run single-threaded (configured in `xunit.runner.json`)

## Important Notes

- Devices must have "developer mode" enabled to be discoverable/controllable
- All public APIs are async (`Task`/`Task<T>`) - use `await` consistently
- Device uses sync locks for thread-safe command tracking
- Discovery uses SSDP multicast (default: 239.255.255.250)
