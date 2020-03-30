using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrador, Almacenero, Vendedor")]
[Route("api/[controller]")]
[ApiController]
public class ChartController : ControllerBase
{
    private DbContextSistema _context;
 
    public ChartController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: Chart/ObtenerGraficas
    [HttpGet("[action]")]
    public async Task<IActionResult> ObtenerGraficas()
    {
        var ingresos = await _context.Ingresos.ToListAsync();
        if (ingresos == null)
        {
            return BadRequest(new { ok = false, message = "No existen ingresos en el sistema" });
        }
        var ventas = await _context.Ventas.ToListAsync();
        if (ventas == null)
        {
            return BadRequest(new { ok = false, message = "No existen ventas en el sistema" });
        }
        DashboardModel model = new DashboardModel { totalIngresos = ingresos.Where(i => i.estado == "Aceptado").Sum(i => i.total),
                                                    totalVentas = ventas.Where(v => v.estado == "Aceptado").Sum(v => v.total) };
        model.ingresos       = ingresos.GroupBy(i => i.fecha_hora.Month).Select(i => new ChartModel { Label = i.Key.ToString(), Data = i.Sum(i => i.total)});
        model.ventas         = ventas.GroupBy(v => v.fecha_hora.Month).Select(v => new ChartModel { Label = v.Key.ToString(), Data = v.Sum(v => v.total)});
        
        return Ok(model);
    }

}