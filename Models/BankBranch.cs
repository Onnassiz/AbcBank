using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class BankBranch
    {
        public string Id { get; set; }

        [Required]
        public string BranchName { get; set; }

        [Required]
        public string SortCode { get; set; }

        public string AddressId { get; set; }

        public Address Address { get; set; }

        public List<Person> Persons { get; set; }
    }
}