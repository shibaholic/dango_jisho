namespace Domain.Enums;

public enum Priority
{
    // Spec priorities
    spec1,
    spec2,

    // Ichi priorities
    ichi1,
    ichi2,

    // News priorities
    news1,
    news2,

    // Gai priorities
    gai1,
    gai2,

    nf01,
    nf02,
    nf03,
    nf04,
    nf05,
    nf06,
    nf07,
    nf08,
    nf09,
    nf10,
    nf11,
    nf12,
    nf13,
    nf14,
    nf15,
    nf16,
    nf17,
    nf18,
    nf19,
    nf20,
    nf21,
    nf22,
    nf23,
    nf24,
    nf25,
    nf26,
    nf27,
    nf28,
    nf29,
    nf30,
    nf31,
    nf32,
    nf33,
    nf34,
    nf35,
    nf36,
    nf37,
    nf38,
    nf39,
    nf40,
    nf41,
    nf42,
    nf43,
    nf44,
    nf45,
    nf46,
    nf47,
    nf48 
}

public static class PriorityExtensions
{
    public static Priority Parse(string value)
    {
        if (Enum.TryParse<Priority>(value, true, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Invalid priority: {value}");
    }
    
    public static string ToDatabaseValue(this Priority priority)
    {
        // Converts the enum value to its database string representation
        return priority.ToString().ToLower(); // Converts to lowercase: e.g., Spec1 -> "spec1"
    }
}