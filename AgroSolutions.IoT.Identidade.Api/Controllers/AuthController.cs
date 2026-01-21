using AgroSolutions.IoT.Identidade.Application.DTOs;
using AgroSolutions.IoT.Identidade.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.IoT.Identidade.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioRequest request)
    {
        try
        {
            var response = await _usuarioService.RegistrarUsuarioAsync(request);
            return CreatedAtAction(nameof(Registrar), new { id = response.Id }, response);
        }
        catch (Application.Exceptions.ApplicationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Autentica um usuário e retorna um token JWT
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _usuarioService.AutenticarUsuarioAsync(request);
            return Ok(response);
        }
        catch (Application.Exceptions.ApplicationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
