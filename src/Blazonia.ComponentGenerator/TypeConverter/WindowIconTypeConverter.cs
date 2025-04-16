using Blazonia.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazonia.ComponentGenerator.TypeConverter;
public class WindowIconTypeConverter : BaseTypeConverter
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
                        {propName} = (OneOf.OneOf<{ComponentTypeName},string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentTypeName.Replace("?", "")}){propName}.AsT0;
                        }}
                        else 
                        {{
                            NativeControl.{AvaloniaPropertyName} = new global::Avalonia.Controls.WindowIcon(global::Avalonia.Platform.AssetLoader.Open(new Uri({propName}.AsT1)));
                        }}
                    }}
                    break;
";
    }

    public override bool ShouldConvert(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetFullName() == "Avalonia.Controls.WindowIcon";
    }
}
