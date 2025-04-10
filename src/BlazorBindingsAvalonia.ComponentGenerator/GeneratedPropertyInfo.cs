﻿using BlazorBindingsAvalonia.ComponentGenerator.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BlazorBindingsAvalonia.ComponentGenerator;

public partial class GeneratedPropertyInfo
{
    private readonly IPropertySymbol _propertyInfo;
    private Lazy<string> _componentPropertyNameLazy;
    private Lazy<string> _componentTypeLazy;

    public GeneratedTypeInfo ContainingType { get; set; }
    public GeneratedPropertyKind Kind { get; }
    public string AvaloniaPropertyName { get; set; }
    public string AvaloniaContainingTypeName { get; }
    public string ComponentName { get; }
    public Compilation Compilation { get; }
    public bool IsGeneric { get; }
    public INamedTypeSymbol GenericTypeArgument { get; }

    public bool IsCanParseFromString;

    public bool IsIBrush;

    public bool IsThickness;

    public bool IsCornerRadius;
    public string ComponentPropertyName
    {
        get => _componentPropertyNameLazy.Value;
        set => _componentPropertyNameLazy = new Lazy<string>(value);
    }

    public string ComponentType
    {
        get => _componentTypeLazy.Value;
        set => _componentTypeLazy = new Lazy<string>(value);
    }

    private GeneratedPropertyInfo(GeneratedTypeInfo typeInfo,
                                  string avaloniaPropertyName,
                                  string avaloniaContainingTypeName,
                                  string componentPropertyName,
                                  string componentType,
                                  GeneratedPropertyKind kind)
    {
        ContainingType = typeInfo;
        Compilation = typeInfo.Compilation;
        ComponentName = typeInfo.TypeName;
        Kind = kind;
        AvaloniaPropertyName = avaloniaPropertyName;
        AvaloniaContainingTypeName = avaloniaContainingTypeName;
        _componentTypeLazy = new Lazy<string>(componentType);
        _componentPropertyNameLazy = new Lazy<string>(componentPropertyName);
    }

    private GeneratedPropertyInfo(GeneratedTypeInfo typeInfo, IPropertySymbol propertyInfo, GeneratedPropertyKind kind)
    {
        
        _propertyInfo = propertyInfo;
        Kind = kind;
        if (typeInfo.TypeName.StartsWith("TabControl"))
        {

        }


        ContainingType = typeInfo;
        Compilation = typeInfo.Compilation;
        ComponentName = typeInfo.TypeName;
        AvaloniaPropertyName = ComponentWrapperGenerator.GetIdentifierName(propertyInfo.Name);
        IsGeneric = typeInfo.Settings.GenericProperties.TryGetValue(propertyInfo.Name, out var genericTypeArgument);
        GenericTypeArgument = genericTypeArgument;
        AvaloniaContainingTypeName = GetTypeNameAndAddNamespace(propertyInfo.ContainingType);

        ComponentName = ComponentWrapperGenerator.GetIdentifierName(_propertyInfo.ContainingType.Name);
        _componentPropertyNameLazy = new Lazy<string>(GetComponentPropertyName);
        _componentTypeLazy = new Lazy<string>(() => GetComponentPropertyTypeName(_propertyInfo, typeInfo, IsRenderFragmentProperty, makeNullable: true));

        IsCanParseFromString = false;
        var parameterType = ((INamedTypeSymbol)propertyInfo.Type);
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
        IsIBrush = false;
        if (parameterType.GetFullName() == "Avalonia.Media.IBrush")
        {
            IsIBrush = true;
        }

        IsThickness = false;

        if (parameterType.GetFullName() == "Avalonia.Thickness")
        {
            IsThickness = true;
        }
        IsCornerRadius = false;

        if (parameterType.GetFullName() == "Avalonia.CornerRadius")
        {
            IsCornerRadius = true;
        }
        string GetComponentPropertyName()
        {
            if (ContainingType.Settings.Aliases.TryGetValue(AvaloniaPropertyName, out var aliasName))
                return aliasName;

            if (IsRenderFragmentProperty && _propertyInfo.GetAttributes().Any(a
                => a.AttributeClass.Name == "ContentAttribute"))
            {
                return "ChildContent";
            }

            return ComponentWrapperGenerator.GetIdentifierName(_propertyInfo.Name);
        }
    }

