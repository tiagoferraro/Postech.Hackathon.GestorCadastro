using Postech.Hackathon.GestorCadastro.Domain.Exceptions;

namespace Postech.Hackathon.GestorCadastro.Domain.Entities;

public class Especialidade
{
    public Guid IdEspecialidade { get; private set; } = Guid.NewGuid();
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public bool IndAtivo { get; private set; }

    private Especialidade()
    {
        Nome = string.Empty;
        Descricao = string.Empty;
        DataCriacao = DateTime.UtcNow;
        IndAtivo = true;
    }

    public Especialidade(string nome, string descricao)
    {
        Nome = nome;
        Descricao = descricao;
        DataCriacao = DateTime.UtcNow;
        IndAtivo = true;

        Validar();
    }

    private void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new DomainException("O nome da especialidade não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(Descricao))
            throw new DomainException("A descrição da especialidade não pode ser vazia.");
    }
} 