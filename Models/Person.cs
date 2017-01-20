using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class Person
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string MaidenName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string MarritalStatus { get; set; }

        [Required]
        public string Sex { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        public string AddressId { get; set; }

        public string Descriminator { get; set; }

        public string BankBranchId { get; set; }

        public Address Address { get; set; }

        public BankBranch BankBranch { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FullName
        {
            get { return LastName + ", " + FirstName + " " + MiddleName; }
        }

        public int Age {
            get
            {
                var Today = DateTime.Today;
                var Age = Today.Year - DateOfBirth.Year;
                return Age;
            }
        }

        public List<AccountHolder> AccountHolders { get; set; }
    }
}