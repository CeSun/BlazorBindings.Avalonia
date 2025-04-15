using Microsoft.AspNetCore.Components.Routing;

namespace Blazonia.UriNavigation;

internal class BlazorScrollToLocationHash : IScrollToLocationHash
{
    public Task RefreshScrollPositionForHash(string locationAbsolute) => Task.CompletedTask;
}
