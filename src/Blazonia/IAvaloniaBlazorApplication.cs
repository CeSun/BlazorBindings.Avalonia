using Blazonia.Navigation;

namespace Blazonia;

public interface IAvaloniaBlazorApplication
{
    IServiceProvider ServiceProvider { get; }

    void Initialize(IServiceProvider serviceProvider);

    AvaloniaNavigation Navigation { get; }
}
