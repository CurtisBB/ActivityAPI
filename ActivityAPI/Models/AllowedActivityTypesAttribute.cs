using System.ComponentModel.DataAnnotations;

public class AllowedActivityTypesAttribute : ValidationAttribute
{
    private readonly string[] _allowedTypes = { "Phone Call", "Email", "Document", "Appointment" };

    public AllowedActivityTypesAttribute()
    {
        ErrorMessage = "Invalid activity type. Allowed types are: Phone Call, Email, Document, Appointment.";
    }

    public override bool IsValid(object value)
    {
        if (value is string activityType)
        {
            return _allowedTypes.Contains(activityType);
        }

        return false;
    }
}
