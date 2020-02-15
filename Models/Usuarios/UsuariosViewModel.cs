
public class UsuarioViewModel
{
    public int idUsuario { get; set; }
    public int idRol { get; set; }
    public string rol { get; set; }
    public string nombre { get; set; }
    public string tipo_documento { get; set; }
    public string num_documento { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
    public string email { get; set; }
    public byte[] password_hash { get; set; }
    public string imgUrl { get; set; }
    public bool activo { get; set; }
}