using System.ComponentModel.DataAnnotations;

public class ActualizarViewModel
{
    [Required]
    public int idCategoria { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre no debe de tener mas de 50 caracteres ni menos de 3 caracteres")]
    public string nombre { get; set; }
    [StringLength(256, ErrorMessage = "La descripción no debe tener más de 256 caracteres")]
    public string descripcion { get; set; }
}