using Avalonia.Controls;

namespace BlazorBindingsAvalonia.Elements;

public partial class StyledElement
{
    [Parameter] public string Classes { get; set; }
    
    protected override bool HandleAdditionalParameter(string name, object value)
    {
        if (name == nameof(Classes))
        {
            if (!Equals(Classes, value))
            {
                NativeControl.Classes.Replace(Avalonia.Controls.Classes.Parse((string)value));
            }
            return true;
        }
        
        return base.HandleAdditionalParameter(name, value);
    }
}
