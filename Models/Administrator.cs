using System;

namespace AbcBank.Models
{
    public class Administrator : Person
    {
        public DateTime HireDate { get; set; }

        public string AdminType { get; set; }
    }
}