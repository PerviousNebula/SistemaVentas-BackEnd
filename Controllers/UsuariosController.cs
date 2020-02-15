using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class UsuariosController : ControllerBase
{
    private readonly DbContextSistema _context;

    public UsuariosController(DbContextSistema context)
    {
        _context = context;
    }

    // GET: api/Usuarios/Listar
    [HttpGet("[action]")]
    public async Task<IEnumerable<UsuarioViewModel>> Listar([FromQuery] PaginationParameters pagParams)
    {
        var usuarios = PagedList<Usuario>.ToPagedList(await _context.Usuarios.Include(r => r.rol)
                                                                             .OrderBy(r => r.nombre)
                                                                             .ToListAsync(),
                                                      pagParams.PageNumber,
                                                      pagParams.PageSize);
        var metadata = new
	    {
            usuarios.TotalCount,
            usuarios.PageSize,
            usuarios.CurrentPage,
            usuarios.TotalPages,
            usuarios.HasNext,
            usuarios.HasPrevious
	    };
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return usuarios.Select(u => new UsuarioViewModel {
            idUsuario = u.idUsuario,
            idRol = u.idRol,
            rol = u.rol.nombre,
            nombre = u.nombre,
            tipo_documento = u.tipo_documento,
            num_documento = u.num_documento,
            direccion = u.direccion,
            telefono = u.telefono,
            email = u.email,
            password_hash = u.password_hash,
            imgUrl = u.imgUrl,
            activo = u.activo
        });
    }

    // GET: api/Usuarios/Mostrar/1
    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Mostrar([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id proporcionado del usuario es inválido"
            });
        }
        
        var usuario = await _context.Usuarios.Include(u => u.rol).SingleOrDefaultAsync(u => u.idUsuario == id);
        
        if (usuario == null)
        {
            return NotFound(new {
                ok = false,
                message = "El usuario que intenta buscar no existe en el sistema"
            });
        }

        return Ok(new UsuarioViewModel {
            idUsuario = usuario.idUsuario,
            idRol = usuario.idRol,
            rol = usuario.rol.nombre,
            nombre = usuario.nombre,
            tipo_documento = usuario.tipo_documento,
            num_documento = usuario.num_documento,
            direccion = usuario.direccion,
            telefono = usuario.telefono,
            email = usuario.email,
            password_hash = usuario.password_hash,
            imgUrl = usuario.imgUrl,
            activo = usuario.activo
        });
    }

    // POST: api/Usuarios/Crear
    [HttpPost("[action]")]
    public async Task<IActionResult> Crear([FromBody] UsuariosCrearModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string email = model.email.ToLower();

        if (await _context.Usuarios.AnyAsync(u => u.email == email))
        {
            return BadRequest(new {
                ok = false,
                message = "El email ya existe"
            });
        }
        
        CrearPasswordHash(model.password, out byte[] passwordHash, out byte[] passwordSalt);
        
        Usuario usuario = new Usuario
        {
            idRol = model.idRol,
            nombre = model.nombre,
            direccion = model.direccion,
            tipo_documento = model.tipo_documento,
            num_documento = model.num_documento,
            telefono = model.telefono,
            email = model.email.ToLower(),
            password_hash = passwordHash,
            password_salt = passwordSalt,
            imgUrl = model.imgUrl,
            activo = true
        };
        
        _context.Usuarios.Add(usuario);

        try
        {
            await _context.SaveChangesAsync();
        } catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su artículo, inténtelo más tarde"
            });
        }
        
        List<UsuarioViewModel> usuariosModel = await ListaDeUsuarios();
        
        return Ok(new {
            ok = true,
            message = "El usuario se ha creado exitosamente!",
            usuarios = usuariosModel
        });
    }
    
    // PUT: api/Usuarios/Actualizar
    [HttpPut("[action]")]
    public async Task<IActionResult> Actualizar([FromBody] UsuariosActualizarModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (model.idUsuario <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id del usuario proporcionado es inválido"
            });
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == model.idUsuario);

        if (usuario == null)
        {
            return NotFound(new {
                ok = false,
                message = "El usuario que intenta actualizar no se encuentra en el sistema"
            });
        }

        usuario.idRol = model.idRol;
        usuario.nombre = model.nombre;
        usuario.tipo_documento = model.tipo_documento;
        usuario.num_documento = model.num_documento;
        usuario.direccion = model.direccion;
        usuario.telefono = model.telefono;
        usuario.email = model.email;

        if (model.act_password)
        {
            CrearPasswordHash(model.password, out byte[] passwordHash, out byte[] passwordSalt);
            usuario.password_hash = passwordHash;
        }

        try
        {
            await _context.SaveChangesAsync();
        } 
        catch(DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su artículo, inténtelo más tarde"
            });
        }

        List<UsuarioViewModel> usuariosModel = await ListaDeUsuarios();

        return Ok(new {
            ok = true,
            message = "El usuario se ha actualizado exitosamente!",
            usuarios = usuariosModel
        });
    }

    [HttpPost("[action]")]
    public ActionResult UploadProfilePic([FromForm] UsuariosImgModel file)
    {
        string fileName = String.Empty;
        try
        {
            string extension = "." + file.image.FileName.Split(".")[file.image.FileName.Split(".").Length - 1];
            fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/images", fileName);
            if (file.image.Length > 0)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.image.CopyTo(fileStream);
                }
            }
            return Ok(new {
                ok = true,
                message = "La imagen ha sido cargada exitosamente!",
                imgUrl =  "/images/" + fileName
            });
        }
        catch (Exception)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al subir la imagen al servidor, intente más tarde"
            });
        }
    }
    
    // PUT: api/Usuarios/Desactivar/1
    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Desactivar([FromRoute] int id)
    {
        if(id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id proporcionado es inválido"
            });
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == id);

        if(usuario == null)
        {
            return BadRequest(new {
                ok = false,
                message = "El usuario que intenta actualizar no se encuentra en el sistema"
            });
        }

        usuario.activo = false;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su artículo, inténtelo más tarde"
            });
        }

        return Ok(new {
            ok = true,
            message = "El usuario se ha desactivado exitosamente!"
        });
    }
    
    // PUT: api/Usuarios/Activar/1
    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Activar([FromRoute] int id)
    {
        if(id <= 0)
        {
            return BadRequest(new {
                ok = false,
                message = "El id proporcionado es inválido"
            });
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == id);

        if(usuario == null)
        {
            return BadRequest(new {
                ok = false,
                message = "El usuario que intenta actualizar no se encuentra en el sistema"
            });
        }

        usuario.activo = true;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            return BadRequest(new {
                ok = false,
                message = "Hubo un problema al crear su artículo, inténtelo más tarde"
            });
        }

        return Ok(new {
            ok = true,
            message = "El usuario se ha activado exitosamente!"
        });
    }
    
    public async Task<List<UsuarioViewModel>> ListaDeUsuarios () {
        // Se obtienen todas las categorias con la recien creada que se mostraran al usuarios
        var usuarios = await this.Listar(new PaginationParameters { PageNumber = 1, PageSize = 10 });
        
        List<UsuarioViewModel> usuariosModel = new List<UsuarioViewModel>();
        foreach (var item in usuarios)
        {
            usuariosModel.Add(new UsuarioViewModel {
                idRol = item.idRol,
                rol = item.rol,
                idUsuario = item.idUsuario,
                nombre = item.nombre,
                direccion = item.direccion,
                tipo_documento = item.tipo_documento,
                num_documento = item.num_documento,
                telefono = item.telefono,
                email = item.email.ToLower(),
                password_hash = item.password_hash,
                imgUrl = item.imgUrl,
                activo = item.activo
            });
        }
        return usuariosModel;
    }
    
    private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
