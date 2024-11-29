using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Category { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
