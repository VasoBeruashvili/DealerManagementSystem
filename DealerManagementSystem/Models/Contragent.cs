using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    [Table("Contragents", Schema = "book")]
    public class Contragent
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "UserName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [JsonProperty("password")]
        public string Password { get; set; }

        public string Code { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string Account { get; set; }
        public string Account2 { get; set; }



        //დამატებითი არასტანდარტული ველები
        public string Col1_543 { get; set; }
        public string Col2_544 { get; set; }
        public string Col3_545 { get; set; }
        public string Col4_546 { get; set; }
        public string Col5_547 { get; set; }
        public string Col6_548 { get; set; }

        public string Col7_549_C4 { get; set; }
        public string Col8_550_C4 { get; set; }

        public string Col9_551 { get; set; }
        public string Col10_552 { get; set; }
        public string Col11_553 { get; set; }
        public string Col12_554 { get; set; }
        public string Col13_555 { get; set; }
        public string Col14_556 { get; set; }

        public string Col7_557_C5 { get; set; }
        public string Col8_558_C5 { get; set; }

        public string Col7_559_C6 { get; set; }
        public string Col8_560_C6 { get; set; }

        public string Col7_561_C7 { get; set; }
        public string Col8_562_C7 { get; set; }
        //---



        public List<ContragentDiscount> ContragentDiscounts { get; set; }
    }
}