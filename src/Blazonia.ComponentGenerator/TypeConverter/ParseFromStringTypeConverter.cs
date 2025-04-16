using Blazonia.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blazonia.ComponentGenerator.TypeConverter;
public class ParseFromStringTypeConverter : BaseTypeConverter
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
                            NativeControl.{AvaloniaPropertyName} = {ComponentTypeName.Replace("?", "")}.Parse({propName}.AsT1);
                        }}
                    }}
                    break;
";
    }

    public override bool ShouldConvert(INamedTypeSymbol typeSymbol)
    {
        var IsCanParseFromString = false;
        var parameterType = typeSymbol;
        var parameterNamepace = parameterType.ContainingNamespace;
        if (parameterNamepace.ToDisplayString() != "System")
        {
            var parseMethod = parameterType.GetMethod("Parse");
            if (parseMethod != null)
            {
                if (parseMethod.Parameters.Count() == 1 && parseMethod.Parameters[0].Type.GetFullName() == "System.String")
                {
                    IsCanParseFromString = true;
                }
            }
        }
        return IsCanParseFromString;
    }
}
