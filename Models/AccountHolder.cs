using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class AccountHolder
    {
        public string Id { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string PersonId { get; set; }

        public Account Account { get; set; }

        public Person Person { get; set; }
    }
}