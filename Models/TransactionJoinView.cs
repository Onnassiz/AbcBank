using System;

namespace AbcBank.Models
{
    public class TransactionJoinView
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string From { get; set; }
        public string Personnel { get; set; }
        public string Description { get; set; }
    }
}