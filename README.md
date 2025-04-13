# Postech.Hackathon.GestorCadastro

Sistema de gerenciamento de cadastro desenvolvido durante o Hackathon da Postech.

## Descrição

O Gestor de Cadastro é uma aplicação desenvolvida para gerenciar cadastros de forma eficiente e segura, seguindo as melhores práticas de desenvolvimento e arquitetura de software.

## Estrutura do Projeto

O projeto está organizado nas seguintes camadas:

- **Api**: Camada de apresentação, responsável por expor os endpoints da aplicação
- **Application**: Camada de aplicação, contendo os casos de uso e regras de negócio
- **Domain**: Camada de domínio, contendo as entidades e regras de domínio
- **Infra**: Camada de infraestrutura, responsável pela persistência e serviços externos
- **Ioc**: Camada de injeção de dependência, responsável pela configuração do container IoC
- **Test**: Projeto de testes unitários
- **TestIntegration**: Projeto de testes de integração

## Estrutura da API

A API segue o padrão RESTful e está organizada da seguinte forma:

### Endpoints Principais

#### Autenticação
- `POST /api/Autenticador/login` - Login com email e senha
- `POST /api/Autenticador/login/cpf` - Login com CPF
- `POST /api/Autenticador/login/crm` - Login com CRM
- `GET /api/Autenticador/ValidarToken` - Validação de token
- `GET /api/Autenticador/DadosUsuarioLogado` - Obter dados do usuário logado
- `POST /api/Autenticador/alterar-senha` - Alterar senha do usuário

#### Cadastro de Pessoas
- `POST /api/Pessoa/Cadastrar` - Cadastrar nova pessoa
- `PUT /api/Pessoa/Alterar` - Alterar dados de uma pessoa

#### Especialidades
- `GET /api/Especialidade` - Lista todas as especialidades
- `GET /api/Especialidade/{id}` - Obtém detalhes de uma especialidade específica
- `POST /api/Especialidade` - Cria uma nova especialidade

#### Médicos
- `GET /api/Medico/PorEspecialidade/{especialidadeId}` - Lista médicos por especialidade

