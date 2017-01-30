using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AbcBank.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CardUniqueAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "{0} already taken.";
        public CardUniqueAttribute()
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
                if (context.Cards.Any(x => x.CardNumber == value.ToString()))
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}
