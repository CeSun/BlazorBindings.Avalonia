using Blazonia.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazonia.ComponentGenerator.TypeConverter;
public class ThicknessTypeConverter : BaseTypeConverter
{
    public override string GetBlazorTypeFullName(string ComponentType)
    {
        return $"OneOf.OneOf<{ComponentType}, double, (double, double), (double, double, double, double), string>";
    }

    public override string GetHandleValueProperty(string ComponentTypeName, string AvaloniaPropertyName, string propName)
    {
        var type = "global::Avalonia.Thickness";

        return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = (OneOf.OneOf<{ComponentTypeName}, double, (double, double), (double, double, double, double), string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentTypeName.Replace("?", "")}){propName}.AsT0;
                        }}
                        else if ({propName}.IsT1)
                        {{
                            NativeControl.{AvaloniaPropertyName} = new {type}({propName}.AsT1);
                        }}
                        else if ({propName}.IsT2)
                        {{
                            NativeControl.{AvaloniaPropertyName} = new {type}({propName}.AsT2.Item1,{propName}.AsT2.Item2);
                        }}
                        else if ({propName}.IsT3)
                        {{
                            NativeControl.{AvaloniaPropertyName} = new {type}({propName}.AsT3.Item1,{propName}.AsT3.Item2,{propName}.AsT3.Item3,{propName}.AsT3.Item4);
                        }}
                        else 
                        {{
                            NativeControl.{AvaloniaPropertyName} = {type}.Parse({propName}.AsT4);
                        }}
                    }}
                    break;
";
    }

    public override bool ShouldConvert(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetFullName() == "Avalonia.Thickness";
    }
}
