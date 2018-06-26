using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class InvoiceItem
    {
        public long IndexNumber { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
        public long InvoiceNum { get; set; }
        public string InvoiceTerm { get; set; }
        public DateTime? Tdate { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Account { get; set; }
        public string CurrencyCode { get; set; }
    }
}