using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AbcBank.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AccountUniqueAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "{0} already taken.";
        public AccountUniqueAttribute()
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
                    if (context.Accounts.Find(id).AccountNumber != value.ToString())
                    {
                        if (context.Accounts.Any(x => x.AccountNumber == value.ToString()))
                        {
                            return new ValidationResult(
                                FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                else
                {
                    if (context.Accounts.Any(x => x.AccountNumber == "Abc Bank - " + value.ToString()))
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