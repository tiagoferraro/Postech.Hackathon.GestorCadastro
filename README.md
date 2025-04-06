# Postech.Hackathon.Agenda

Sistema de gerenciamento de agenda desenvolvido durante o Hackathon da Postech.

## Estrutura do Projeto

O projeto está organizado nas seguintes camadas:

- **Api**: Camada de apresentação, responsável por expor os endpoints da aplicação
- **Application**: Camada de aplicação, contendo os casos de uso e regras de negócio
- **Domain**: Camada de domínio, contendo as entidades e regras de domínio
- **Infra**: Camada de infraestrutura, responsável pela persistência e serviços externos
- **Ioc**: Camada de injeção de dependência, responsável pela configuração do container IoC
- **Test**: Projeto de testes unitários
- **TestIntegration**: Projeto de testes de integração

## Tecnologias Utilizadas

- .NET 7.0
- ASP.NET Core
- Entity Framework Core
- xUnit
- Docker

## Como Executar

1. Clone o repositório
2. Execute `dotnet restore`
3. Execute `dotnet build`
4. Execute `dotnet run --project Postech.Hackathon.Agenda.Api`

## Como Executar os Testes

Execute `dotnet test` na raiz do projeto para executar todos os testes.