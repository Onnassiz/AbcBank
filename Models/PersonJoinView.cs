using System;

namespace AbcBank.Models
{
    public class PersonJoinView
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Sex { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string BranchName { get; set; }
        public string MarritalStatus { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Banker { get; set; }
    }
}