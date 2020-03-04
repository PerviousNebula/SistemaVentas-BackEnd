using System.ComponentModel.DataAnnotations;

public class DetalleVenta
{
    public int idDetalleVenta { get; set; }
    [Required]
    public int idVenta { get; set; }
    [Required]
    public int idArticulo { get; set; }
    [Required]
    public int cantidad { get; set; }
    [Required]
    public decimal precio { get; set; }
    [Required]
    public decimal descuento { get; set; }

    public Venta venta { get; set; }
    public Articulo articulo { get; set; }

}