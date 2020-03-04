using System;
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
    public async Task<IActionResult> Articulos([FromQuery] string filter)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = (string.IsNullOrEmpty(filter)) ? await _context.Articulos.Include(a => a.categoria).OrderBy(a => a.nombre).ToListAsync()
                                                   : await _context.Articulos.Where(a => a.nombre.ToLower().Contains(filter.ToLower()))
                                                                             .Include(a => a.categoria)
                                                                             .ToListAsync();
        var model = items.Select(art => new ArticuloPDF {
            nombre = art.nombre,
            categoria = art.categoria.nombre,
            codigo = art.codigo,
            precio_venta = art.precio_venta,
            stock = art.stock,
            descripcion = art.descripcion,
            activo = art.activo
        }).ToList();

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
            return File(file, "application/pdf", "Reporte_Inventario.pdf");
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
    public async Task<IActionResult> Categorias([FromQuery] string filter)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = (string.IsNullOrEmpty(filter)) ? await _context.Categorias.OrderBy(c => c.nombre).ToListAsync()
                                                   : await _context.Categorias.Where(c => c.nombre.ToLower().Contains(filter.ToLower()))
                                                                              .ToListAsync();
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
            return File(file, "application/pdf", "Reporte_Inventario.pdf");
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
    public async Task<IActionResult> Ingresos([FromQuery] string filter)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Landscape,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = (string.IsNullOrEmpty(filter)) ? await _context.Ingresos.Include(i => i.usuario)
                                                                            .Include(i => i.proveedor)
                                                                            .OrderByDescending(i => i.idIngreso)
                                                                            .ToListAsync()
                                                   : await _context.Ingresos.Where(i => i.serie_comprobante == filter)
                                                                            .Include(i => i.usuario)
                                                                            .Include(i => i.proveedor)
                                                                            .ToListAsync();
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
            return File(file, "application/pdf", "Reporte_Inventario.pdf");
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
    public async Task<IActionResult> Proveedores([FromQuery] string filter)
    {
        var globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10 },
            DocumentTitle = "Reporte de Inventariado"
        };

        var items = (string.IsNullOrEmpty(filter)) ? await _context.Personas.Where(p => p.tipo_persona == "Proveedor")
                                                                             .OrderBy(r => r.nombre)
                                                                             .ToListAsync()
                                                   : await _context.Personas.Where(c => c.nombre.ToLower().Contains(filter.ToLower()) && c.tipo_persona == "proveedor")
                                                                            .ToListAsync();
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
            return File(file, "application/pdf", "Reporte_Inventario.pdf");
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