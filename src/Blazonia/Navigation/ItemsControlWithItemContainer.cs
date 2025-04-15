using Avalonia.Controls;

namespace Blazonia.Navigation;

public class ItemsControlWithItemContainer : ItemsControl
{
    protected override bool NeedsContainerOverride(object item, int index, out object recycleKey)
    {
        recycleKey = DefaultRecycleKey;
        return true;
    }
}