    public string GetPropertyDeclaration()
    {
        // razor compiler doesn't allow 'new' properties, it considers them as duplicates.
        if (_propertyInfo is not null && _propertyInfo.IsHidingMember())
        {
            return "";
        }

        const string indent = "        ";

        var xmlDocContents = _propertyInfo is null ? "" : ComponentWrapperGenerator.GetXmlDocContents(_propertyInfo, indent);

        if (ComponentPropertyName == "HeaderTemplate")
        {
            return $@"{xmlDocContents}{indent}[Parameter] public {ComponentType}<object> {ComponentPropertyName} {{ get; set; }}
";

        }
        if (IsThickness || IsCornerRadius)
        {
            return $@"{xmlDocContents}{indent}[Parameter] public OneOf.OneOf<{ComponentType}, double, (double, double), (double, double, double, double), string> {ComponentPropertyName} {{ get; set; }}
";
        }
        if (IsCanParseFromString)
        {
            return $@"{xmlDocContents}{indent}[Parameter] public OneOf.OneOf<{ComponentType}, string> {ComponentPropertyName} {{ get; set; }}
";
        }
        if (IsIBrush)
        {
            return $@"{xmlDocContents}{indent}[Parameter] public OneOf.OneOf<{ComponentType}, global::Avalonia.Media.Color, string> {ComponentPropertyName} {{ get; set; }}
";
        }
        return $@"{xmlDocContents}{indent}[Parameter] public {ComponentType} {ComponentPropertyName} {{ get; set; }}
";
    }

    public string GetHandleValueProperty()
    {
        var propName = ComponentPropertyName;
        if (IsThickness || IsCornerRadius)
        {
            var type = IsThickness ? "global::Avalonia.Thickness" : "global::Avalonia.CornerRadius";

            return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = (OneOf.OneOf<{ComponentType}, double, (double, double), (double, double, double, double), string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentType.Replace("?", "")}){propName}.AsT0;
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
        if (IsCanParseFromString)
        {
            return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = (OneOf.OneOf<{ComponentType},string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentType.Replace("?", "")}){propName}.AsT0;
                        }}
                        else 
                        {{
                            NativeControl.{AvaloniaPropertyName} = {ComponentType.Replace("?", "")}.Parse({propName}.AsT1);
                        }}
                    }}
                    break;
