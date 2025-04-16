using Blazonia.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazonia.ComponentGenerator.TypeConverter;
public class ControlThemeTypeConverter : BaseTypeConverter
{
    public override string GetBlazorTypeFullName(string ComponentType)
    {
        return $"OneOf.OneOf<{ComponentType}, string>";
    }

    public override string GetHandleValueProperty(string ComponentTypeName, string AvaloniaPropertyName, string propName)
    {
        return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = (OneOf.OneOf<{ComponentTypeName}, string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentTypeName.Replace("?", "")}){propName}.AsT0;
                        }}
                        else 
                        {{

                            NativeControl.{AvaloniaPropertyName} =   global::Avalonia.Controls.ResourceNodeExtensions.FindResource(global::Avalonia.Application.Current, {propName}.AsT1) as global::Avalonia.Styling.ControlTheme;
                        }}
                    }}
                    break;
";
    }

    public override bool ShouldConvert(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetFullName() == "Avalonia.Styling.ControlTheme";
    }
}
