using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelLibrary.SpreadSheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/csvcreator")]
[ApiController]
public class CSVCreatorController : ControllerBase
{
    private readonly DbContextSistema _context;

    public CSVCreatorController(DbContextSistema context)
    {
        _context = context;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Categorias([FromQuery] CategoriaFilterModel filter, bool filtered = true)
    {
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
        // Se valida si la busqueda arrojo resultados
        if (items == null)
        {
            return BadRequest(new {
                ok = false,
                message = "No se encontraron resultados en su busqueda"
            });
        }

        string fileName = "reporte_inventario.xls";
        Workbook workbook = new Workbook();
        Worksheet worksheet = new Worksheet("Categorias");
        
        // Cabecera de la hoja de Excel
        worksheet.Cells[0, 0] = new Cell("Nombre");
        worksheet.Cells.ColumnWidth[0, 1] = 3000;
        worksheet.Cells[0, 1] = new Cell("Descripción");
        worksheet.Cells[0, 2] = new Cell("Estado");

        // Cuerpo de la hoja de Excel
        for (int i = 0; i < items.Count; i++)
        {
            worksheet.Cells[i+1, 0] = new Cell(items[i].nombre);
            worksheet.Cells[i+1, 1] = new Cell(items[i].descripcion);
            worksheet.Cells[i+1, 2] = new Cell(items[i].activo ? "ACTIVO" : "INACTIVO");
        }
        workbook.Worksheets.Add(worksheet);
        MemoryStream m = new MemoryStream();
        workbook.SaveToStream(m);
        return File(m.ToArray(), "application/vnd.ms-excel", fileName);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Articulos([FromQuery] ArticuloFilterModel filter, bool filtered = true)
    {
        var items = await _context.Articulos.Include(a => a.categoria).OrderBy(a => a.nombre).ToListAsync();
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
        
        string fileName = "reporte_inventario.xls";
        Workbook workbook = new Workbook();
        Worksheet worksheet = new Worksheet("Articulos");
        
        // Cabecera de la hoja de Excel
        worksheet.Cells[0, 0] = new Cell("Nombre");
        worksheet.Cells[0, 1] = new Cell("Descripción");
        worksheet.Cells[0, 2] = new Cell("Código");
        worksheet.Cells[0, 3] = new Cell("Precio");
        worksheet.Cells[0, 4] = new Cell("Stock");
        worksheet.Cells[0, 5] = new Cell("Categoría");
        worksheet.Cells[0, 6] = new Cell("Estado");

        // Cuerpo de la hoja de Excel
        for (int i = 0; i < items.Count; i++)
        {
            worksheet.Cells[i+1, 0] = new Cell(items[i].nombre);
            worksheet.Cells[i+1, 1] = new Cell(items[i].descripcion);
            worksheet.Cells[i+1, 2] = new Cell(items[i].codigo);
            worksheet.Cells[i+1, 3] = new Cell(items[i].precio_venta);
            worksheet.Cells[i+1, 4] = new Cell(items[i].stock);
            worksheet.Cells[i+1, 5] = new Cell(items[i].categoria.nombre);
            worksheet.Cells[i+1, 6] = new Cell(items[i].activo ? "ACTIVO" : "INACTIVO");
        }
        workbook.Worksheets.Add(worksheet);
        MemoryStream m = new MemoryStream();
        workbook.SaveToStream(m);
        return File(m.ToArray(), "application/vnd.ms-excel", fileName);
    }
        
    [HttpGet("[action]")]
    public async Task<IActionResult> Ingresos([FromQuery] IngresoFilterModel filter, bool filtered = true)
    {
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
        
        string fileName = "reporte_inventario.xls";
        Workbook workbook = new Workbook();
        Worksheet worksheet = new Worksheet("Ingresos");
        
        // Cabecera de la hoja de Excel
        worksheet.Cells[0, 0] = new Cell("Usuario");
        worksheet.Cells[0, 1] = new Cell("Proveedor");
        worksheet.Cells[0, 2] = new Cell("Tipo Comprobante");
        worksheet.Cells[0, 3] = new Cell("Serie Comprobante");
        worksheet.Cells[0, 4] = new Cell("No. Comprobante");
        worksheet.Cells[0, 5] = new Cell("Fecha");
        worksheet.Cells[0, 6] = new Cell("Total");
        worksheet.Cells[0, 7] = new Cell("Estado");

        // Cuerpo de la hoja de Excel
        for (int i = 0; i < items.Count; i++)
        {
            worksheet.Cells[i+1, 0] = new Cell(items[i].usuario.nombre);
            worksheet.Cells[i+1, 1] = new Cell(items[i].proveedor.nombre);
            worksheet.Cells[i+1, 2] = new Cell(items[i].tipo_comprobante);
            worksheet.Cells[i+1, 3] = new Cell(items[i].serie_comprobante);
            worksheet.Cells[i+1, 4] = new Cell(items[i].num_comprobante);
            worksheet.Cells[i+1, 5] = new Cell(items[i].fecha_hora.ToShortDateString());
            worksheet.Cells[i+1, 6] = new Cell("$"+ items[i].total);
            worksheet.Cells[i+1, 7] = new Cell(items[i].estado.ToUpper());
        }
        workbook.Worksheets.Add(worksheet);
        MemoryStream m = new MemoryStream();
        workbook.SaveToStream(m);
        return File(m.ToArray(), "application/vnd.ms-excel", fileName);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Proveedores([FromQuery] ProveedorFilterModel filter, bool filtered = true)
    {
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
        
        string fileName = "reporte_inventario.xls";
        Workbook workbook = new Workbook();
        Worksheet worksheet = new Worksheet("Proveedores");
        
        // Cabecera de la hoja de Excel
        worksheet.Cells[0, 0] = new Cell("Nombre");
        worksheet.Cells[0, 1] = new Cell("Direccion");
        worksheet.Cells[0, 2] = new Cell("Teléfono");
        worksheet.Cells[0, 3] = new Cell("Email");

        // Cuerpo de la hoja de Excel
        for (int i = 0; i < items.Count; i++)
        {
            worksheet.Cells[i+1, 0] = new Cell(items[i].nombre);
            worksheet.Cells[i+1, 1] = new Cell(items[i].direccion);
            worksheet.Cells[i+1, 2] = new Cell(items[i].telefono);
            worksheet.Cells[i+1, 3] = new Cell(items[i].email);
        }
        workbook.Worksheets.Add(worksheet);
        MemoryStream m = new MemoryStream();
        workbook.SaveToStream(m);
        return File(m.ToArray(), "application/vnd.ms-excel", fileName);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Ventas([FromQuery] VentaFilterModel filter, bool filtered = true)
    {
        var items = await _context.Ventas.Include(v => v.usuario).Include(v => v.persona).OrderByDescending(i => i.idVenta).ToListAsync();

        if (items == null)
        {
            return NotFound(new {
                ok = false,
                message = "No se encontraron resultados en su busqueda"
            });
        }
        
        if (filtered)
        {
            if (filter.idCliente > 0) { items = items.Where(i => i.idPersona == filter.idCliente).ToList(); }
            if (!string.IsNullOrEmpty(filter.tipo_comprobante)) { items = items.Where(i => i.tipo_comprobante.IndexOf(filter.tipo_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.serie_comprobante)) { items = items.Where(i => i.serie_comprobante.IndexOf(filter.serie_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!string.IsNullOrEmpty(filter.num_comprobante)) { items = items.Where(i => i.num_comprobante.IndexOf(filter.num_comprobante, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (filter.fecha_inicio != null) { items = items.Where(i => i.fecha_hora >= filter.fecha_inicio).ToList(); }
            if (filter.fecha_fin != null) { items = items.Where(i => i.fecha_hora <= filter.fecha_fin).ToList(); }
            if (filter.activo) { items = items.Where(i => i.estado.IndexOf("aceptado", StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
            if (!filter.activo) { items = items.Where(i => i.estado.IndexOf("anulado", StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        }

        string fileName = "reporte_inventario.xls";
        Workbook workbook = new Workbook();
        Worksheet worksheet = new Worksheet("Ventas");
        
        // Cabecera de la hoja de Excel
        worksheet.Cells[0, 0] = new Cell("Usuario");
        worksheet.Cells[0, 1] = new Cell("Cliente");
        worksheet.Cells[0, 2] = new Cell("Tipo Comprobante");
        worksheet.Cells[0, 3] = new Cell("Serie Comprobante");
        worksheet.Cells[0, 4] = new Cell("No. Comprobante");
        worksheet.Cells[0, 5] = new Cell("Fecha");
        worksheet.Cells[0, 6] = new Cell("Total");
        worksheet.Cells[0, 7] = new Cell("Estado");

        // Cuerpo de la hoja de Excel
        for (int i = 0; i < items.Count; i++)
        {
            worksheet.Cells[i+1, 0] = new Cell(items[i].usuario.nombre);
            worksheet.Cells[i+1, 1] = new Cell(items[i].persona.nombre);
            worksheet.Cells[i+1, 2] = new Cell(items[i].tipo_comprobante);
            worksheet.Cells[i+1, 3] = new Cell(items[i].serie_comprobante);
            worksheet.Cells[i+1, 4] = new Cell(items[i].num_comprobante);
            worksheet.Cells[i+1, 5] = new Cell(items[i].fecha_hora.ToShortDateString());
            worksheet.Cells[i+1, 6] = new Cell("$" + items[i].total);
            worksheet.Cells[i+1, 7] = new Cell(items[i].estado.ToUpper());
        }
        workbook.Worksheets.Add(worksheet);
        MemoryStream m = new MemoryStream();
        workbook.SaveToStream(m);
        return File(m.ToArray(), "application/vnd.ms-excel", fileName);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Clientes([FromQuery] ClientesFilterModel filter, bool filtered = true)
    {
        var items = await _context.Personas.Where(c => c.tipo_persona == "Cliente").ToListAsync();

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
            if (!string.IsNullOrEmpty(filter.email)) { items = items.Where(i => i.email.IndexOf(filter.email, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); }
        }

        string fileName = "reporte_inventario.xls";
        Workbook workbook = new Workbook();
        Worksheet worksheet = new Worksheet("Clientes");
        
        // Cabecera de la hoja de Excel
        worksheet.Cells[0, 0] = new Cell("Nombre");
        worksheet.Cells[0, 1] = new Cell("Dirección");
        worksheet.Cells[0, 2] = new Cell("Teléfono");
        worksheet.Cells[0, 3] = new Cell("Email");

        // Cuerpo de la hoja de Excel
        for (int i = 0; i < items.Count; i++)
        {
            worksheet.Cells[i+1, 0] = new Cell(items[i].nombre);
            worksheet.Cells[i+1, 1] = new Cell(items[i].direccion);
            worksheet.Cells[i+1, 2] = new Cell(items[i].telefono);
            worksheet.Cells[i+1, 3] = new Cell(items[i].email);
        }
        workbook.Worksheets.Add(worksheet);
        MemoryStream m = new MemoryStream();
        workbook.SaveToStream(m);
        return File(m.ToArray(), "application/vnd.ms-excel", fileName);
    }

}