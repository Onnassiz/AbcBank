namespace AbcBank.Models
{
    public class AccountJoinModel
    {
        public string Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string Descriminator { get; set; }
        public string Branch { get; set; }
        public bool IsJoint { get; set; }
        public string Holder { get; set; }
        public bool IsActive { get; set; }
        public double Balance { get; set; }
        public double Overdraft { get; set; }
        public int MonthlyCount { get; set; }
        public double OverdraftLimit { get; set; }
        public double OverdraftInterestRate { get; set; }
        public double InterestRate { get; set; }
        public int MonthlyLimit { get; set; }
        public double DailyOut { get; set; }
    }
}