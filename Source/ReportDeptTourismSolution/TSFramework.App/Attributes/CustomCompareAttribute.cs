using System.ComponentModel.DataAnnotations;
using TSFramework.App.Processors;

namespace TSFramework.App.Attributes
{
    public class CustomCompareAttribute : CompareAttribute
    {
        public CustomCompareAttribute(string otherProperty) : base(otherProperty)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(AppProcessor.Messagor.GetMessage(ErrorMessage), name, OtherPropertyDisplayName);
        }
    }
}