namespace Postech.Hackathon.GestorCadastro.Application.DTO.Request;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
