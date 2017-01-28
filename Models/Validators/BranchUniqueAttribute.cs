using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AbcBank.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BranchUniqueAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "This branch name already exists on a different bank";
        public BranchUniqueAttribute()
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
                    if (context.BankBranches.Find(id).BranchName != value.ToString())
                    {
                        if (context.BankBranches.Any(x => x.BranchName == value.ToString()))
                        {
                            return new ValidationResult(
                                FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                else
                {
                    if (context.BankBranches.Any(x => x.BranchName == "Abc Bank - " + value.ToString()))
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