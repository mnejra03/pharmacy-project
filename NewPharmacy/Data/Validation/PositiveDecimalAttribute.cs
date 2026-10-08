using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class PositiveDecimalAttribute : ValidationAttribute
    {
        private readonly decimal _minimumValue;

        public PositiveDecimalAttribute(double minimumValue)
        {
            _minimumValue = Convert.ToDecimal(minimumValue);
        }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue >= _minimumValue;
            }

            return false;
        }
    }
}
