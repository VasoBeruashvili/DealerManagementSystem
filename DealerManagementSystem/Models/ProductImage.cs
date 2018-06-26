using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class ProductImage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("productId")]
        public int? ProductId { get; set; }

        [JsonProperty("image")]
        public byte[] Image { get; set; }
    }
}