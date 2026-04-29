using Avalonia;
using System;
using ParetoApp;

BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

// Service configuration for Avalonia.
static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
              .UsePlatformDetect()
              .LogToTrace();
