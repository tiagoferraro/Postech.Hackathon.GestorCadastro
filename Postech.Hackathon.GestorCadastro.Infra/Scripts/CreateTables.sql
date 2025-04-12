-- Criar tabela de Usuários
CREATE TABLE Usuario (
    UsuarioId UNIQUEIDENTIFIER PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    CPF NVARCHAR(11) NOT NULL UNIQUE,
    SenhaHash NVARCHAR(100) NOT NULL,
    TipoUsuario INT NOT NULL,
    DataCriacao DATETIME NOT NULL,
    UltimoLogin DATETIME NULL,
    IndAtivo BIT NOT NULL DEFAULT 1
);

-- Criar tabela de Médicos
CREATE TABLE Medicos (
    IdMedico UNIQUEIDENTIFIER PRIMARY KEY,
    IdUsuario UNIQUEIDENTIFIER NOT NULL,
    IdEspecialidade UNIQUEIDENTIFIER NOT NULL,
    CRM NVARCHAR(20) NOT NULL UNIQUE,
    ValorConsulta DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(UsuarioId)
); 