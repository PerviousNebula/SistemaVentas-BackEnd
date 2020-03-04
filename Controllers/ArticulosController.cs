using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Authorize(Roles = "Administrador, Almacenero")]
[Route("api/[controller]")]
[ApiController]
public class ArticulosController : ControllerBase
{
    private readonly DbContextSistema _context;

    public ArticulosController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Articulos/Listar
    [HttpGet("[action]")]
    public async Task<IEnumerable<ArticuloViewModel>> Listar([FromQuery] PaginationParameters PaginationParameters)
    {
        var items = await _context.Articulos.Include(a => a.categoria).OrderBy(a => a.nombre).ToListAsync();
        var articulos = PagedList<Articulo>.ToPagedList(items,
                                                        PaginationParameters.PageNumber,
                                                        PaginationParameters.PageSize);
        var metadata = new
	    {
            articulos.TotalCount,
            articulos.PageSize,
            articulos.CurrentPage,
            articulos.TotalPages,
            articulos.HasNext,
            articulos.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return articulos.Select(a => new ArticuloViewModel
        {
            idArticulo = a.idArticulo,
            idCategoria = a.idCategoria,
            categoria = a.categoria.nombre,
            nombre = a.nombre,
            descripcion = a.descripcion,
            codigo = a.codigo,
            stock = a.stock,
            precio_venta = a.precio_venta,
            activo = a.activo
        });
    }

    // GET: api/Articulos/Mostrar/1
    [HttpGet("[action]/{id}")]
    public async Task<ActionResult> Mostrar([FromRoute] int id)
    {
        if (id <= 0) {
            return BadRequest(new {
                ok = false,
                message = "El id proporcionado del articulo es inválido"
            });
        }
        var articulo = await _context.Articulos.Include(a => a.categoria).SingleOrDefaultAsync(a => a.idArticulo == id);
        if (articulo == null)
        {
            return NotFound(new {
                ok = false,
                message = "El artículo que intenta buscar no existe en el sistema"
            });
        }
        return Ok(new ArticuloViewModel {
            idArticulo = articulo.idArticulo,
            idCategoria = articulo.idCategoria,
            categoria = articulo.categoria.nombre,
            nombre = articulo.nombre,
            descripcion = articulo.descripcion,
            codigo = articulo.codigo,
            precio_venta = articulo.precio_venta,
            stock = articulo.stock,
            activo = articulo.activo
        });
    }

    // GET: api/Articulos/BuscarCodigoIngreso/12345
    [HttpGet("[action]/{codigo}")]
    public async Task<ActionResult> BuscarCodigoIngreso([FromRoute] string codigo)
    {
        if (codigo.Length <= 0) {
            return BadRequest(new {
                ok = false,
                message = "El código proporcionado del articulo es inválido"
            });
        }
        var articulo = await _context.Articulos.Include(a => a.categoria)
                                               .Where(a => a.activo == true)
                                               .SingleOrDefaultAsync(a => a.codigo == codigo);
        if (articulo == null)
        {
            return NotFound(new {
                ok = false,
                message = "El artículo que intenta buscar no existe en el sistema"
            });
        }
        return Ok(new ArticuloViewModel {
            idArticulo = articulo.idArticulo,
            idCategoria = articulo.idCategoria,
            categoria = articulo.categoria.nombre,
            nombre = articulo.nombre,
            descripcion = articulo.descripcion,
            codigo = articulo.codigo,
            precio_venta = articulo.precio_venta,
            stock = articulo.stock,
            activo = articulo.activo
        });
    }
    
    // PUT: api/Articulos/Actualizar
    [HttpPut("[action]")]
    public async Task<ActionResult> Actualizar([FromBody] ArticuloActualizarModel model) 
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (model.idArticulo <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El artículo que intenta actualizar no se encuentra en el sistema"
            });
        }
        var articulo = await _context.Articulos.FirstOrDefaultAsync(a => a.idArticulo == model.idArticulo);

        if (articulo == null)
        {
            return NotFound(new {
                ok = false,
                message = "El artículo que intenta actualizar no se encuentra en el sistema"
            });
        }

        articulo.idCategoria = model.idCategoria;
        articulo.codigo = model.codigo;
        articulo.nombre = model.nombre;
        articulo.precio_venta = model.precio_venta;
        articulo.stock = model.stock;
        articulo.descripcion = model.descripcion;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Guardar Excepcion
            return NotFound(new {
                ok = false,
                message = "Hubo un error al actualizar el artículo, intente más tarde"
            });
        }

        List<ArticuloViewModel> articulosModel = await ListaDeArticulos();
        return Ok(new {
            ok = true,
            message = "El artículo se ha actualizado correctamente",
            articulos = articulosModel
        });
    }

    // POST: api/Articulos/Crear
    [HttpPost("[action]")]
    public async Task<IActionResult> Crear([FromBody] ArticuloCrearModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Articulo articulo = new Articulo 
        {
            idCategoria = model.idCategoria,
            codigo = model.codigo,
            nombre = model.nombre,
            descripcion = model.descripcion,
            precio_venta = model.precio_venta,
            stock = model.stock,
            activo = true
        };
        _context.Articulos.Add(articulo);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su artículo, inténtelo más tarde"
            });
        }
                
        List<ArticuloViewModel> articulosModel = await ListaDeArticulos();
        return Ok(new {
            ok = true,
            message = "El artículo se ha creado correctamente",
            articulos = articulosModel
        });
    }

    // PUT: api/Articulos/Desactivar/1
    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> Desactivar([FromRoute] int id) 
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id del artículo que ha proporcionado es inválido"
            });
        }
        var articulo = await _context.Articulos.FirstOrDefaultAsync(c => c.idArticulo == id);

        if (articulo == null)
        {
            return NotFound(new {
                ok = false,
                message = "El artículo que intenta desactivar no se encuentra en el sistema"
            });
        }

        articulo.activo = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Guardar Excepcion
            return NotFound(new {
                ok = false,
                message = "Hubo un error al desactivar el artículo, intente más tarde"
            });
        }
        return Ok(new {
            Ok = true,
            message = "El artículo se ha desactivado exitosamente!"
        });
    }

    // PUT: api/Articulos/Activar/1
    [HttpPut("[action]/{id}")]
    public async Task<ActionResult> Activar([FromRoute] int id) 
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id del artículo que ha proporcionado es inválido"
            });
        }
        var articulo = await _context.Articulos.FirstOrDefaultAsync(c => c.idArticulo == id);

        if (articulo == null)
        {
            return NotFound(new {
                ok = false,
                message = "El artículo que intenta activar no se encuentra en el sistema"
            });
        }

        articulo.activo = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Guardar Excepcion
            return NotFound(new {
                ok = false,
                message = "Hubo un error al activar el artículo, intente más tarde"
            });
        }
        return Ok(new {
            Ok = true,
            message = "El artículo se ha activado exitosamente!"
        });
    }

    [HttpGet("[action]/{hint}")]
    public async Task<ActionResult> Filtrar([FromRoute] string hint, [FromQuery] PaginationParameters filterParametros)
    {
        if (string.IsNullOrEmpty(hint))
        {
            return BadRequest(new {
                ok = false,
                message = "Error al filtrar, el filtro no tiene ningún caracter"
            });
        }
        var items = await _context.Articulos.Where(a => a.nombre.ToLower().Contains(hint.ToLower()))
                                            .Include(a => a.categoria)
                                            .ToListAsync();
        var articulos = PagedList<Articulo>.ToPagedList(items, filterParametros.PageNumber, filterParametros.PageSize);
        // Response headers para la paginación
        var metadata = new
	    {
            articulos.TotalCount,
            articulos.PageSize,
            articulos.CurrentPage,
            articulos.TotalPages,
            articulos.HasNext,
            articulos.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        return Ok(articulos.Select(a => new ArticuloViewModel {
            idArticulo = a.idArticulo,
            idCategoria = a.idCategoria,
            nombre = a.nombre,
            descripcion = a.descripcion,
            codigo = a.codigo,
            precio_venta = a.precio_venta,
            stock = a.stock,
            categoria = a.categoria.nombre,
            activo = a.activo
        }));
    }    
    
    // Retornar lista completa de articulos actualizada con su respectivo model y paginación
    public async Task<List<ArticuloViewModel>> ListaDeArticulos () {
        // Se obtienen todas los articulos con la recien creada que se mostraran al usuarios
        var articulos = await this.Listar(new PaginationParameters { PageNumber = 1, PageSize = 10 });
        
        List<ArticuloViewModel> articulosModel = new List<ArticuloViewModel>();
        foreach (var item in articulos)
        {
            articulosModel.Add(new ArticuloViewModel {
                idArticulo = item.idArticulo,
                idCategoria = item.idCategoria,
                categoria = item.categoria,
                nombre = item.nombre,
                descripcion = item.descripcion,
                codigo = item.codigo,
                precio_venta = item.precio_venta,
                stock = item.stock,
                activo = item.activo
             });
        }
        return articulosModel;
    }

}