namespace AbcBank.Models
{
    public class Savings: Account
    {
        public double InterestRate {
            get { return 2; }
        }

        public int MonthlyLimit {
            get { return 3; }
        }

        public int MonthlyCount { get; set; }
    }
}