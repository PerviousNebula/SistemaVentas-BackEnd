using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrador")]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly DbContextSistema _context;

    public RolesController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Roles/Listar
    [HttpGet("[action]")]
    public async Task<IEnumerable<RolViewModel>> Listar([FromQuery] CategoriasParametros categoriasParametros)
    {
        var roles = await _context.Roles.OrderBy(r => r.nombre).ToListAsync();
        return roles.Select(r => new RolViewModel
        {
            idRol = r.idRol,
            nombre = r.nombre,
            descripcion = r.descripcion,
            activo = r.activo
        });
    }


}