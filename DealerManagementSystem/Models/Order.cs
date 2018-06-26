using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class Order
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("productId")]
        public int ProductId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("contragentId")]
        public int ContragentId { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }
    }
}