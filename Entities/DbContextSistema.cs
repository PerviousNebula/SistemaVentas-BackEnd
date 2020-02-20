using Microsoft.EntityFrameworkCore;

public class DbContextSistema : DbContext
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Articulo> Articulos { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Persona> Personas { get; set; }    
    public DbSet<Menu> Menu { get; set; }
    public DbSet<Subcategory> Subcategorias { get; set; }
    public DbSet<Menu_Rol> Menu_Rol { get; set; }
    public DbContextSistema(DbContextOptions<DbContextSistema> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CategoriaMap());
        modelBuilder.ApplyConfiguration(new ArticuloMap());
        modelBuilder.ApplyConfiguration(new RolMap());
        modelBuilder.ApplyConfiguration(new UsuarioMap());
        modelBuilder.ApplyConfiguration(new PersonaMap());
        modelBuilder.ApplyConfiguration(new MenuMap());
        modelBuilder.ApplyConfiguration(new SubcategoryMap());
        modelBuilder.ApplyConfiguration(new Menu_RolMap());
    }
}