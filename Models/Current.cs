namespace AbcBank.Models
{
    public class Current : Account
    {
        public double OverDraftLimit {
            get { return 50; }
        }

        public double OverDraft { get; set; }

        public double OverdraftInterestRate {
            get { return 4; }
        }
    }
}