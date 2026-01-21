using AgroSolutions.IoT.Identidade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AgroSolutions.IoT.Identidade.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed do usuário administrador padrão
        var adminId = Guid.NewGuid();
        var senhaHashAdmin = BCrypt.Net.BCrypt.HashPassword("12345");

        // Criando o usuário através do método factory
        var admin = Usuario.Criar("admin", "admin@agro.com", senhaHashAdmin);
        
        // Usando reflexão para definir o Id e DataCriacao (pois são private set)
        typeof(Usuario).GetProperty(nameof(Usuario.Id))!
            .SetValue(admin, adminId);
        
        typeof(Usuario).GetProperty(nameof(Usuario.DataCriacao))!
            .SetValue(admin, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        modelBuilder.Entity<Usuario>().HasData(admin);
    }
}
