using Postech.Hackathon.GestorCadastro.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Postech.Hackathon.GestorCadastro.Application.DTO.Request;

public class AlterarRequest
{
    [Required]
    public required Guid Id { get; set; }
    [Required]
    public required string Nome { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [MinLength(6)]
    public required string Senha { get; set; }
    [Required]
    [StringLength(11, MinimumLength = 11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos numéricos")]
    public required string CPF { get; set; }
    [Required]
    public required ETipoUsuario TipoUsuario { get; set; }
    public MedicoRequest? Medico { get; set; }
} 