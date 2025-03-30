namespace Postech.Hackathon.GestorCadastro.Api.Models;

public record ApiErrorResponse(int StatusCode, string Message, string? Details);
