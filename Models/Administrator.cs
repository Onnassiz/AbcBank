using System;

namespace AbcBank.Models
{
    public class Administrator : Person
    {
        public DateTime HireDate { get; set; }

        public int CustomerCount { get; set; }
    }
}