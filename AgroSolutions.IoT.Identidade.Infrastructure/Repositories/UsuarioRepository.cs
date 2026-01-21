using AgroSolutions.IoT.Identidade.Domain.Entities;
using AgroSolutions.IoT.Identidade.Domain.Repositories;
using AgroSolutions.IoT.Identidade.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.IoT.Identidade.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<Usuario>> ObterTodosAsync()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistePorEmailAsync(string email)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email);
    }
}
