using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class History
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tdate")]
        public DateTime Tdate { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("implementationOf")]
        public string ImplementationOf { get; set; }

        [JsonProperty("docNum")]
        public long DocNum { get; set; }

        [JsonProperty("waybillNum")]
        public string WaybillNum { get; set; }

        [JsonProperty("amountIn")]
        public double AmountIn { get; set; }

        [JsonProperty("amountOut")]
        public double AmountOut { get; set; }

        [JsonProperty("rest")]
        public double Rest { get; set; }

        [JsonProperty("docType")]
        public int? DocType { get; set; }
    }
}