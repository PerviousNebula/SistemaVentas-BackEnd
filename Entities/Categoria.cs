using System.ComponentModel.DataAnnotations;

public class Categoria {
    public int idCategoria { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre no debe de tener mas de 50 caracteres ni menos de 3 caracteres")]
    public string nombre { get; set; }
    [StringLength(256)]
    public string descripcion { get; set; }
    public bool activo { get; set; }
}
