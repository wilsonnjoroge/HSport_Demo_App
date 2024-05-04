﻿
#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HSport.App.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvaillable { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [JsonIgnore]

        public virtual Category Category { get; set; }
        public bool IsAvailable { get; internal set; }
    }
}
