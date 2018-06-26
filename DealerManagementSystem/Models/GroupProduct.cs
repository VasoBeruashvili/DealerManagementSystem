using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    [Table("GroupProducts", Schema = "book")]
    public class GroupProduct
    {
        [Column("id")]
        [JsonProperty("id")]        
        public int Id { get; set; }

        [Column("parentid")]
        [JsonProperty("parentId")]        
        public int? ParentId { get; set; }

        [Column("path")]
        [JsonProperty("path")]
        public string Path { get; set; }

        [Column("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Column("tag")]
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [Column("account")]
        [JsonProperty("account")]
        public string Account { get; set; }

        [Column("order_id")]
        [JsonProperty("orderId")]
        public int? OrderId { get; set; }

        [Column("client_id")]
        [JsonProperty("clientId")]
        public int ClientId { get; set; }


        [NotMapped]
        [JsonProperty("children")]
        public List<GroupProduct> Children { get; set; }
    }
}