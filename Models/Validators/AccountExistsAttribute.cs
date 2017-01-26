using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AbcBank.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AccountExistsAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "{0} does not exist.";
        public AccountExistsAttribute()
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
                var context = new MyDbContext();
                if (!context.Accounts.Any(x => x.AccountNumber == value.ToString()))
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}