using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DetalleIngresoMap : IEntityTypeConfiguration<DetalleIngreso>
{
    public void Configure(EntityTypeBuilder<DetalleIngreso> builder)
    {
        builder.ToTable("DetalleIngreso").HasKey(di => di.idDetalleIngreso);
    }
}