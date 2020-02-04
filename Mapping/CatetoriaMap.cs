using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoriaMap : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categoria").HasKey(c => c.idCategoria);
        builder.Property(c => c.nombre).HasMaxLength(50);
        builder.Property(c => c.descripcion).HasMaxLength(256);
    }
}