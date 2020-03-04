using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class VentasController : ControllerBase
{
    private readonly DbContextSistema _context;

    public VentasController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Ventas/Listar
    [HttpGet("[action]")]
    public async Task<IEnumerable<VentaViewModel>> Listar([FromQuery] PaginationParameters pagParams)
    {
        var ventas = PagedList<Venta>.ToPagedList(await _context.Ventas.Include(v => v.usuario)
                                                                       .Include(v => v.persona)
                                                                       .OrderByDescending(v => v.idVenta)
                                                                       .ToListAsync(),
                                                     pagParams.PageNumber,
                                                     pagParams.PageSize);
        var metadata = new
	    {
            ventas.TotalCount,
            ventas.PageSize,
            ventas.CurrentPage,
            ventas.TotalPages,
            ventas.HasNext,
            ventas.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return ventas.Select(v => new VentaViewModel {
            idVenta = v.idVenta,
            idCliente = v.idPersona,
            cliente = v.persona.nombre,
            idUsuario = v.idUsuario,
            usuario = v.usuario.nombre,
            tipo_comprobante = v.tipo_comprobante,
            serie_comprobante = v.serie_comprobante,
            num_comprobante = v.num_comprobante,
            fecha_hora = v.fecha_hora,
            impuesto = v.impuesto,
            total = v.total,
            estado = v.estado
        });
    }

    // GET: api/Ventas/Mostrar/2
    [HttpGet("[action]/{id}")]
    public async Task<ActionResult> Mostrar([FromRoute] int id) {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id de la venta que ha proporcionado es inválido"
            });
        }

        var venta = await _context.Ventas.Include(v => v.usuario)
                                         .Include(v => v.persona)
                                         .Include(v => v.detalles)
                                         .FirstOrDefaultAsync(i => i.idVenta == id);
        if (venta == null)
        {
            return BadRequest(new {
                ok = false,
                message = "La venta que busca no se encuentra en el sistema"
            });
        }

        IEnumerable<DetalleVentaViewModel> detallesModel = await this.ListarDetalles(id);

        return Ok(new VentaViewModel {
            idVenta = venta.idVenta,
            idCliente = venta.idPersona,
            cliente = venta.persona.nombre,
            idUsuario = venta.idUsuario,
            usuario = venta.usuario.nombre,
            tipo_comprobante = venta.tipo_comprobante,
            serie_comprobante = venta.serie_comprobante,
            num_comprobante = venta.num_comprobante,
            fecha_hora = venta.fecha_hora,
            impuesto = venta.impuesto,
            total = venta.total,
            detalles = detallesModel,
            estado = venta.estado
        });
    }
    
    // GET: api/Ventas/ListarDetalles/8
    [HttpGet("[action]/{idVenta}")]
    public async Task<IEnumerable<DetalleVentaViewModel>> ListarDetalles([FromRoute] int idVenta)
    {
        var detalles = await _context.DetallesVentas.Include(a => a.articulo)
                                                    .Where(d => d.idVenta == idVenta)
                                                    .ToListAsync();
        return detalles.Select(d => new DetalleVentaViewModel {
            idArticulo = d.idArticulo,
            articulo = d.articulo.nombre,
            cantidad = d.cantidad,
            precio = d.precio,
            descuento = d.descuento
        });
    }
    
    // POST: api/Ventas/Crear
    [HttpPost("[action]")]
    public async Task<IActionResult> Crear([FromBody] VentaCrearModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fechaActual = DateTime.Now;
        
        Venta venta = new Venta {
            idPersona = model.idCliente,
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
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            int idVenta = venta.idVenta;
            foreach (var item in model.detalles)
            {
                DetalleVenta detalle = new DetalleVenta {
                    idVenta = idVenta,
                    idArticulo = item.idArticulo,
                    precio = item.precio,
                    cantidad = item.cantidad,
                    descuento = item.descuento
                };
                _context.DetallesVentas.Add(detalle);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su venta, inténtelo más tarde"
            });
        }
                
        var ventas = await this.Listar(new PaginationParameters { PageNumber = 1, PageSize = 10 });
        return Ok(new {
            ok = true,
            message = "La venta se ha creado correctamente",
            ventas
        });
    }

    // PUT: api/Ventas/Desactivar/3
    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Desactivar([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id de la venta que ha proporcionado es inválido"
            });
        }

        var venta = await _context.Ventas.FirstOrDefaultAsync(i => i.idVenta == id);

        if (venta == null)
        {
            return BadRequest(new {
                ok = false,
                message = "La venta que busca no se encuentra en el sistema"
            });
        }

        venta.estado = "Anulado";

        try
        {
            await _context.SaveChangesAsync();
            // Actualizar el stock del articulo
            var detalle = await _context.DetallesVentas.Include(a => a.articulo)
                                                       .Where(d => d.idVenta == id)
                                                       .ToListAsync();
            foreach (var det in detalle)
            {
                var articulo = await _context.Articulos.FirstOrDefaultAsync(a => a.idArticulo == det.articulo.idArticulo);
                articulo.stock = det.articulo.stock + det.cantidad;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al anular su venta, inténtelo más tarde"
            });
        }

        return Ok(new {
            ok = true,
            message = "La venta se ha anulado correctamente"
        });
    }
    
    // GET: api/Ventas/Filtrar/1234
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
        var items = await _context.Ventas.Where(v => v.serie_comprobante == hint)
                                           .Include(v => v.usuario)
                                           .Include(v => v.persona)
                                           .ToListAsync();
        var ventas = PagedList<Venta>.ToPagedList(items, filterParametros.PageNumber, filterParametros.PageSize);
        // Response headers para la paginación
        var metadata = new
	    {
            ventas.TotalCount,
            ventas.PageSize,
            ventas.CurrentPage,
            ventas.TotalPages,
            ventas.HasNext,
            ventas.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        return Ok(ventas.Select(v => new VentaViewModel {
            idVenta = v.idVenta,
            idCliente = v.idPersona,
            cliente = v.persona.nombre,
            idUsuario = v.idUsuario,
            usuario = v.usuario.nombre,
            tipo_comprobante = v.tipo_comprobante,
            serie_comprobante = v.serie_comprobante,
            num_comprobante = v.num_comprobante,
            fecha_hora = v.fecha_hora,
            impuesto = v.impuesto,
            total = v.total,
            estado = v.estado
        }));
    }

}
