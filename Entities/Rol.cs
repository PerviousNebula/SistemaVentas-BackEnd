using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Rol {
    public int idRol { get; set; }
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "El nombre no debe de tener más de 30 caracteres ni menos de 3 caracteres")]
    public string nombre { get; set; }
    [StringLength(256, ErrorMessage = "La descripción no debe de tener más de 100 caracteres")]
    public string descripcion { get; set; }
    public bool activo { get; set; }

    public ICollection<Usuario> usuarios { get; set; }
}