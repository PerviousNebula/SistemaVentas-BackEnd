using System.ComponentModel.DataAnnotations;

public class Menu
{
    public int idMenu { get; set; }
    [Required]
    public string title { get; set; }
    [Required]
    public string icon { get; set; }

}