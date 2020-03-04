using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class PersonasController : ControllerBase
{
    private readonly DbContextSistema _context;

    public PersonasController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Personas/ListarClientes
    [Authorize(Roles = "Administrador, Vendedor")]
    [HttpGet("[action]")]
    public async Task<IEnumerable<PersonaViewModel>> ListarClientes([FromQuery] PaginationParameters pagParams)
    {
        var personas = PagedList<Persona>.ToPagedList(await _context.Personas.Where(p => p.tipo_persona == "Cliente")
                                                                             .OrderBy(r => r.nombre)
                                                                             .ToListAsync(),
                                                      pagParams.PageNumber,
                                                      pagParams.PageSize);
        var metadata = new
	    {
            personas.TotalCount,
            personas.PageSize,
            personas.CurrentPage,
            personas.TotalPages,
            personas.HasNext,
            personas.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return personas.Select(p => new PersonaViewModel {
            idPersona = p.idPersona,
            nombre = p.nombre,
            tipo_persona = p.tipo_persona,
            tipo_documento = p.tipo_documento,
            num_documento = p.num_documento,
            direccion = p.direccion,
            telefono = p.telefono,
            email = p.email
        });
    }

    // GET: api/Personas/ListarProveedores
    [Authorize(Roles = "Administrador, Almacenero")]
    [HttpGet("[action]")]
    public async Task<IEnumerable<PersonaViewModel>> ListarProveedores([FromQuery] PaginationParameters pagParams)
    {
        var personas = PagedList<Persona>.ToPagedList(await _context.Personas.Where(p => p.tipo_persona == "Proveedor")
                                                                             .OrderBy(r => r.nombre)
                                                                             .ToListAsync(),
                                                      pagParams.PageNumber,
                                                      pagParams.PageSize);
        var metadata = new
	    {
            personas.TotalCount,
            personas.PageSize,
            personas.CurrentPage,
            personas.TotalPages,
            personas.HasNext,
            personas.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return personas.Select(p => new PersonaViewModel {
            idPersona = p.idPersona,
            nombre = p.nombre,
            tipo_persona = p.tipo_persona,
            tipo_documento = p.tipo_documento,
            num_documento = p.num_documento,
            direccion = p.direccion,
            telefono = p.telefono,
            email = p.email
        });
    }

    // POST: api/Personas/Crear
    [Authorize(Roles = "Administrador, Almacenero, Vendedor")]
    [HttpPost("[action]")]
    public async Task<IActionResult> Crear([FromBody] PersonaCrearModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string email = model.email.ToLower();

        if (await _context.Personas.AnyAsync(p => p.email == email))
        {
            return BadRequest(new {
                ok = false,
                message = "El email ya existe"
            });
        }
        
        Persona persona = new Persona
        {
            nombre = model.nombre,
            tipo_persona = model.tipo_persona,
            tipo_documento = model.tipo_documento,
            num_documento = model.num_documento,
            direccion = model.direccion,
            telefono = model.telefono,
            email = model.email
        };
        
        _context.Personas.Add(persona);

        try
        {
            await _context.SaveChangesAsync();
        } catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear la persona, inténtelo más tarde"
            });
        }
        
        List<PersonaViewModel> personasModel = (model.tipo_persona == "cliente") ? await ListaDePersonas(true) : 
                                                                                   await ListaDePersonas(false);        
        return Ok(new {
            ok = true,
            message = "El" + model.tipo_persona + "se ha creado exitosamente!",
            personas = personasModel
        });
    }

    // PUT: api/Personas/Actualizar
    [Authorize(Roles = "Administrador, Almacenero, Vendedor")]
    [HttpPut("[action]")]
    public async Task<IActionResult> Actualizar([FromBody] PersonaActualizarModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (model.idPersona <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id del "+ model.tipo_persona +" proporcionado es inválido"
            });
        }

        var persona = await _context.Personas.FirstOrDefaultAsync(p => p.idPersona == model.idPersona);

        if (persona == null)
        {
            return NotFound(new {
                ok = false,
                message = "El "+ model.tipo_persona +" que intenta actualizar no se encuentra en el sistema"
            });
        }

        persona.nombre = model.nombre;
        persona.tipo_persona = model.tipo_persona;
        persona.tipo_documento = model.tipo_documento;
        persona.num_documento = model.num_documento;
        persona.direccion = model.direccion;
        persona.telefono = model.telefono;
        persona.email = model.email;

        try
        {
            await _context.SaveChangesAsync();
        } 
        catch(DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al actualizar, inténtelo más tarde"
            });
        }

        List<PersonaViewModel> personasModel = (model.tipo_persona == "cliente") ? await ListaDePersonas(true) : 
                                                                                   await ListaDePersonas(false);

        return Ok(new {
            ok = true,
            message = "El " + model.tipo_persona + " se ha actualizado exitosamente!",
            personas = personasModel
        });
    }

    // GET: api/Personas/FiltrarClientes/arturo
    [Authorize(Roles = "Administrador, Vendedor")]
    [HttpGet("[action]/{hint}")]
    public async Task<ActionResult> FiltrarClientes([FromRoute] string hint, [FromQuery] PaginationParameters filterParametros)
    {
        if (string.IsNullOrEmpty(hint))
        {
            return BadRequest(new {
                ok = false,
                message = "Error al filtrar, el filtro no tiene ningún caracter"
            });
        }
        var items = await _context.Personas.Where(c => c.nombre.ToLower().Contains(hint.ToLower()) &&
                                                       c.tipo_persona == "Cliente"
                                                ).ToListAsync();
        var personas = PagedList<Persona>.ToPagedList(items, filterParametros.PageNumber, 10);
        // Response headers para la paginación
        var metadata = new
	    {
            personas.TotalCount,
            personas.PageSize,
            personas.CurrentPage,
            personas.TotalPages,
            personas.HasNext,
            personas.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        return Ok(personas.Select(p => new PersonaViewModel {
            idPersona = p.idPersona,
            nombre = p.nombre,
            tipo_persona = p.tipo_persona,
            tipo_documento = p.tipo_documento,
            num_documento = p.num_documento,
            direccion = p.direccion,
            telefono = p.telefono,
            email = p.email
        }));
    }    
    
    // GET: api/Personas/FiltrarClientes/arturo
    [Authorize(Roles = "Administrador, Almacenero")]
    [HttpGet("[action]/{hint}")]
    public async Task<ActionResult> FiltrarProveedores([FromRoute] string hint, [FromQuery] PaginationParameters filterParametros)
    {
        if (string.IsNullOrEmpty(hint))
        {
            return BadRequest(new {
                ok = false,
                message = "Error al filtrar, el filtro no tiene ningún caracter"
            });
        }
        var items = await _context.Personas.Where(c => c.nombre.ToLower().Contains(hint.ToLower()) && c.tipo_persona == "proveedor")
                                           .ToListAsync();
        var personas = PagedList<Persona>.ToPagedList(items, filterParametros.PageNumber, 10);
        // Response headers para la paginación
        var metadata = new
	    {
            personas.TotalCount,
            personas.PageSize,
            personas.CurrentPage,
            personas.TotalPages,
            personas.HasNext,
            personas.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        
        return Ok(personas.Select(p => new PersonaViewModel {
            idPersona = p.idPersona,
            nombre = p.nombre,
            tipo_persona = p.tipo_persona,
            tipo_documento = p.tipo_documento,
            num_documento = p.num_documento,
            direccion = p.direccion,
            telefono = p.telefono,
            email = p.email
        }));
    }        
    
    // Retornar lista completa de personas actualizada con su respectivo model y paginación
    public async Task<List<PersonaViewModel>> ListaDePersonas(bool cliente = true) {
        IEnumerable<PersonaViewModel> personas;
        PaginationParameters pagParams = new PaginationParameters {PageNumber = 1, PageSize = 10 };
        personas = (cliente) ? await this.ListarClientes(pagParams)
                             : await this.ListarProveedores(pagParams);

        List<PersonaViewModel> personasModel = new List<PersonaViewModel>();
        foreach (var item in personas)
        {
            personasModel.Add(new PersonaViewModel {
                idPersona = item.idPersona,
                nombre = item.nombre,
                tipo_persona = item.tipo_persona,
                tipo_documento = item.tipo_documento,
                num_documento = item.num_documento,
                direccion = item.direccion,
                telefono = item.telefono,
                email = item.email
            });
        }
        return personasModel;
    }
    
}