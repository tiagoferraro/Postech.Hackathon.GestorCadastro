namespace Postech.Hackathon.GestorCadastro.Application.DTO.Response
{
    public record LoginResponse(
        string Token ,
        string RefreshToken ,
        DateTime Expiration,
        string Username,
        string Role
    );
}
