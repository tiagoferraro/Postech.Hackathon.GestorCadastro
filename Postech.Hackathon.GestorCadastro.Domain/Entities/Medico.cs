using Postech.Hackathon.GestorCadastro.Domain.Exceptions;

namespace Postech.Hackathon.GestorCadastro.Domain.Entities;

public class Medico
{
    public Guid MedicoId { get; private set; } = Guid.NewGuid();
    public string CRM { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid EspecialidadeId { get; private set; }

    // Construtor sem parâmetros para o Dapper
    private Medico()
    {
        CRM = string.Empty;
    }

    public Medico(string crm, Guid idUsuario, Guid idEspecialidade)
    {
        CRM = crm;
        UsuarioId = idUsuario;
        EspecialidadeId = idEspecialidade;

        Validar();
    }

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(CRM))
            throw new DomainException("O CRM do médico não pode ser vazio.");

        if (UsuarioId == Guid.Empty)
            throw new DomainException("O ID do usuário não pode ser vazio.");

        if (EspecialidadeId == Guid.Empty)
            throw new DomainException("O ID da especialidade não pode ser vazio.");
    }

    public void AtualizarDados(string crm, Guid idEspecialidade)
    {
        CRM = crm;
        EspecialidadeId = idEspecialidade;

        Validar();
    }
} 