using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/pdfcreator")]
[ApiController]
public class PdfCreatorController : ControllerBase
{
    private IConverter _converter;
    private readonly DbContextSistema _context;
 
    public PdfCreatorController(IConverter converter, DbContextSistema context)
    {
        _converter = converter;
        _context = context;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Articulos([FromQuery] ArticuloFilterModel filter, bool filtered = true)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };
        var items = await _context.Articulos.Include(a => a.categoria).ToListAsync();
        if (items == null)
        {
            return NotFound(new {
                ok = false,
                message = "No se encontraron resultados en su búsqueda"
            });
        }
        // Se aplican los filtros solamente si el usuario los mando en el modelo
        if (filtered)
        {
            if (!string.IsNullOrEmpty(filter.nombre)) { items = items.Where(i => i.nombre.IndexOf(filter.nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.descripcion)) { items = items.Where(i => i.descripcion.IndexOf(filter.descripcion, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.codigo)) { items = items.Where(i => i.codigo.IndexOf(filter.codigo, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (filter.stock > 0) { items = items.Where(i => i.stock >= filter.stock).ToList(); }
            if (filter.precio_min > 0) { items = items.Where(i => i.precio_venta >= filter.precio_min).ToList(); }
            if (filter.precio_max > 0) { items = items.Where(i => i.precio_venta <= filter.precio_max).ToList(); }
            if (filter.idCategoria > 0) { items = items.Where(i => i.idCategoria == filter.idCategoria).ToList(); }
            items = items.Where(i => i.activo == filter.activo).ToList();
        }
        var model = items.Select(art => new ArticuloPDF {
            nombre = art.nombre,
            categoria = art.categoria.nombre,
            codigo = art.codigo,
            precio_venta = art.precio_venta,
            stock = art.stock,
            descripcion = art.descripcion,
            activo = art.activo
        }).OrderBy(art => art.nombre).ToList();

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetArticulosHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Categorias([FromQuery] CategoriaFilterModel filter, bool filtered = true)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };
        List<Categoria> items;
        if (!filtered)
        {
            items = await _context.Categorias.OrderBy(c => c.nombre).ToListAsync(); 
        }
        else
        {
            items = await _context.Categorias.Where(c =>  c.nombre.Contains(filter.nombre != null ? filter.nombre : string.Empty) && 
                                                    c.descripcion.Contains(filter.descripcion != null ? filter.descripcion : string.Empty) && 
                                                    c.activo == filter.activo)
                                             .OrderBy(c => c.nombre)
                                             .ToListAsync();
        }

        if (items == null)
        {
            return BadRequest(new {
                ok = false,
                message = "No se encontraron resultados en su busqueda"
            });
        }

        var model = items.Select(cat => new CategoriaPDF {
            nombre = cat.nombre,
            descripcion = cat.descripcion,
            activo = cat.activo
        }).ToList();

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetCategoriasHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Ingresos([FromQuery] IngresoFilterModel filter, bool filtered = true)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Landscape,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = await _context.Ingresos.Include(i => i.usuario).Include(i => i.proveedor).OrderByDescending(i => i.idIngreso).ToListAsync();

        if (items == null)
        {
            return NotFound(new {
                ok = false,
                message = "No se encontraron resultados en su búsqueda"
            });
        }
        
        // Se aplican los filtros que se hayan mandado en el modelo
        if (filtered) {
            if (filter.idProveedor > 0) { items = items.Where(i => i.idProveedor == filter.idProveedor).ToList(); }
            if (!string.IsNullOrEmpty(filter.tipo_comprobante)) { items = items.Where(i => i.tipo_comprobante.IndexOf(filter.tipo_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.serie_comprobante)) { items = items.Where(i => i.serie_comprobante.IndexOf(filter.serie_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.num_comprobante)) { items = items.Where(i => i.num_comprobante.IndexOf(filter.num_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (filter.fecha_inicio != null) { items = items.Where(i => i.fecha_hora >= filter.fecha_inicio).ToList(); }
            if (filter.fecha_fin != null) { items = items.Where(i => i.fecha_hora <= filter.fecha_fin).ToList(); }
            if (filter.activo) { items = items.Where(i => i.estado.IndexOf("aceptado", StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!filter.activo) { items = items.Where(i => i.estado.IndexOf("anulado", StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        }
        
        var model = items.Select(i => new IngresosPDF {
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
        }).ToList();

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetIngresosHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Proveedores([FromQuery] ProveedorFilterModel filter, bool filtered = true)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = await _context.Personas.Where(c => c.tipo_persona == "proveedor").ToListAsync();
        
        if (items == null)
        {
            return NotFound(new {
                ok = false,
                message = "No se encontraron resultados en su busqueda"
            });
        }
        
        if (filtered)
        {
            if (!string.IsNullOrEmpty(filter.nombre)) { items = items.Where(i => i.nombre.IndexOf(filter.nombre, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.direccion)) { items = items.Where(i => i.direccion.IndexOf(filter.direccion, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.telefono)) { items = items.Where(i => i.telefono.IndexOf(filter.telefono, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.email)) { items = items.Where(i => i.nombre.IndexOf(filter.email, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        }
        var model = items.Select(p => new ProveedoresPDF {
            nombre = p.nombre,
            tipo_persona = p.tipo_persona,
            tipo_documento = p.tipo_documento,
            num_documento = p.num_documento,
            direccion = p.direccion,
            telefono = p.telefono,
            email = p.email
        }).ToList();

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetProveedoresHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Ventas([FromQuery] string filter)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Landscape,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = (string.IsNullOrEmpty(filter)) ? await _context.Ventas.Include(v => v.usuario)
                                                                          .Include(v => v.persona)
                                                                          .OrderByDescending(v => v.idVenta)
                                                                          .ToListAsync()
                                                   : await _context.Ventas.Where(v => v.serie_comprobante == filter)
                                                                          .Include(v => v.usuario)
                                                                          .Include(v => v.persona)
                                                                          .ToListAsync();
        var model = items.Select(v => new VentasPDF {
            cliente = v.persona.nombre,
            usuario = v.usuario.nombre,
            tipo_comprobante = v.tipo_comprobante,
            serie_comprobante = v.serie_comprobante,
            num_comprobante = v.num_comprobante,
            fecha_hora = v.fecha_hora,
            impuesto = v.impuesto,
            total = v.total,
            estado = v.estado
        }).ToList();

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetVentasHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Venta([FromRoute] int id)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Landscape,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };
        
        // Se obtiene la venta del id proporcionado
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
        
        var model = new VentaPDF {
            cliente = venta.persona.nombre,
            num_documento = venta.persona.num_documento,
            direccion = venta.persona.direccion,
            telefono = venta.persona.telefono,
            email = venta.persona.email,
            usuario = venta.usuario.nombre,
            tipo_comprobante = venta.tipo_comprobante,
            serie_comprobante = venta.serie_comprobante,
            num_comprobante = venta.num_comprobante,
            fecha_hora = venta.fecha_hora,
            impuesto = venta.impuesto,
            total = venta.total,
            estado = venta.estado
        };

        // Se obtienen los detalles de la venta obtenida
        var detalles = await _context.DetallesVentas.Include(a => a.articulo)
                                                    .Where(d => d.idVenta == id)
                                                    .ToListAsync();
        IEnumerable<DetalleVentaPDF> detallesModel = detalles.Select(d => new DetalleVentaPDF {
            articulo = d.articulo.nombre,
            cantidad = d.cantidad,
            precio = d.precio,
            descuento = d.descuento
        });
        model.detalles = detallesModel;

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetVentaHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Clientes([FromQuery] string filter)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = (string.IsNullOrEmpty(filter)) ? await _context.Personas.Where(p => p.tipo_persona == "Cliente")
                                                                            .OrderBy(r => r.nombre)
                                                                            .ToListAsync()
                                                   : await _context.Personas.Where(c => c.nombre.ToLower().Contains(filter.ToLower()) && c.tipo_persona == "Cliente")
                                                                            .ToListAsync();
        var model = items.Select(c => new ClientesPDF {
            nombre = c.nombre,
            tipo_persona = c.tipo_persona,
            tipo_documento = c.tipo_documento,
            num_documento = c.num_documento,
            direccion = c.direccion,
            telefono = c.telefono,
            email = c.email
        }).ToList();

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = TemplateGenerator.GetClientesHTMLString(model),
            WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Página [page] de [toPage]", Line = true },
            FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Sistema de Inventario ADMINPRO" }
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };

        try
        {
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        catch (AggregateException ex)
        {
            return BadRequest(new {
                ok = false,
                message = ex.Message
            });
        }
    }

}