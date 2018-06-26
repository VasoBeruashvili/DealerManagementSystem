using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class ContragentDiscount
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("discount")]
        public double? Discount { get; set; }
    }
}