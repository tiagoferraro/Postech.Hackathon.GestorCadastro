using Postech.Hackathon.GestorCadastro.Domain.Exceptions;

namespace Postech.Hackathon.GestorCadastro.Domain.Entities;

public class Medico
{
    public Guid IdMedico { get; private set; } = Guid.NewGuid();
    public string CRM { get; private set; }
    public Guid IdUsuario { get; private set; }
    public Guid IdEspecialidade { get; private set; }

    // Construtor sem parâmetros para o Dapper
    private Medico()
    {
        CRM = string.Empty;
    }

    public Medico(string crm, Guid idUsuario, Guid idEspecialidade)
    {
        CRM = crm;
        IdUsuario = idUsuario;
        IdEspecialidade = idEspecialidade;

        Validar();
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(CRM))
            throw new DomainException("O CRM do médico não pode ser vazio.");

        if (IdUsuario == Guid.Empty)
            throw new DomainException("O ID do usuário não pode ser vazio.");

        if (IdEspecialidade == Guid.Empty)
            throw new DomainException("O ID da especialidade não pode ser vazio.");
    }

    public void AtualizarDados(string crm, Guid idEspecialidade)
    {
        CRM = crm;
        IdEspecialidade = idEspecialidade;

        Validar();
    }
} 