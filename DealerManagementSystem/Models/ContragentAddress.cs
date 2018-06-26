using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    public class ContragentAddress
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("contragentId")]
        public int ContragentId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}