using System.Reflection;

namespace ProductService.Startup;

public static class SettingsValidator
{
    public static AppSettings Validate(this AppSettings appSettings)
    {
        ExecuteValidation(appSettings);
        return appSettings;
    }

    private static void ExecuteValidation(object settings)
    {
        foreach (PropertyInfo prop in settings.GetType().GetProperties())
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            
            if (type == typeof(string))
                ArgumentException.ThrowIfNullOrEmpty(prop.GetValue(settings) as string);
        }
    }
}