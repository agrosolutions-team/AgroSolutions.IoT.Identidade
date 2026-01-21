using AgroSolutions.IoT.Identidade.Application.DTOs;
using AgroSolutions.IoT.Identidade.Application.Interfaces;
using AgroSolutions.IoT.Identidade.Domain.Entities;
using AgroSolutions.IoT.Identidade.Domain.Repositories;

namespace AgroSolutions.IoT.Identidade.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<UsuarioResponse> RegistrarUsuarioAsync(RegistrarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new Exceptions.ApplicationException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new Exceptions.ApplicationException("Email é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Senha))
            throw new Exceptions.ApplicationException("Senha é obrigatória");

        if (await _usuarioRepository.ExistePorEmailAsync(request.Email))
            throw new Exceptions.ApplicationException("Email já está em uso");

        var senhaHash = _passwordHasher.HashPassword(request.Senha);
        var usuario = Usuario.Criar(request.Nome, request.Email, senhaHash);

        await _usuarioRepository.AdicionarAsync(usuario);

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.DataCriacao);
    }

    public async Task<LoginResponse> AutenticarUsuarioAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new Exceptions.ApplicationException("Email é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Senha))
            throw new Exceptions.ApplicationException("Senha é obrigatória");

        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        
        if (usuario == null)
            throw new Exceptions.ApplicationException("Credenciais inválidas");

        if (!_passwordHasher.VerifyPassword(request.Senha, usuario.SenhaHash))
            throw new Exceptions.ApplicationException("Credenciais inválidas");

        var token = _tokenService.GerarToken(usuario);
        var usuarioResponse = new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.DataCriacao);

        return new LoginResponse(token, usuarioResponse);
    }

    public async Task<IEnumerable<UsuarioResponse>> ListarUsuariosAsync()
    {
        var usuarios = await _usuarioRepository.ObterTodosAsync();
        
        return usuarios.Select(u => new UsuarioResponse(u.Id, u.Nome, u.Email, u.DataCriacao));
    }
}
