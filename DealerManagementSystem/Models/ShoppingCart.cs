using Newtonsoft.Json;

namespace DealerManagementSystem.Models
{
    public class ShoppingCart
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
    }
}