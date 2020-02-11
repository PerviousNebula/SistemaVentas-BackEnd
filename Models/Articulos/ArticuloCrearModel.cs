using System.ComponentModel.DataAnnotations;

public class ArticuloCrearModel
{
    [Required]
    public int idCategoria { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre no debe tener más de 50 caracteres, ni menos de 3 caracteres")]
    public string nombre { get; set; }
    public string codigo { get; set; }
    [Required]
    public decimal precio_venta { get; set; }
    [Required]
    public int stock { get; set; }
    public string descripcion { get; set; }
}