# Screen Capture Library

A lightweight C# WinForms library for capturing screenshots with a visual selection tool. Features DPI-aware screen capture and automatic clipboard integration.

## Features

- 🖼️ **Full-screen capture** - Captures the current monitor
- ✂️ **Visual selection tool** - Draw a rectangle to select specific areas
- 📋 **Clipboard integration** - Automatically copies selection to clipboard
- 🎯 **DPI-aware** - Works correctly with Windows scaling (125%, 150%, 200%, etc.)
- 📦 **Easy integration** - Simple API for quick implementation
- ⚡ **Lightweight** - Minimal dependencies, pure WinForms

## Requirements

- .NET Framework 4.7.2 or higher
- Windows 7 or later
- WinForms project

## Installation

### NuGet Package (coming soon)
```bash
Install-Package ScreenCaptureLib
```

### Manual Installation
1. Download the latest `.dll` from [Releases](../../releases)
2. Add reference to your project
3. Add `using ScreenCaptureLib;` to your code

### Build from Source
```bash
git clone https://github.com/yourusername/screen-capture-library.git
cd screen-capture-library
```

Open the solution in Visual Studio and build the project. The DLL will be in `bin/Release/`.

## Usage

### Basic Example

```csharp
using ScreenCaptureLib;

// Open the capture window
Capture captureForm = new Capture();
captureForm.ShowDialog();

// The selected area is automatically copied to clipboard
// User can now paste it anywhere with Ctrl+V
```

### Select screen Example

```csharp
private void btnScreenshot_Click(object sender, EventArgs e)
{
    // Hide your main form (optional)
    this.Hide();
    
    // Show capture dialog
    using (Capture captureForm = new Capture(1)) // you need to add the index of the screen the default value is -1
    {
        captureForm.ShowDialog();
    }
    
    // Show your main form again
    this.Show();
    
    // The screenshot is now in the clipboard
    MessageBox.Show("Screenshot copied to clipboard!");
}
```


## How It Works

### DPI Scaling Detection
The application automatically detects Windows display scaling using the `GetDeviceCaps` API:

```csharp
private float GetScalingFactor()
{
    Graphics g = this.CreateGraphics();
    IntPtr desktop = g.GetHdc();
    
    int logicalScreenHeight = GetDeviceCaps(desktop, VERTRES);
    int physicalScreenHeight = GetDeviceCaps(desktop, DESKTOPVERTRES);
    
    float scalingFactor = (float)physicalScreenHeight / logicalScreenHeight;
    
    g.ReleaseHdc(desktop);
    g.Dispose();
    
    return scalingFactor;
}
```

### Screen Capture Process
1. Detects the current monitor where the form is displayed
2. Calculates actual pixel dimensions accounting for DPI scaling
3. Captures the screen using `Graphics.CopyFromScreen()`
4. Displays in a fullscreen, borderless window
5. Converts mouse coordinates to actual image coordinates for accurate cropping


### Key Components

- **WinForms** - UI framework
- **GDI+** - Graphics rendering and screen capture
- **Win32 API** - DPI detection via `GetDeviceCaps`



**Properties:**
- Automatically handles DPI scaling
- Captures to clipboard on selection
- Fullscreen, borderless overlay

## Configuration



## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have questions:
- Open an [issue](../../issues)
- Check existing [discussions](../../discussions)

---

