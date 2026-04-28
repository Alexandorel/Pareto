using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;

BuildAvaloniaApp().Start(AppMain, args);

static void AppMain(Application app, string[] args)
{
    // Main windows
    var window = new Window
    {
        Title = "Pareto App pe macOS",
        Width = 800,
        Height = 800
    };

    app.Run(window);
}

// Service configuration for Avalonia.
static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<Application>()
              .UsePlatformDetect()
              .LogToTrace();
