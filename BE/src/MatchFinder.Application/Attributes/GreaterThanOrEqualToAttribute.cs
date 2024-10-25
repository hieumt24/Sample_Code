using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Attributes
{
    public class GreaterThanOrEqualToAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public GreaterThanOrEqualToAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (IComparable)value;

            if (currentValue == null)
                return ValidationResult.Success;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (IComparable)property.GetValue(validationContext.ObjectInstance);
            if (comparisonValue == null)
                return ValidationResult.Success;

            if (currentValue.CompareTo(comparisonValue) < 0)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}