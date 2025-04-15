using Avalonia;
using Avalonia.Markup.Xaml;
using Blazonia.HelloWorld.Pages;

namespace Blazonia.HelloWorld;

public class App : AppBase
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
public class AppBase : BlazoniaApplication<MainPage>
{
}
