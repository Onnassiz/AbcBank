using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AbcBank.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PostCodeUniqueAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "The postal code {0} is already taken";

        public PostCodeUniqueAttribute()
            : base(DefaultErrorMessage)
        {

        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name);
        }

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                var idField = validationContext.ObjectInstance.GetType().GetProperty("Id");
                var id = idField.GetValue(validationContext.ObjectInstance, null);
                var context = new MyDbContext();
                if (id != null)
                {
                    if (context.Addresses.Find(id).PostCode != value.ToString())
                    {
                        if (context.Addresses.Any(x => x.PostCode == value.ToString()))
                        {
                            return new ValidationResult(
                                FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                else
                {
                    if (context.Addresses.Any(x => x.PostCode == value.ToString()))
                    {
                        return new ValidationResult(
                            FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}