using System.ComponentModel.DataAnnotations;

public class Subcategory
{
    public int idSubcategory { get; set; }
    [Required]
    public string title { get; set; }
    [Required]
    public string url { get; set; }
    [Required]
    public int idMenu { get; set; }

    public Menu menu { get; set; }
}