using System.ComponentModel.DataAnnotations;

namespace AbcBank.Models
{
    public class Transfer
    {
        [Required]
        [AccountExists]
        public string PrincipalAccount { get; set; }

        [Required]
        [AccountExists]
        [Unlike("PrincipalAccount")]
        public string ReceivingAccount { get; set; }
    }
}