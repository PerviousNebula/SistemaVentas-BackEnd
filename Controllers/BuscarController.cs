using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrador")]
[Route("api/[controller]")]
[ApiController]
public class BuscarController : ControllerBase {
    private readonly DbContextSistema _context;

    public BuscarController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Buscar/FiltrarTodo/123
    [HttpGet("[action]/{filtro}")]
    public async Task<IActionResult> FiltrarTodo([FromRoute] string filtro)
    {
        BuscarViewModel model = new BuscarViewModel();
        model.categorias = await _context.Categorias.Where(c => c.nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                                c.descripcion.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                                                    .Select(c => new CategoriaViewModel {
                                                        idCategoria = c.idCategoria,
                                                        nombre = c.nombre,
                                                        descripcion = c.descripcion,
                                                        activo = c.activo
                                                    }).ToListAsync();
        model.articulos = await _context.Articulos.Where(a => a.nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                              a.descripcion.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                                                  .Select(a => new ArticuloViewModel {
                                                      idArticulo = a.idArticulo,
                                                      nombre = a.nombre,
                                                      descripcion = a.descripcion,
                                                      activo = a.activo
                                                  }).ToListAsync();
        model.ingresos = await _context.Ingresos.Where(i => i.serie_comprobante.Contains(filtro) || i.num_comprobante.Contains(filtro))
                                                .Select(i => new IngresoViewModel {
                                                    idIngreso = i.idIngreso,
                                                    num_comprobante = i.num_comprobante,
                                                    serie_comprobante = i.serie_comprobante,
                                                    estado = i.estado
                                                }).ToListAsync();
        model.proveedores = await _context.Personas.Where(p => p.tipo_persona == "Proveedor" &&
                                                               p.nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                               p.email.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                                                    .Select(p => new PersonaViewModel {
                                                        idPersona = p.idPersona,
                                                        nombre = p.nombre,
                                                        email = p.email,
                                                        num_documento = p.num_documento
                                                    }).ToListAsync();
        model.ventas = await _context.Ventas.Where(v => v.serie_comprobante.Contains(filtro) || v.num_comprobante.Contains(filtro))
                                            .Select(v => new VentaViewModel {
                                                idVenta = v.idVenta,
                                                num_comprobante = v.num_comprobante,
                                                serie_comprobante = v.serie_comprobante,
                                                estado = v.estado
                                            }).ToListAsync();
        model.clientes = await _context.Personas.Where(p => p.tipo_persona == "Cliente" &&
                                                            p.nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                            p.email.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                                                .Select(p => new PersonaViewModel {
                                                    idPersona = p.idPersona,
                                                    nombre = p.nombre,
                                                    email = p.email,
                                                    num_documento = p.num_documento
                                                }).ToListAsync();
        model.usuarios = await _context.Usuarios.Where(u => u.nombre.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                            u.email.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                                                .Select(u => new UsuarioViewModel {
                                                    idUsuario = u.idUsuario,
                                                    imgUrl = u.imgUrl,
                                                    nombre = u.nombre,
                                                    email = u.email,
                                                    activo = u.activo
                                                }).ToListAsync();
        model.sinResultados = model.categorias.Count == 0 && model.articulos.Count == 0 && model.ingresos.Count == 0 && model.proveedores.Count == 0 &&
                              model.ventas.Count == 0 && model.clientes.Count == 0 && model.usuarios.Count == 0;
        return Ok(model);
    }
}