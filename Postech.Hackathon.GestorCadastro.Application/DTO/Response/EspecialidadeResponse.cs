namespace Postech.Hackathon.GestorCadastro.Application.DTO.Response;

public record EspecialidadeResponse(
    Guid IdEspecialidade,
    string Nome,
    string Descricao,
    DateTime DataCriacao
); 