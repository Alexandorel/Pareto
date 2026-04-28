using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using ParetoApp;

BuildAvaloniaApp().Start(AppMain, args);

static void AppMain(Application app, string[] args)
{
    var panou = new StackPanel
    {
        Margin = new Avalonia.Thickness(20),
        Spacing = 15
    };

    panou.Children.Add(new TextBlock { Text = "Bine ai venit în aplicația Pareto!", FontSize = 24 });
    panou.Children.Add(new TextBlock { Text = "Numele tău:" });
    panou.Children.Add(new TextBox { Watermark = "Scrie aici..." });
    panou.Children.Add(new Button { Content = "Trimite" });

    // left menu
    var meniu = new StackPanel { Margin = new Avalonia.Thickness(10), Spacing = 10 };
    meniu.Children.Add(new Button { Content = "General", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch, CornerRadius = new Avalonia.CornerRadius(8) });

    // split view
    var layoutPrincipal = new SplitView
    {
        DisplayMode = SplitViewDisplayMode.Inline,
        IsPaneOpen = true,
        OpenPaneLength = 200,
        Pane = meniu,
        Content = panou
    };

    // Main windows
    var window = new Window
    {
        Title = "Pareto",
        Width = 800,
        Height = 800,

        Content = layoutPrincipal
    };

    app.Run(window);
}

// Service configuration for Avalonia.
static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
              .UsePlatformDetect()
              .LogToTrace();
