using AgroSolutions.IoT.Identidade.Domain.Entities;
using AgroSolutions.IoT.Identidade.Domain.Exceptions;
using FluentAssertions;

namespace AgroSolutions.IoT.Identidade.Tests.Domain.Entities;

public class UsuarioTests
{
    [Fact]
    public void Criar_DeveRetornarUsuarioValido_QuandoDadosEstaoCorretos()
    {
        // Arrange
        var nome = "João Silva";
        var email = "joao@example.com";
        var senhaHash = "$2a$11$hashedpassword";

        // Act
        var usuario = Usuario.Criar(nome, email, senhaHash);

        // Assert
        usuario.Should().NotBeNull();
        usuario.Id.Should().NotBeEmpty();
        usuario.Nome.Should().Be(nome);
        usuario.Email.Should().Be(email);
        usuario.SenhaHash.Should().Be(senhaHash);
        usuario.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "email@test.com", "hash")]
    [InlineData(null, "email@test.com", "hash")]
    [InlineData("   ", "email@test.com", "hash")]
    public void Criar_DeveLancarDomainException_QuandoNomeEhInvalido(string nomeInvalido, string email, string senhaHash)
    {
        // Act
        var act = () => Usuario.Criar(nomeInvalido, email, senhaHash);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome é obrigatório");
    }

    [Theory]
    [InlineData("João Silva", "", "hash")]
    [InlineData("João Silva", null, "hash")]
    [InlineData("João Silva", "   ", "hash")]
    public void Criar_DeveLancarDomainException_QuandoEmailEhVazio(string nome, string emailInvalido, string senhaHash)
    {
        // Act
        var act = () => Usuario.Criar(nome, emailInvalido, senhaHash);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email é obrigatório");
    }

    [Fact]
    public void Criar_DeveLancarDomainException_QuandoEmailNaoContemArroba()
    {
        // Arrange
        var nome = "João Silva";
        var emailInvalido = "emailinvalido.com";
        var senhaHash = "hash";

        // Act
        var act = () => Usuario.Criar(nome, emailInvalido, senhaHash);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Email inválido");
    }

    [Theory]
    [InlineData("João Silva", "email@test.com", "")]
    [InlineData("João Silva", "email@test.com", null)]
    [InlineData("João Silva", "email@test.com", "   ")]
    public void Criar_DeveLancarDomainException_QuandoSenhaHashEhInvalido(string nome, string email, string senhaHashInvalida)
    {
        // Act
        var act = () => Usuario.Criar(nome, email, senhaHashInvalida);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Senha é obrigatória");
    }

    [Fact]
    public void AtualizarNome_DeveAtualizarNome_QuandoNomeEhValido()
    {
        // Arrange
        var usuario = Usuario.Criar("João Silva", "joao@test.com", "hash");
        var novoNome = "João Pedro Silva";

        // Act
        usuario.AtualizarNome(novoNome);

        // Assert
        usuario.Nome.Should().Be(novoNome);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void AtualizarNome_DeveLancarDomainException_QuandoNomeEhInvalido(string nomeInvalido)
    {
        // Arrange
        var usuario = Usuario.Criar("João Silva", "joao@test.com", "hash");

        // Act
        var act = () => usuario.AtualizarNome(nomeInvalido);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Nome não pode ser vazio");
    }

    [Fact]
    public void AtualizarSenha_DeveAtualizarSenhaHash_QuandoSenhaHashEhValido()
    {
        // Arrange
        var usuario = Usuario.Criar("João Silva", "joao@test.com", "hash");
        var novaSenhaHash = "$2a$11$newhashedpassword";

        // Act
        usuario.AtualizarSenha(novaSenhaHash);

        // Assert
        usuario.SenhaHash.Should().Be(novaSenhaHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void AtualizarSenha_DeveLancarDomainException_QuandoSenhaHashEhInvalido(string senhaHashInvalida)
    {
        // Arrange
        var usuario = Usuario.Criar("João Silva", "joao@test.com", "hash");

        // Act
        var act = () => usuario.AtualizarSenha(senhaHashInvalida);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Senha não pode ser vazia");
    }
}
