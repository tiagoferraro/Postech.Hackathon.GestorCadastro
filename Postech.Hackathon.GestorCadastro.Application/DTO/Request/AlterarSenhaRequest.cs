namespace Postech.Hackathon.GestorCadastro.Application.DTO.Request;

public record AlterarSenhaRequest(
    string SenhaAtual,
    string NovaSenha
); 