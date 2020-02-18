using System.ComponentModel.DataAnnotations;

public class PersonaCrearModel
{
    [Required]
    public string tipo_persona { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre no debe tener más de 100 caracteres ni menos de 3 caracteres")]
    public string nombre { get; set; }
    public string tipo_documento { get; set; }
    public string num_documento { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
    public string email { get; set; }
}