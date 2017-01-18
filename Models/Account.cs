using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AbcBank.Models
{
    public class Account
    {
        public string Id { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        public double Balance { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime CloseDate { get; set; }

        public double DailyLimit {
            get { return 300; }
        }

        public double DailyIn { get; set; }

        public bool IsActive { get; set; }

        public bool IsJoint { get; set; }
    }
}