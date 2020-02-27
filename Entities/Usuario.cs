using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Usuario
{
    public int idUsuario { get; set; }
    [Required]
    public int idRol { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre no debe tener más de 100 caracteres ni menos de 3 caracteres.")]
    public string nombre { get; set; }
    [StringLength(20, ErrorMessage = "El tipo de documento no debe tener más de 20 caracteres.")]
    public string tipo_documento { get; set; }
    [StringLength(20, ErrorMessage = "El número de documento no debe tener más de 20 caracteres.")]
    public string num_documento { get; set; }
    [StringLength(70, ErrorMessage = "La dirección no debe tener más de 70 caracteres.")]
    public string direccion { get; set; }
    [StringLength(20, MinimumLength = 10, ErrorMessage = "El número de teléfono no debe tener más de 20 caracteres ni menos de 10 caracteres.")]
    public string telefono { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "El email no debe tener más de 50 caracteres.")]
    public string email { get; set; }
    [Required]
    public byte[] password_hash { get; set; }
    [Required]
    public byte[] password_salt { get; set; }
    public string imgUrl { get; set; }
    public bool activo { get; set; }

    public Rol rol { get; set; }
    public ICollection<Ingreso> ingresos { get; set; }
}