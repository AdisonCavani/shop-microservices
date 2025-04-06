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
        foreach (PropertyInfo prop in settings.GetType().GetProperties())
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            
            if (type == typeof(string))
                ArgumentException.ThrowIfNullOrWhiteSpace(prop.GetValue(settings) as string);
        }
    }
}