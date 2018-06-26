using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("groupId")]
        public int? GroupId { get; set; }
        
        [JsonProperty("path")]
        public string Path { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("code")]
        public string Code { get; set; }
        
        [JsonProperty("unitId")]
        public int? UnitId { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }



        [NotMapped]
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [NotMapped]
        [JsonProperty("price")]
        public double? Price { get; set; }

        [NotMapped]
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [NotMapped]
        [JsonProperty("ordered")]
        public int Ordered { get; set; }

        [NotMapped]
        [JsonProperty("stock")]
        public double Stock { get; set; }

        [NotMapped]
        [JsonProperty("total")]
        public double Total { get; set; }

        [NotMapped]
        [JsonProperty("totalSum")]
        public double TotalSum { get; set; }

        [NotMapped]
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [NotMapped]
        [JsonProperty("images")]
        public List<ProductImage> Images { get; set; }

        [NotMapped]
        [JsonProperty("groupName")]
        public string GroupName { get; set; }
    }
}