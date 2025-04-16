using Blazonia.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;
using System;

namespace Blazonia.ComponentGenerator.TypeConverter;
public class IBrushTypeConverter : BaseTypeConverter
{
    public override string GetBlazorTypeFullName(string ComponentType)
    {
        return $"OneOf.OneOf<{ComponentType}, global::Avalonia.Media.Color, string>";
    }

    public override string GetHandleValueProperty(string ComponentTypeName, string AvaloniaPropertyName, string propName)
    {
        return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = (OneOf.OneOf<{ComponentTypeName}, Avalonia.Media.Color, string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentTypeName.Replace("?", "")}){propName}.AsT0;
                        }}
                        else if ({propName}.IsT1)
                        {{
                            NativeControl.{AvaloniaPropertyName} = new global::Avalonia.Media.Immutable.ImmutableSolidColorBrush({propName}.AsT1);
                        }}
                        else 
                        {{
                            NativeControl.{AvaloniaPropertyName} = Avalonia.Media.Brush.Parse({propName}.AsT2);
                        }}
                    }}
                    break;
";
    }

    public override bool ShouldConvert(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetFullName() == "Avalonia.Media.IBrush";
    }
}
