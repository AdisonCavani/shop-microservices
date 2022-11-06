using System.Reflection;

namespace Server.Settings;

public static class Validator
{
    public static DbSettings Validate(this DbSettings dbSettings)
    {
        ExecuteValidation(dbSettings);
        return dbSettings;
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