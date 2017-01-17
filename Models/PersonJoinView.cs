using System;

namespace AbcBank.Models
{
    public class PersonJoinView
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
        public string AdminType { get; set; }
        public DateTime HireDate { get; set; }
        public string BranchName { get; set; }
    }
}