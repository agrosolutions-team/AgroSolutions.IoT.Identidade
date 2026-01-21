using AgroSolutions.IoT.Identidade.Domain.Exceptions;

namespace AgroSolutions.IoT.Identidade.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public DateTime DataCriacao { get; private set; }

    private Usuario() { }

    private Usuario(string nome, string email, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        DataCriacao = DateTime.UtcNow;
        
        Validar();
    }

    public static Usuario Criar(string nome, string email, string senhaHash)
    {
        return new Usuario(nome, email, senhaHash);
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome não pode ser vazio");

        Nome = nome;
    }

    public void AtualizarSenha(string novaSenhaHash)
    {
        if (string.IsNullOrWhiteSpace(novaSenhaHash))
            throw new DomainException("Senha não pode ser vazia");

        SenhaHash = novaSenhaHash;
    }

    private void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new DomainException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(Email))
            throw new DomainException("Email é obrigatório");

        if (!Email.Contains("@"))
            throw new DomainException("Email inválido");

        if (string.IsNullOrWhiteSpace(SenhaHash))
            throw new DomainException("Senha é obrigatória");
    }
}
