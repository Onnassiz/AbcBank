namespace AbcBank.Models
{
    public class Customer : Person
    {
        public string BankerId { get; set; }

        public string TellerId { get; set; }
    }
}