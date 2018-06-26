using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public string Chief { get; set; }
    }
}