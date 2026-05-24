# Beijer HMI Project - CameraCognex

This is an HMI (Human-Machine Interface) project built with **IX Developer** from Beijer Electronics.

## Project Overview

- **Project Name**: CameraCognex
- **Target Platform**: Windows
- **Startup Screen**: Camera
- **IX Developer Version**: 2.53.65422.0

## Project Purpose

HMI system for interfacing with Cognex cameras. The project integrates with `Cognex.DataMan.SDK.PC.dll` for camera communication and control.

## File Structure

### Core Project Files
- `CameraCognex.neoproj` - Main project file (do not edit manually)
- `Project.xml` - Project configuration and structure

### Screens (XAML-based)
- `Camera.xaml` / `Camera.neoxaml` - Main camera interface screen
- Location: Screens group in IX Developer

### Controllers & Communication
- `Controller1.neo` / `Controller1.cfg` - Controller configuration
- `Controllers.xml` - Controller definitions
- `Tags.neo` - Global tags and poll groups
- `Devices.xml` - Output devices (e.g., Printer1)

### Features & Services
- `AlarmServer.neo` / `AlarmSettings.xml` - Alarm management
- `Security.neo` / `Security.xml` - Security configuration
- `MultipleLanguages.neo` - Multi-language support
- `Expressions.neo` - Expression definitions
- `ProjectConfiguration.neo` - Project settings

### Build Files
- `BuildFiles/` - Generated build artifacts
- `Temp/` - Temporary files

## Working with IX Developer Files

### File Formats
- `.neoproj` - Project file (XML-based, managed by IX Developer)
- `.neo` - Binary/component files (managed by IX Developer)
- `.xaml` - Screen definitions (can be edited with caution)
- `.neoxaml` - Compiled XAML (do not edit)
- `.xml` - Configuration files
- `.cri` - Runtime/compiled files (auto-generated)

### What to Edit
- **XAML files** (`Camera.xaml`) - Screen layouts, controls (use IX Developer primarily)
- **Script files** (if any) - Code-behind logic
- **XML configurations** - Where appropriate

### What NOT to Edit Manually
- `.neoproj` - Use IX Developer IDE
- `.neo` files - Managed by IX Developer
- `.neoxaml` - Auto-generated
- `.cri` files - Compiled runtime files
- `BuildFiles/` contents

## Development Workflow

1. **Primary Development**: Use **IX Developer** IDE for:
   - Screen design and layout
   - Tag creation and configuration
   - Controller setup
   - Alarm configuration
   - Project compilation and deployment

2. **VS Code Usage**: Use for:
   - XAML editing (when needed)
   - Script/code editing
   - Version control (git)
   - Quick file inspection

3. **Building**: Use IX Developer's built-in build system

## Tags & Data Binding

- Tags are defined in `Tags.neo` (managed via IX Developer)
- Poll groups: PollGroup1-5 (500ms interval default)
- Controller: Controller1 for PLC/external device communication

## External Dependencies

- `ReferencedAssembly/Cognex.DataMan.SDK.PC.dll` - Cognex DataMan SDK for camera integration

## Notes

- Project uses SQLite storage provider
- Keyboard layout: US
- VNC Server: Disabled by default
- FTP Server: Disabled by default
