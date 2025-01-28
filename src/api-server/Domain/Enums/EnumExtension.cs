namespace Domain.Enums;

public static class EnumExtension
{
    public static T Parse<T>(string value)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) 
        {
            throw new ArgumentException("T must be an enumerated type");
        }
        
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Invalid priority: {value}");
    }
    
    public static string? ToDatabaseValue<T>(T? value)
        where T : struct, IConvertible
    {
        if (value == null) return null;
        if (!typeof(T).IsEnum) 
        {
            throw new ArgumentException("T must be an enumerated type");
        }
        
        // Converts the enum value to its database string representation
        return value.ToString().ToLower(); // Converts to lowercase: e.g., Spec1 -> "spec1"
    }
}