
namespace DealerManagementSystem.Models
{
    public class DebtChangeModel
    {
        public string remaining_time { get; set; }
        public decimal sruli_mimdinare_davalianeba { get; set; }
        public decimal mimdinare_gadasaxdeli { get; set; }
        public decimal avg_overdue_amount { get; set; }
        public int avg_overdue_days { get; set; }
        public string contragentCurrency { get; set; }
    }
}