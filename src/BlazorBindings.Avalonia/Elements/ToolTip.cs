// <auto-generated>
//     This code was generated by a BlazorBindings.Avalonia component generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>



#pragma warning disable CA2252

using Avalonia.Controls;
using System.Runtime.CompilerServices;

namespace BlazorBindings.AvaloniaBindings.Elements
{
    internal static class ToolTipInitializer
    {
        [ModuleInitializer]
        internal static void RegisterAdditionalHandlers()
        {
            AttachedPropertyRegistry.RegisterAttachedPropertyHandler("ToolTip.Tip",
                (element, value) => AC.ToolTip.SetTip((AC.Control)element, value));

            AttachedPropertyRegistry.RegisterAttachedPropertyHandler("ToolTip.Placement",
                (element, value) => AC.ToolTip.SetPlacement((AC.Control)element, (PlacementMode)value));
        }
    }

    /// <summary>
    /// A control which pops up a hint when a control is hovered.
    /// </summary>
    public partial class ToolTip : ContentControl
    {
        
    }
}
