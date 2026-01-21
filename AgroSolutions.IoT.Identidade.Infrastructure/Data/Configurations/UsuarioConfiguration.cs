using AgroSolutions.IoT.Identidade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroSolutions.IoT.Identidade.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(u => u.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.SenhaHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.DataCriacao)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
