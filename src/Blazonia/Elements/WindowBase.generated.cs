// <auto-generated>
//     This code was generated by a BlazorBindingsAvalonia component generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>



#pragma warning disable CA2252

namespace Blazonia.Components
{
    /// <summary>
    /// Base class for top-level windows.
    /// </summary>
    public abstract partial class WindowBase : TopLevel
    {
        static WindowBase()
        {
            RegisterAdditionalHandlers();
        }

        /// <summary>
        /// Gets or sets whether this window appears on top of all other windows
        /// </summary>
        [Parameter] public bool? Topmost { get; set; }
        [Parameter] public EventCallback OnActivated { get; set; }
        [Parameter] public EventCallback OnDeactivated { get; set; }
        [Parameter] public EventCallback<AC.PixelPointEventArgs> OnPositionChanged { get; set; }
        [Parameter] public EventCallback<AC.WindowResizedEventArgs> OnResized { get; set; }

        public new AC.WindowBase NativeControl => (AC.WindowBase)((AvaloniaObject)this).NativeControl;


        protected override void HandleParameter(string name, object value)
        {
            switch (name)
            {
                case nameof(Topmost):
                    if (!Equals(Topmost, value))
                    {
                        Topmost = (bool?)value;
                        NativeControl.Topmost = Topmost ?? (bool)AC.WindowBase.TopmostProperty.GetDefaultValue(AC.WindowBase.TopmostProperty.OwnerType);
                    }
                    break;
                case nameof(OnActivated):
                    if (!Equals(OnActivated, value))
                    {
                        void NativeControlActivated(object sender, EventArgs e) => InvokeEventCallback(OnActivated);

                        OnActivated = (EventCallback)value;
                        NativeControl.Activated -= NativeControlActivated;
                        NativeControl.Activated += NativeControlActivated;
                    }
                    break;
                case nameof(OnDeactivated):
                    if (!Equals(OnDeactivated, value))
                    {
                        void NativeControlDeactivated(object sender, EventArgs e) => InvokeEventCallback(OnDeactivated);

                        OnDeactivated = (EventCallback)value;
                        NativeControl.Deactivated -= NativeControlDeactivated;
                        NativeControl.Deactivated += NativeControlDeactivated;
                    }
                    break;
                case nameof(OnPositionChanged):
                    if (!Equals(OnPositionChanged, value))
                    {
                        void NativeControlPositionChanged(object sender, AC.PixelPointEventArgs e) => InvokeEventCallback(OnPositionChanged, e);

                        OnPositionChanged = (EventCallback<AC.PixelPointEventArgs>)value;
                        NativeControl.PositionChanged -= NativeControlPositionChanged;
                        NativeControl.PositionChanged += NativeControlPositionChanged;
                    }
                    break;
                case nameof(OnResized):
                    if (!Equals(OnResized, value))
                    {
                        void NativeControlResized(object sender, AC.WindowResizedEventArgs e) => InvokeEventCallback(OnResized, e);

                        OnResized = (EventCallback<AC.WindowResizedEventArgs>)value;
                        NativeControl.Resized -= NativeControlResized;
                        NativeControl.Resized += NativeControlResized;
                    }
                    break;

                default:
                    base.HandleParameter(name, value);
                    break;
            }
        }

        static partial void RegisterAdditionalHandlers();
    }
}
