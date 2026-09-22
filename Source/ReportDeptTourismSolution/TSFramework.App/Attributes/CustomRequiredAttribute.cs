using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TSFramework.App.Processors;

namespace TSFramework.App.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        private string _displayName;

        public CustomRequiredAttribute()
        {
            ErrorMessage = AppProcessor.Messagor.GetMessage("Common_RequiredMessage");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = string.Format(FormatErrorMessage(metadata.GetDisplayName()), metadata.GetDisplayName()),
                ValidationType = "required"
            };
            return new[] {rule};
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var attributes = validationContext.ObjectType.GetProperty(validationContext.MemberName)
                ?.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            _displayName = attributes != null && attributes.Length > 0
                ? ((DisplayNameAttribute) attributes[0]).DisplayName
                : validationContext.DisplayName;

            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            _displayName = name;
            return string.Format(ErrorMessageString, name);
        }
    }
}