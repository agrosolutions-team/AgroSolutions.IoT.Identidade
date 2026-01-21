using AgroSolutions.IoT.Identidade.Application.DTOs;
using AgroSolutions.IoT.Identidade.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.IoT.Identidade.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Lista todos os usuários (requer autenticação)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _usuarioService.ListarUsuariosAsync();
        return Ok(usuarios);
    }
}
