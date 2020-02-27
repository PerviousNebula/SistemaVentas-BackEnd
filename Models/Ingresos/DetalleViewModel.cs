using System.ComponentModel.DataAnnotations;

public class DetalleViewModel
{
    [Required]
    public int idArticulo { get; set; }
    public string articulo { get; set; }
    [Required]
    public int cantidad { get; set; }
    [Required]
    public decimal precio { get; set; }
}