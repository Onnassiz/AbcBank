using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class Card
    {
        public string Id { get; set; }

        [Required]
        public string CardName { get; set; }

        [Required]
        [CardUnique]
        public string CardNumber { get; set; }

        [Required]
        public string CardPin { get; set; }

        public string Token { get; set; }

        [Required]
        public string AccountId { get; set; }

        public Account Account { get; set; }
    }
}
