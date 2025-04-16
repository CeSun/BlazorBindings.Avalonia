using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazonia.ComponentGenerator.TypeConverter;
public abstract class BaseTypeConverter
{
    public static List<BaseTypeConverter> TypeConverters = new List<BaseTypeConverter>
    {
        new ControlThemeTypeConverter(),
        new CornerRadiusTypeConverter(),
        new IBrushTypeConverter(),
        new ThicknessTypeConverter(),
        new WindowIconTypeConverter(),
        new IImageTypeConverter(),
        new ParseFromStringTypeConverter()
    };
    public abstract bool ShouldConvert(INamedTypeSymbol typeSymbol);

    public abstract string GetBlazorTypeFullName(string ComponentType);

    public abstract string GetHandleValueProperty(string ComponentTypeName, string AvaloniaPropertyName, string propName);

}
