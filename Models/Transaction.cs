using System;
using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class Transaction
    {
        public string Id { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime DateCreated { get; set; }

        public string From { get; set; }

        public string Description { get; set; }

        public Account Account { get; set; }

        public string PersonId { get; set; }

        public Person Person { get; set; }
    }
}