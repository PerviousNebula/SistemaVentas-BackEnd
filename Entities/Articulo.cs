using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Articulo
{
    public int idArticulo { get; set; }
    [Required]
    public int idCategoria { get; set; }
    public string codigo { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre no debe tener más de 50 caracteres, ni menos de 3 caracteres")]
    public string nombre { get; set; }
    [Required]
    public decimal precio_venta { get; set; }
    [Required]
    public int stock { get; set; }
    public string descripcion { get; set; }
    public bool activo { get; set; }
    public Categoria categoria { get; set; }
    public ICollection<DetalleIngreso> detallesIngresos { get; set; }
}