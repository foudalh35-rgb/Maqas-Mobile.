using System.ComponentModel.DataAnnotations;

namespace MyProject.Web.ViewModels;

public class CreateProductViewModel
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
}
