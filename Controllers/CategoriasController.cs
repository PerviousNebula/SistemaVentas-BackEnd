using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly DbContextSistema _context;
    public CategoriasController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Categorias/Listar
    [HttpGet("[action]")]
    public async Task<IEnumerable<CategoriaViewModel>> Listar([FromQuery] CategoriasParametros categoriasParametros)
    {
        var categorias = PagedList<Categoria>.ToPagedList(await _context.Categorias.OrderBy(c => c.nombre).ToListAsync(),
                                                          categoriasParametros.PageNumber,
                                                          categoriasParametros.PageSize);
        var metadata = new
	    {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return categorias.Select(c => new CategoriaViewModel
        {
            idCategoria = c.idCategoria,
            nombre = c.nombre,
            descripcion = c.descripcion,
            activo = c.activo
        });
    }

    // GET: api/Categorias/Mostrar/1
    [HttpGet("[action]/{id}")]
    public async Task<ActionResult> Mostrar([FromRoute] int id)
    {
        var catetegoria = await _context.Categorias.FindAsync(id);
        if (catetegoria == null)
        {
            return NotFound();
        }
        return Ok(new CategoriaViewModel {
            idCategoria = catetegoria.idCategoria,
            nombre = catetegoria.nombre,
            descripcion = catetegoria.descripcion,
            activo = catetegoria.activo
        });
    }

    // PUT: api/Categorias/Actualizar
    [HttpPut("[action]")]
    public async Task<ActionResult> Actualizar([FromBody] ActualizarViewModel model) 
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (model.idCategoria <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "La categoría que intenta actualizar no se encuentra en el sistema"
            });
        }
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idCategoria == model.idCategoria);

        if (categoria == null)
        {
            return NotFound(new {
                ok = false,
                message = "La categoría que intenta actualizar no se encuentra en el sistema"
            });
        }

        categoria.nombre = model.nombre;
        categoria.descripcion = model.descripcion;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Guardar Excepcion
            return NotFound(new {
                ok = false,
                message = "Hubo un error al actualizar la categoría, intente más tarde"
            });
        }
        return Ok(new {
            Ok = true,
            message = "La categoría se ha actualizado exitosamente!"
        });
    }

    // POST: api/Categorias/Crear
    [HttpPost("[action]")]
    public async Task<IActionResult> Crear([FromBody] CrearViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Categoria categoria = new Categoria {
            nombre = model.nombre,
            descripcion = model.descripcion,
            activo = true
        };
        _context.Categorias.Add(categoria);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su categoría, inténtelo más tarde"
            });
        }
        return Ok(new {
            ok = true,
            message = "Su categoría fue creada exitosamente"
        });
    }

    // DELETE: api/Categorias/Eliminar/5
    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound(new {
                ok = false,
                message = "La categoría que intenta eliminar no se encuentra en el sistema"
            });
        }
        _context.Categorias.Remove(categoria);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un error al eliminar la categoría, intente más tarde"
            });
        }
        return Ok(new {
            ok = true,
            message = "La categoría se ha eliminado exitosamente",
            categoria
        });
    }

    // PUT: api/Categorias/Desactivar/1
    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> Desactivar([FromRoute] int id) 
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "La categoría que intenta desactivar no se encuentra en el sistema"
            });
        }
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idCategoria == id);

        if (categoria == null)
        {
            return NotFound(new {
                ok = false,
                message = "La categoría que intenta desactivar no se encuentra en el sistema"
            });
        }

        categoria.activo = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Guardar Excepcion
            return NotFound(new {
                ok = false,
                message = "Hubo un error al desactivar la categoría, intente más tarde"
            });
        }
        return Ok(new {
            Ok = true,
            message = "La categoría se ha desactivado exitosamente!"
        });
    }

    // PUT: api/Categorias/Activar/1
    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> Activar([FromRoute] int id) 
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "La categoría que intenta activar no se encuentra en el sistema"
            });
        }
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idCategoria == id);

        if (categoria == null)
        {
            return NotFound(new {
                ok = false,
                message = "La categoría que intenta activar no se encuentra en el sistema"
            });
        }

        categoria.activo = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Guardar Excepcion
            return NotFound(new {
                ok = false,
                message = "Hubo un error al activar la categoría, intente más tarde"
            });
        }
        return Ok(new {
            Ok = true,
            message = "La categoría se ha activado exitosamente!"
        });
    }
}
