using AgroSolutions.IoT.Identidade.Application.DTOs;

namespace AgroSolutions.IoT.Identidade.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioResponse> RegistrarUsuarioAsync(RegistrarUsuarioRequest request);
    Task<LoginResponse> AutenticarUsuarioAsync(LoginRequest request);
    Task<IEnumerable<UsuarioResponse>> ListarUsuariosAsync();
}
