CREATE TABLE Especialidade (
    IdEspecialidade UNIQUEIDENTIFIER PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(500) NOT NULL,
    DataCriacao DATETIME NOT NULL,
    IndAtivo BIT NOT NULL DEFAULT 1
);

-- Inserir algumas especialidades iniciais
INSERT INTO Especialidade (IdEspecialidade, Nome, Descricao, DataCriacao, IndAtivo)
VALUES 
    (NEWID(), 'Cardiologia', 'Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem o coração', GETDATE(), 1),
    (NEWID(), 'Dermatologia', 'Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem a pele', GETDATE(), 1),
    (NEWID(), 'Oftalmologia', 'Especialidade médica que se ocupa do diagnóstico e tratamento das doenças que acometem os olhos', GETDATE(), 1),
    (NEWID(), 'Pediatria', 'Especialidade médica dedicada à assistência à criança e ao adolescente', GETDATE(), 1),
    (NEWID(), 'Psiquiatria', 'Especialidade médica que se ocupa do diagnóstico e tratamento das doenças mentais', GETDATE(), 1); 