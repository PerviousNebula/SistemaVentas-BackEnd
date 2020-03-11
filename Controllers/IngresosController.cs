using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador, Almacenero")]
public class IngresosController : ControllerBase
{
    private readonly DbContextSistema _context;

    public IngresosController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Ingresos/Listar
    [HttpGet("[action]")]
    public async Task<IEnumerable<IngresoViewModel>> Listar([FromQuery] PaginationParameters pagParams)
    {
        var ingresos = PagedList<Ingreso>.ToPagedList(await _context.Ingresos.Include(i => i.usuario)
                                                                             .Include(i => i.proveedor)
                                                                             .OrderByDescending(i => i.idIngreso)
                                                                             .ToListAsync(),
                                                     pagParams.PageNumber,
                                                     pagParams.PageSize);
        var metadata = new
	    {
            ingresos.TotalCount,
            ingresos.PageSize,
            ingresos.CurrentPage,
            ingresos.TotalPages,
            ingresos.HasNext,
            ingresos.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return ingresos.Select(i => new IngresoViewModel {
            idIngreso = i.idIngreso,
            idProveedor = i.idProveedor,
            proveedor = i.proveedor.nombre,
            idUsuario = i.idUsuario,
            usuario = i.usuario.nombre,
            tipo_comprobante = i.tipo_comprobante,
            serie_comprobante = i.serie_comprobante,
            num_comprobante = i.num_comprobante,
            fecha_hora = i.fecha_hora,
            impuesto = i.impuesto,
            total = i.total,
            estado = i.estado
        });
    }

    // GET: api/Ingresos/Mostrar/2
    [HttpGet("[action]/{id}")]
    public async Task<ActionResult> Mostrar([FromRoute] int id) {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id del ingreso que ha proporcionado es inválido"
            });
        }

        var ingreso = await _context.Ingresos.Include(i => i.usuario)
                                             .Include(i => i.proveedor)
                                             .Include(i => i.detalles)
                                             .FirstOrDefaultAsync(i => i.idIngreso == id);
        if (ingreso == null)
        {
            return BadRequest(new {
                ok = false,
                message = "El ingreso que busca no se encuentra en el sistema"
            });
        }

        IEnumerable<DetalleViewModel> detallesModel = await this.ListarDetalles(id);

        return Ok(new IngresoViewModel {
            idIngreso = ingreso.idIngreso,
            idProveedor = ingreso.idProveedor,
            proveedor = ingreso.proveedor.nombre,
            idUsuario = ingreso.idUsuario,
            usuario = ingreso.usuario.nombre,
            tipo_comprobante = ingreso.tipo_comprobante,
            serie_comprobante = ingreso.serie_comprobante,
            num_comprobante = ingreso.num_comprobante,
            fecha_hora = ingreso.fecha_hora,
            impuesto = ingreso.impuesto,
            total = ingreso.total,
            detalles = detallesModel,
            estado = ingreso.estado
        });
    }

    // POST: api/Ingresos/Crear
    [HttpPost("[action]")]
    public async Task<IActionResult> Crear([FromBody] IngresoCrearModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var fechaActual = DateTime.Now;
        Ingreso ingreso = new Ingreso {
            idProveedor = model.idProveedor,
            idUsuario = model.idUsuario,
            tipo_comprobante = model.tipo_comprobante,
            serie_comprobante = model.serie_comprobante,
            num_comprobante = model.num_comprobante,
            fecha_hora = fechaActual,
            impuesto = model.impuesto,
            total = model.total,
            estado = "Aceptado"
        };
        try
        {
            _context.Ingresos.Add(ingreso);
            await _context.SaveChangesAsync();
            int idIngreso = ingreso.idIngreso;
            foreach (var item in model.detalles)
            {
                DetalleIngreso detalle = new DetalleIngreso {
                    idIngreso = idIngreso,
                    idArticulo = item.idArticulo,
                    precio = item.precio,
                    cantidad = item.cantidad
                };
                _context.DetallesIngresos.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su ingreso, inténtelo más tarde"
            });
        }
                
        var ingresos = await this.Listar(new PaginationParameters { PageNumber = 1, PageSize = 10 });
        return Ok(new {
            ok = true,
            message = "El ingreso se ha creado correctamente",
            ingresos
        });
    }

    // PUT: api/Ingresos/Actualizar
    [HttpPut("[action]")]
    public async Task<IActionResult> Actualizar([FromBody] IngresoActualizarModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ingreso = await _context.Ingresos.Include(i => i.usuario)
                                             .Include(i => i.proveedor)
                                             .Include(i => i.detalles)
                                             .FirstOrDefaultAsync(i => i.idIngreso == model.idIngreso);

        if (ingreso == null)
        {
            return BadRequest(new {
                ok = false,
                message = "El ingreso que busca no se encuentra en el sistema"
            });
        }

        try
        {
            ingreso.idProveedor = model.idProveedor;
            ingreso.tipo_comprobante = model.tipo_comprobante;
            ingreso.serie_comprobante = model.serie_comprobante;
            ingreso.num_comprobante = model.num_comprobante;
            ingreso.impuesto = model.impuesto;
            ingreso.total = model.total;
            await _context.SaveChangesAsync();
        } 
        catch(Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al actualizar su ingreso, inténtelo más tarde"
            });
        }
        return Ok(new {
            ok = true,
            message = "El ingreso se ha actualizado correctamente"
        });
    }

    // POST: api/Ingresos/Filtrar/1234    
    [HttpPost("[action]")]
    public async Task<ActionResult> Filtrar([FromBody] IngresoFilterModel model, [FromQuery] PaginationParameters filterParametros)
    {
        if (model == null)
        {
            return BadRequest(new {
                ok = false,
                message = "Error al filtrar, el filtro es nulo"
            });
        }
        
        var items = await _context.Ingresos.Include(i => i.usuario).Include(i => i.proveedor).ToListAsync();
        
        if (items == null)
        {
            return NotFound(new {
                ok = false,
                message = "No se encontraron resultados en su búsqueda"
            });
        }

        // Se aplican los filtros que se hayan mandado en el modelo
        if (model.idProveedor > 0) { items = items.Where(i => i.idProveedor == model.idProveedor).ToList(); }
        if (!string.IsNullOrEmpty(model.tipo_comprobante)) { items = items.Where(i => i.tipo_comprobante.IndexOf(model.tipo_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        if (!string.IsNullOrEmpty(model.serie_comprobante)) { items = items.Where(i => i.serie_comprobante.IndexOf(model.serie_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        if (!string.IsNullOrEmpty(model.num_comprobante)) { items = items.Where(i => i.num_comprobante.IndexOf(model.num_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        if (model.fecha_inicio != null) { items = items.Where(i => i.fecha_hora >= model.fecha_inicio).ToList(); }
        if (model.fecha_fin != null) { items = items.Where(i => i.fecha_hora <= model.fecha_fin).ToList(); }
        if (model.activo) { items = items.Where(i => i.estado.IndexOf("aceptado", StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        if (!model.activo) { items = items.Where(i => i.estado.IndexOf("anulado", StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }


        var ingresos = PagedList<Ingreso>.ToPagedList(items, filterParametros.PageNumber, filterParametros.PageSize);
        
        // Response headers para la paginación
        var metadata = new
	    {
            ingresos.TotalCount,
            ingresos.PageSize,
            ingresos.CurrentPage,
            ingresos.TotalPages,
            ingresos.HasNext,
            ingresos.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        return Ok(ingresos.Select(i => new IngresoViewModel {
            idIngreso = i.idIngreso,
            idProveedor = i.idProveedor,
            proveedor = i.proveedor.nombre,
            idUsuario = i.idUsuario,
            usuario = i.usuario.nombre,
            tipo_comprobante = i.tipo_comprobante,
            serie_comprobante = i.serie_comprobante,
            num_comprobante = i.num_comprobante,
            fecha_hora = i.fecha_hora,
            impuesto = i.impuesto,
            total = i.total,
            estado = i.estado
        }).OrderByDescending(i => i.idIngreso));
    }
    
    // GET: api/Ingresos/ListarDetalles
    [HttpGet("[action]/{idIngreso}")]
    public async Task<IEnumerable<DetalleViewModel>> ListarDetalles([FromRoute] int idIngreso)
    {
        var detalles = await _context.DetallesIngresos.Include(a => a.articulo)
                                                      .Where(d => d.idIngreso == idIngreso)
                                                      .ToListAsync();
        return detalles.Select(d => new DetalleViewModel {
            idArticulo = d.idArticulo,
            articulo = d.articulo.nombre,
            cantidad = d.cantidad,
            precio = d.precio
        });
    }

    // PUT: api/Ingresos/Desactivar/3
    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Desactivar([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id del ingreso que ha proporcionado es inválido"
            });
        }

        var ingreso = await _context.Ingresos.FirstOrDefaultAsync(i => i.idIngreso == id);

        if (ingreso == null)
        {
            return BadRequest(new {
                ok = false,
                message = "El ingreso que busca no se encuentra en el sistema"
            });
        }

        ingreso.estado = "Anulado";

        try
        {
            await _context.SaveChangesAsync();
            // Actualizar el stock del articulo
            var detalle = await _context.DetallesIngresos.Include(a => a.articulo)
                                                         .Where(d => d.idIngreso == id)
                                                         .ToListAsync();
            foreach (var det in detalle)
            {
                var articulo = await _context.Articulos.FirstOrDefaultAsync(a => a.idArticulo == det.articulo.idArticulo);
                articulo.stock = det.articulo.stock-det.cantidad;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al anular su ingreso, inténtelo más tarde"
            });
        }

        return Ok(new {
            ok = true,
            message = "El ingreso se ha anulado correctamente"
        });
    }

}