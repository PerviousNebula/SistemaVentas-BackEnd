using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Authorize(Roles = "Administrador, Almacenero")]
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
    public async Task<IEnumerable<CategoriaViewModel>> Listar([FromQuery] PaginationParameters pagParams)
    {
        var items = await _context.Categorias.OrderBy(c => c.nombre).ToListAsync();        
        var categorias = PagedList<Categoria>.ToPagedList(items, pagParams.PageNumber, pagParams.PageSize);
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

        // Se valida si el identificador de la categoria que se quiere actualizar es valido
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
        
        var categorias = await this.Listar(new PaginationParameters { PageNumber = 1, PageSize = 10});

        return Ok(new {
            Ok = true,
            message = "La categoría se ha actualizado exitosamente!",
            categorias
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

        // Se valida si la categoria ya existe en el sistema
        var existeCategoria = await _context.Categorias.FirstOrDefaultAsync(c => c.nombre == model.nombre);
        if (existeCategoria != null)
        {
            return BadRequest(new {
                ok = false,
                message = "La categoría que intenta agregar ya existe"
            });
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
        catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su categoría, inténtelo más tarde"
            });
        }

        var categorias = await this.Listar(new PaginationParameters {PageNumber = 1, PageSize = 10});
        
        return Ok(new {
            Ok = true,
            message = "La categoría se ha creado exitosamente!",
            categorias
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

    // POST: api/Categorias/Filtrar
    [HttpPost("[action]")]
    public async Task<IActionResult> Filtrar([FromBody] CategoriaFilterModel model, [FromQuery] PaginationParameters pagParams)
    {
        if (model == null)
        {
            return BadRequest(new {
                ok = false,
                message = "Error al filtrar, el modelo del filtro es nulo"
            });
        }
        var items = await _context.Categorias.Where(c =>  c.nombre.Contains(model.nombre != null ? model.nombre : string.Empty) && 
                                                          c.descripcion.Contains(model.descripcion != null ? model.descripcion : string.Empty) && 
                                                          c.activo == model.activo)
                                              .OrderBy(c => c.nombre)
                                              .ToListAsync();
        if (items == null)
        {
            return NotFound(new {
                ok = false,
                message = "No se encontraron resultados de su búsqueda"
            });
        }
        var categorias = PagedList<Categoria>.ToPagedList(items, pagParams.PageNumber, pagParams.PageSize);
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
        
        return Ok(categorias.Select(c => new CategoriaViewModel {
            idCategoria = c.idCategoria,
            nombre = c.nombre,
            descripcion = c.descripcion,
            activo = c.activo
        }));
    }    
    
}
