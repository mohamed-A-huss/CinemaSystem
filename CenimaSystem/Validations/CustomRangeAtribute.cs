
namespace CinemaSystem.Validations
{
    public class CustomRangeAtribute : ValidationAttribute
    {
        private readonly double _min;
        private readonly double _max;

        public CustomRangeAtribute(double min, double max)
        {
            _min = min;
            _max = max;
        }
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is double doubleValue)
            {
                if (doubleValue >= _min && doubleValue <= _max)
                {
                    return true;
                }
            }

            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"The field {name} must be between {_min} and {_max}.";
        }
    }
}
