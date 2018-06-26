using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class FinaOrder
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tdate")]
        public DateTime Tdate { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonProperty("overdueDays")]
        public int? OverdueDays { get; set; }

        [JsonProperty("debit")]
        public decimal Debit { get; set; }

        [JsonProperty("credit")]
        public decimal Credit { get; set; }

        [JsonProperty("rest")]
        public double Rest { get; set; }

        [JsonProperty("docNum")]
        public long? DocNum { get; set; }

        [JsonProperty("statusId")]
        public int? StatusId { get; set; }

        [JsonProperty("refId")]
        public int? RefId { get; set; }

        [JsonProperty("docType")]
        public int? DocType { get; set; }

        [JsonProperty("waybillStatus")]
        public int? WaybillStatus { get; set; }
    }
}