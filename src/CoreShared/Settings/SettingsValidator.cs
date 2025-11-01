using System.Reflection;

namespace CoreShared.Settings;

public static class SettingsValidator
{
    public static TSettings Validate<TSettings>(this TSettings settings) where TSettings : BaseAppSettings
    {
        ArgumentNullException.ThrowIfNull(settings);
        ExecuteValidation(settings);
        
        return settings;
    }

    private static void ExecuteValidation(object settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        
        foreach (PropertyInfo prop in settings.GetType().GetProperties())
        {
            if (prop.GetIndexParameters().Length > 0)
                continue;
            
            var value = prop.GetValue(settings);
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            
            if (type == typeof(string))
                ArgumentException.ThrowIfNullOrWhiteSpace(prop.GetValue(settings) as string, prop.Name);
            else if (!type.IsValueType)
            {
                if (IsNullable(prop))
                    continue;
                
                if (value is null)
                    ArgumentNullException.ThrowIfNull(prop.GetValue(settings), prop.Name);
                
                ExecuteValidation(value!);
            }
        }
    }

    private static bool IsNullable(PropertyInfo prop)
    {
        return prop.CustomAttributes
            .FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute")
            is not null;
    }
}