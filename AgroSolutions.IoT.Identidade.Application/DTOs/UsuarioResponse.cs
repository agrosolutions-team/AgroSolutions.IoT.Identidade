namespace AgroSolutions.IoT.Identidade.Application.DTOs;

public record UsuarioResponse(Guid Id, string Nome, string Email, DateTime DataCriacao);
