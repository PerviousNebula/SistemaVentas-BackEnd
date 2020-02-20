using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class Menu_RolMap : IEntityTypeConfiguration<Menu_Rol>
{
    public void Configure(EntityTypeBuilder<Menu_Rol> builder)
    {
        builder.ToTable("Menu_Rol").HasKey(mr => mr.idMenu_Rol);
    }
}