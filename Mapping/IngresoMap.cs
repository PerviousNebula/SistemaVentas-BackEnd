using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class IngresoMap : IEntityTypeConfiguration<Ingreso>
{
    public void Configure(EntityTypeBuilder<Ingreso> builder)
    {
        builder.ToTable("Ingreso").HasKey(i => i.idIngreso);
        builder.HasOne(i => i.proveedor)
               .WithMany(p => p.ingresos)
               .HasForeignKey(i => i.idProveedor);
    }
}