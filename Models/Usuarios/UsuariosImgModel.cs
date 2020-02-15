using Microsoft.AspNetCore.Http;

public class UsuariosImgModel
{
    public string name { get; set; }
    public IFormFile image { get; set; }
}