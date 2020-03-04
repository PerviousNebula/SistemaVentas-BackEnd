using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VentaMap : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Venta").HasKey(v => v.idVenta);
        builder.HasOne(v => v.persona)
               .WithMany(p => p.ventas)
               .HasForeignKey(v => v.idPersona);
    }
}