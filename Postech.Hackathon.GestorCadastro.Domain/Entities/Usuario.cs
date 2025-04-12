using Postech.Hackathon.GestorCadastro.Domain.Enum;
using Postech.Hackathon.GestorCadastro.Domain.Exceptions;

namespace Postech.Hackathon.GestorCadastro.Domain.Entities;

public class Usuario
{
    public Guid UsuarioId { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public string CPF { get; private set; }
    public ETipoUsuario TipoUsuario { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? UltimoLogin { get; private set; }
    public bool IndAtivo { get; private set; }

    // Construtor sem parâmetros para o Dapper
    private Usuario() 
    { 
        Nome = string.Empty;
        Email = string.Empty;
        SenhaHash = string.Empty;
        CPF = string.Empty;
        DataCriacao = DateTime.UtcNow;
        IndAtivo = true;
    }

    public Usuario(string nome, string email, string senha, string cpf, ETipoUsuario tipoUsuario)
    {
        Nome = nome;
        Email = email;
        SenhaHash = HashPassword(senha);
        CPF = cpf;
        TipoUsuario = tipoUsuario;
        DataCriacao = DateTime.UtcNow;
        IndAtivo = true;

        Validar();
    }
   
    public void Logar()
    {
        UltimoLogin = DateTime.UtcNow;
    }

    private static string HashPassword(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new DomainException("A senha do usuário não pode ser vazia.");

        return BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);
    }
    
    public bool ValidaSenha(string senha)
    {        
        return BCrypt.Net.BCrypt.Verify(senha, SenhaHash);
    }

    public void AlterarSenha(string novaSenha)
    {
        if (string.IsNullOrWhiteSpace(novaSenha))
            throw new DomainException("A nova senha não pode ser vazia.");

        SenhaHash = HashPassword(novaSenha);
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new DomainException("O nome do usuário não pode ser vazio.");
            
        if (string.IsNullOrWhiteSpace(Email))
            throw new DomainException("O email do usuário não pode ser vazio.");
            
        if (!Email.Contains('@') || !Email.Contains('.'))
            throw new DomainException("O email informado não é válido.");

        if (string.IsNullOrWhiteSpace(CPF))
            throw new DomainException("O CPF do usuário não pode ser vazio.");

        if (CPF.Length != 11)
            throw new DomainException("O CPF deve conter exatamente 11 caracteres.");

        if (!CPF.All(char.IsDigit))
            throw new DomainException("O CPF deve conter apenas números.");
    }

    public void AtualizarDados(string nome, string email, string senha, string cpf, ETipoUsuario tipoUsuario)
    {
        Nome = nome;
        Email = email;
        SenhaHash = HashPassword(senha);
        CPF = cpf;
        TipoUsuario = tipoUsuario;

        Validar();
    }
}
