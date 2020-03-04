using System.ComponentModel.DataAnnotations;

public class DetalleVentaViewModel
{
    [Required]
    public int idArticulo { get; set; }
    public string articulo { get; set; }
    [Required]
    public int cantidad { get; set; }
    [Required]
    public decimal precio { get; set; }
    [Required]
    public decimal descuento { get; set; }
}