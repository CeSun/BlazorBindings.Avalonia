using Avalonia;
using Avalonia.Markup.Xaml;
using BlazorBindingsAvalonia.HelloWorld.Pages;

namespace BlazorBindingsAvalonia.HelloWorld;

public class App : BlazorBindingsApplicationMainPage
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
public class BlazorBindingsApplicationMainPage : BlazorBindingsApplication<MainPage>
{
}
