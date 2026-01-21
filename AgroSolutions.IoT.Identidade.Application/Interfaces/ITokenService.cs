using AgroSolutions.IoT.Identidade.Domain.Entities;

namespace AgroSolutions.IoT.Identidade.Application.Interfaces;

public interface ITokenService
{
    string GerarToken(Usuario usuario);
}
