using AgroSolutions.IoT.Identidade.Domain.Entities;

namespace AgroSolutions.IoT.Identidade.Domain.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(Guid id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ObterTodosAsync();
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task<bool> ExistePorEmailAsync(string email);
}
