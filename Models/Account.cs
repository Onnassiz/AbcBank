using System;
using System.Collections.Generic;
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

        public string SortCode { get; set; }

        public double Balance { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime CloseDate { get; set; }

        public double DailyLimit {
            get { return 300; }
        }

        public double DailyIn { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Joint Account?")]
        public bool IsJoint { get; set; }

        public int JointHolderLimit {
            get { return 3; }
        }

        [Required]
        public string Descriminator { get; set; }

        public List<AccountHolder> AccountHolders { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}