using AgroSolutions.IoT.Identidade.Application.DTOs;
using AgroSolutions.IoT.Identidade.Application.Interfaces;
using AgroSolutions.IoT.Identidade.Application.Services;
using AgroSolutions.IoT.Identidade.Domain.Entities;
using AgroSolutions.IoT.Identidade.Domain.Repositories;
using FluentAssertions;
using Moq;
using ApplicationException = AgroSolutions.IoT.Identidade.Application.Exceptions.ApplicationException;

namespace AgroSolutions.IoT.Identidade.Tests.Application.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly UsuarioService _sut;

    public UsuarioServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _sut = new UsuarioService(_usuarioRepositoryMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task RegistrarUsuarioAsync_DeveRetornarUsuarioResponse_QuandoDadosSaoValidos()
    {
        // Arrange
        var request = new RegistrarUsuarioRequest("João Silva", "joao@test.com", "SenhaSegura@123");
        var senhaHash = "$2a$11$hashedpassword";

        _usuarioRepositoryMock.Setup(x => x.ExistePorEmailAsync(request.Email))
            .ReturnsAsync(false);

        _passwordHasherMock.Setup(x => x.HashPassword(request.Senha))
            .Returns(senhaHash);

        _usuarioRepositoryMock.Setup(x => x.AdicionarAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _sut.RegistrarUsuarioAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be(request.Nome);
        resultado.Email.Should().Be(request.Email);
        resultado.Id.Should().NotBeEmpty();

        _passwordHasherMock.Verify(x => x.HashPassword(request.Senha), Times.Once);
        _usuarioRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
    }

    [Theory]
    [InlineData("", "email@test.com", "senha")]
    [InlineData(null, "email@test.com", "senha")]
    public async Task RegistrarUsuarioAsync_DeveLancarApplicationException_QuandoNomeEhInvalido(string nomeInvalido, string email, string senha)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest(nomeInvalido, email, senha);

        // Act
        var act = async () => await _sut.RegistrarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Nome é obrigatório");
    }

    [Theory]
    [InlineData("João Silva", "", "senha")]
    [InlineData("João Silva", null, "senha")]
    public async Task RegistrarUsuarioAsync_DeveLancarApplicationException_QuandoEmailEhInvalido(string nome, string emailInvalido, string senha)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest(nome, emailInvalido, senha);

        // Act
        var act = async () => await _sut.RegistrarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Email é obrigatório");
    }

    [Theory]
    [InlineData("João Silva", "email@test.com", "")]
    [InlineData("João Silva", "email@test.com", null)]
    public async Task RegistrarUsuarioAsync_DeveLancarApplicationException_QuandoSenhaEhInvalida(string nome, string email, string senhaInvalida)
    {
        // Arrange
        var request = new RegistrarUsuarioRequest(nome, email, senhaInvalida);

        // Act
        var act = async () => await _sut.RegistrarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Senha é obrigatória");
    }

    [Fact]
    public async Task RegistrarUsuarioAsync_DeveLancarApplicationException_QuandoEmailJaExiste()
    {
        // Arrange
        var request = new RegistrarUsuarioRequest("João Silva", "joao@test.com", "senha");

        _usuarioRepositoryMock.Setup(x => x.ExistePorEmailAsync(request.Email))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.RegistrarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Email já está em uso");
    }

    [Fact]
    public async Task AutenticarUsuarioAsync_DeveRetornarLoginResponse_QuandoCredenciaisSaoValidas()
    {
        // Arrange
        var request = new LoginRequest("joao@test.com", "SenhaSegura@123");
        var usuario = Usuario.Criar("João Silva", request.Email, "$2a$11$hashedpassword");
        var token = "jwt.token.here";

        _usuarioRepositoryMock.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        _passwordHasherMock.Setup(x => x.VerifyPassword(request.Senha, usuario.SenhaHash))
            .Returns(true);

        _tokenServiceMock.Setup(x => x.GerarToken(usuario))
            .Returns(token);

        // Act
        var resultado = await _sut.AutenticarUsuarioAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().Be(token);
        resultado.Usuario.Should().NotBeNull();
        resultado.Usuario.Email.Should().Be(usuario.Email);
        resultado.Usuario.Nome.Should().Be(usuario.Nome);

        _passwordHasherMock.Verify(x => x.VerifyPassword(request.Senha, usuario.SenhaHash), Times.Once);
        _tokenServiceMock.Verify(x => x.GerarToken(usuario), Times.Once);
    }

    [Theory]
    [InlineData("", "senha")]
    [InlineData(null, "senha")]
    public async Task AutenticarUsuarioAsync_DeveLancarApplicationException_QuandoEmailEhInvalido(string emailInvalido, string senha)
    {
        // Arrange
        var request = new LoginRequest(emailInvalido, senha);

        // Act
        var act = async () => await _sut.AutenticarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Email é obrigatório");
    }

    [Theory]
    [InlineData("email@test.com", "")]
    [InlineData("email@test.com", null)]
    public async Task AutenticarUsuarioAsync_DeveLancarApplicationException_QuandoSenhaEhInvalida(string email, string senhaInvalida)
    {
        // Arrange
        var request = new LoginRequest(email, senhaInvalida);

        // Act
        var act = async () => await _sut.AutenticarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Senha é obrigatória");
    }

    [Fact]
    public async Task AutenticarUsuarioAsync_DeveLancarApplicationException_QuandoUsuarioNaoExiste()
    {
        // Arrange
        var request = new LoginRequest("joao@test.com", "senha");

        _usuarioRepositoryMock.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        // Act
        var act = async () => await _sut.AutenticarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Credenciais inválidas");
    }

    [Fact]
    public async Task AutenticarUsuarioAsync_DeveLancarApplicationException_QuandoSenhaEstahIncorreta()
    {
        // Arrange
        var request = new LoginRequest("joao@test.com", "senhaErrada");
        var usuario = Usuario.Criar("João Silva", request.Email, "$2a$11$hashedpassword");

        _usuarioRepositoryMock.Setup(x => x.ObterPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        _passwordHasherMock.Setup(x => x.VerifyPassword(request.Senha, usuario.SenhaHash))
            .Returns(false);

        // Act
        var act = async () => await _sut.AutenticarUsuarioAsync(request);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Credenciais inválidas");
    }

    [Fact]
    public async Task ListarUsuariosAsync_DeveRetornarListaDeUsuarios_QuandoExistemUsuarios()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            Usuario.Criar("João Silva", "joao@test.com", "hash1"),
            Usuario.Criar("Maria Santos", "maria@test.com", "hash2"),
            Usuario.Criar("Pedro Oliveira", "pedro@test.com", "hash3")
        };

        _usuarioRepositoryMock.Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(usuarios);

        // Act
        var resultado = await _sut.ListarUsuariosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(3);
        resultado.Should().Contain(u => u.Nome == "João Silva");
        resultado.Should().Contain(u => u.Nome == "Maria Santos");
        resultado.Should().Contain(u => u.Nome == "Pedro Oliveira");
    }

    [Fact]
    public async Task ListarUsuariosAsync_DeveRetornarListaVazia_QuandoNaoExistemUsuarios()
    {
        // Arrange
        _usuarioRepositoryMock.Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(new List<Usuario>());

        // Act
        var resultado = await _sut.ListarUsuariosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}
