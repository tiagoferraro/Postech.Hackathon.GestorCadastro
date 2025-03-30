using System;

namespace Postech.Hackathon.GestorCadastro.Domain.Exceptions;

public class DomainException(string message) : Exception(message);

