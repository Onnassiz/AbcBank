using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class Address
    {
        public string Id { get; set; }

        [Required]
        public string HouseNumber { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string County { get; set; }

        [Required]
        [PostCodeUnique]
        public string PostCode { get; set; }

        public string ToString { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public DateTime DateUpdated { get; set; }

        public List<BankBranch> BankBranches { get; set; }

        public List<Person> Persons { get; set; }
    }
}