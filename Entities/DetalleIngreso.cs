using System.ComponentModel.DataAnnotations;

public class DetalleIngreso
{
    public int idDetalleIngreso { get; set; }
    [Required]
    public int idIngreso { get; set; }
    [Required]
    public int idArticulo { get; set; }
    [Required]
    public int cantidad { get; set; }
    [Required]
    public decimal precio { get; set; }

    public Ingreso ingreso { get; set; }
    public Articulo articulo { get; set; }
}