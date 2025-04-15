using Microsoft.AspNetCore.Components.Routing;

namespace Blazonia.UriNavigation;

internal class BlazorNavigationInterception : INavigationInterception
{
    public Task EnableNavigationInterceptionAsync() => Task.CompletedTask;
}
