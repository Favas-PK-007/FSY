using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class GeoCoordinateValidationAttribute : ValidationAttribute
{
    private readonly int _maxDecimalPlaces;

    public GeoCoordinateValidationAttribute(int maxDecimalPlaces = 10)
    {
        _maxDecimalPlaces = maxDecimalPlaces;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var propertyName = validationContext.DisplayName;

        if (value == null)
            return ValidationResult.Success;

        if (value is double coordinate)
        {
            // Check range
            if (coordinate < -180 || coordinate > 180)
            {
                return new ValidationResult($"{propertyName} must be between -180 and 180, with up to {_maxDecimalPlaces} decimal places.");
            }

            // Check decimal places
            string valueStr = coordinate.ToString("G17", CultureInfo.InvariantCulture);
            int decimalPlaces = 0;

            if (valueStr.Contains("."))
            {
                string[] parts = valueStr.Split('.');
                decimalPlaces = parts[1].Length;
            }

            if (decimalPlaces > _maxDecimalPlaces)
            {
                return new ValidationResult($"{propertyName} must be between -180 and 180, with up to {_maxDecimalPlaces} decimal places.");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult($"Invalid {propertyName}.");
    }
}