";
        }
        if (IsIBrush == true)
        {

            return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = (OneOf.OneOf<{ComponentType}, Avalonia.Media.Color, string>)value;
                        if ({propName}.IsT0)
                        {{
                            NativeControl.{AvaloniaPropertyName} = ({ComponentType.Replace("?", "")}){propName}.AsT0;
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
        return $@"                case nameof({propName}):
                    if (!Equals({propName}, value))
                    {{
                        {propName} = ({ComponentType})value;
                        NativeControl.{AvaloniaPropertyName} = {GetConvertedProperty(_propertyInfo.Type, propName)};
                    }}
                    break;
";

        string GetConvertedProperty(ITypeSymbol propertyType, string propName)
        {
            if (propertyType is INamedTypeSymbol namedType)
            {
                if (namedType.IsGenericType
                    && namedType.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IList_T
                    && namedType.TypeArguments[0].SpecialType == SpecialType.System_String)
                {
                    return $"AttributeHelper.GetStringList({propName})";
                }

                if (namedType.IsValueType && !namedType.IsNullableStruct())
                {
                    var hasBindingProperty = !_propertyInfo.ContainingType.GetMembers($"{propName}Property").IsEmpty;
                    var defaultValue = hasBindingProperty
                        ? $"({GetTypeNameAndAddNamespace(propertyType)}){AvaloniaContainingTypeName}.{propName}Property.GetDefaultValue({AvaloniaContainingTypeName}.{propName}Property.OwnerType)"
                        : "default";

                    return $"{propName} ?? {defaultValue}";
                }

                if (IsGeneric && namedType.GetFullName() == "Microsoft.Avalonia.Controls.BindingBase")
                {
                    return $"AttributeHelper.GetBinding({propName})";
                }

                if (IsGeneric && namedType.GetFullName() == "System.Collections.IList")
                {
                    return $"AttributeHelper.GetIList({propName})";
                }
            }

            return propName;
        }
    }

    internal static GeneratedPropertyInfo[] GetValueProperties(GeneratedTypeInfo generatedType)
    {
        var componentInfo = generatedType.Settings;

        var props = GetMembers<IPropertySymbol>(componentInfo.TypeSymbol, generatedType.Settings.Include)
            .Where(p => !componentInfo.Exclude.Contains(p.Name))
            .Where(p => !componentInfo.ContentProperties.Contains(p.Name))
            .Where(IsPublicProperty)
            .Where(HasPublicSetter)
            .Where(prop => IsExplicitlyAllowed(prop, generatedType) || !DisallowedComponentTypes.Contains(prop.Type.GetFullName()))
            .Where(prop => prop.Type.GetFullName() == "Avalonia.Media.Brush" || !IsRenderFragmentPropertySymbol(generatedType, prop))
            .OrderBy(prop => prop.Name, StringComparer.OrdinalIgnoreCase);

        return props.Select(prop =>
            {
                if (prop.Type.GetFullName() == "Avalonia.Media.Brush")
                {
                    var propName = prop.Name.Replace("Brush", "") + "Color";

                    if (prop.ContainingType.GetProperty(propName, includeBaseTypes: true) != null)
                    {
                        return null;
                    }

                    return new GeneratedPropertyInfo(generatedType, prop, GeneratedPropertyKind.Value)
                    {
                        ComponentPropertyName = propName,
                        ComponentType = generatedType.GetTypeNameAndAddNamespace("Avalonia.Media", "Color")
                    };
                }
                else
                {
                    return new GeneratedPropertyInfo(generatedType, prop, GeneratedPropertyKind.Value);
                }
            })
            .Where(p => p != null)
            .ToArray();
    }


    private static string GetComponentPropertyTypeName(IPropertySymbol propertySymbol, GeneratedTypeInfo containingType, bool isRenderFragmentProperty = false, bool makeNullable = false)
    {
        var typeSymbol = propertySymbol.Type;
        var isGeneric = containingType.Settings.GenericProperties.TryGetValue(propertySymbol.Name, out var typeArgument);
        var typeArgumentName = typeArgument is null ? "T" : containingType.GetTypeNameAndAddNamespace(typeArgument);

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return containingType.GetTypeNameAndAddNamespace(typeSymbol);
        }
        else if (namedTypeSymbol.IsGenericType
            && namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IList_T
            && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_String)
        {
            // Lists of strings are special-cased because they are handled specially by the handlers as a comma-separated list
            return "string";
        }
        else if (isRenderFragmentProperty)
        {
            return isGeneric ? $"RenderFragment<{typeArgumentName}>" : "RenderFragment";
        }
        else if (makeNullable && namedTypeSymbol.IsValueType && !namedTypeSymbol.IsNullableStruct())
        {
            return containingType.GetTypeNameAndAddNamespace(typeSymbol) + "?";
        }
        else if (namedTypeSymbol.SpecialType == SpecialType.System_Collections_IEnumerable && isGeneric)
        {
            return containingType.GetTypeNameAndAddNamespace("System.Collections.Generic", $"IEnumerable<{typeArgumentName}>");
        }
        else if (namedTypeSymbol.GetFullName() == "System.Collections.IList" && isGeneric)
        {
            return containingType.GetTypeNameAndAddNamespace("System.Collections.Generic", $"IList<{typeArgumentName}>");
        }
        else if (namedTypeSymbol.GetFullName() == "Avalonia.Data.BindingBase" && isGeneric)
        {
            return "Func<T, string>";
        }
        else if (namedTypeSymbol.SpecialType == SpecialType.System_Object && isGeneric)
        {
            return typeArgumentName;
        }
        else
        {
            return containingType.GetTypeNameAndAddNamespace(namedTypeSymbol);
        }
    }

    private static bool IsExplicitlyAllowed(IPropertySymbol propertyInfo, GeneratedTypeInfo generatedType)
    {
        return generatedType.Settings.Include.Contains(propertyInfo.Name)
            || propertyInfo.Type.SpecialType == SpecialType.System_Object && generatedType.Settings.GenericProperties.ContainsKey(propertyInfo.Name);
    }

    private static bool IsPublicProperty(IPropertySymbol propertyInfo)
    {
        return propertyInfo.GetMethod?.DeclaredAccessibility == Accessibility.Public
            && IsBrowsable(propertyInfo)
            && !propertyInfo.IsIndexer
            && !IsObsolete(propertyInfo);
    }

    private static bool IsBrowsable(ISymbol propInfo)
    {
        // [EditorBrowsable(EditorBrowsableState.Never)]
        return !propInfo.GetAttributes().Any(a => a.AttributeClass.Name == nameof(EditorBrowsableAttribute)
            && a.ConstructorArguments.FirstOrDefault().Value?.Equals((int)EditorBrowsableState.Never) == true);
    }

    private static bool HasPublicSetter(IPropertySymbol propertyInfo)
    {
        return propertyInfo.SetMethod?.DeclaredAccessibility == Accessibility.Public;
    }

    private static bool IsObsolete(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass.Name == nameof(ObsoleteAttribute));
    }

    private static readonly List<string> DisallowedComponentTypes = new()
    {
        //"Microsoft.Avalonia.Controls.Button.ButtonContentLayout", // TODO: This is temporary; should be possible to add support later
        //"Microsoft.Avalonia.Controls.ColumnDefinitionCollection",
        //"Microsoft.Avalonia.Controls.PointCollection",
        //"Microsoft.Avalonia.Controls.DoubleCollection",
        //"Microsoft.Avalonia.Controls.ControlTemplate",
        //"Microsoft.Avalonia.Controls.DataTemplate",
        //"Microsoft.Avalonia.Controls.Element",
        //"Microsoft.Avalonia.Font", // TODO: This is temporary; should be possible to add support later
        //"Microsoft.Avalonia.Graphics.Font", // TODO: This is temporary; should be possible to add support later
        //"Microsoft.Avalonia.Controls.FormattedString",
        //"Microsoft.Avalonia.Controls.Shapes.Geometry",
        //"Microsoft.Avalonia.Controls.GradientStopCollection",
        //"System.Windows.Input.ICommand",
        //"System.Object",
        //"Microsoft.Avalonia.Controls.Page",
        //"Microsoft.Avalonia.Controls.RowDefinitionCollection",
        //"Microsoft.Avalonia.Controls.Shadow",
        //"Microsoft.Avalonia.Controls.ShellContent",
        //"Microsoft.Avalonia.Controls.ShellItem",
        //"Microsoft.Avalonia.Controls.ShellSection",
        //"Microsoft.Avalonia.Controls.IVisual",
        //"Microsoft.Avalonia.Controls.View",
        //"Microsoft.Avalonia.Graphics.IShape",
        //"Microsoft.Avalonia.IView",
        //"Microsoft.Avalonia.IViewHandler"
    };

    private string GetTypeNameAndAddNamespace(ITypeSymbol type)
    {
        return ContainingType.GetTypeNameAndAddNamespace(type);
    }

    private string GetTypeNameAndAddNamespace(string @namespace, string typeName)
    {
        return ContainingType.GetTypeNameAndAddNamespace(@namespace, typeName);
    }

    private static IEnumerable<T> GetMembers<T>(ITypeSymbol typeSymbol, IEnumerable<string> includeBaseMemberNames) where T : ISymbol
    {
        var baseMembers = includeBaseMemberNames
            .Select(member => typeSymbol.GetMember(member, true))
            .Where(member => member != null);

        return typeSymbol.GetMembers().Union(baseMembers, SymbolEqualityComparer.Default).OfType<T>();
    }
}
