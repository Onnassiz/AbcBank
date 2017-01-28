using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AbcBank.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EmailUniqueAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "This email is already taken";

        public EmailUniqueAttribute()
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
                    if (context.Persons.Find(id).Email != value.ToString())
                    {
                        if (context.Persons.Any(x => x.Email == value.ToString()))
                        {
                            return new ValidationResult(
                                FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                else
                {
                    if (context.Persons.Any(x => x.Email == value.ToString()))
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