using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Menu_Rol
{
    public int idMenu_Rol { get; set; }
    [Required]
    public int  idMenu { get; set; }
    [Required]
    public int idRol { get; set; }

    public Menu menu { get; set; }
    public Rol rol { get; set; }
}