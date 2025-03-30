# Postech.Hackathon.GestorCadastro

## Sobre o Projeto

Este projeto implementa um serviço de autorização utilizando JWT (JSON Web Tokens) para autenticação e autorização de usuários.

## Tecnologias Utilizadas

- .NET 9.0
- ASP.NET Core
- JWT (JSON Web Tokens)
- Arquitetura em Camadas (Clean Architecture)

## Funcionalidades

- Registro de usuários
- Login de usuários
- Validação de tokens JWT
- Obtenção de informações do usuário autenticado

## Configuração

### Configurações do JWT

As configurações do JWT estão no arquivo `appsettings.json`:

```json
"JwtSettings": {
  "SecretKey": "sua_chave_secreta_muito_segura_com_pelo_menos_32_caracteres",
  "Issuer": "Postech.Hackathon.GestorCadastro",
  "Audience": "Postech.Hackathon.Clients",
  "ExpirationInMinutes": 60,
  "RefreshTokenExpirationInDays": 7
}
```

**Importante**: Para ambientes de produção, substitua a chave secreta por uma chave forte e segura, e considere armazená-la em variáveis de ambiente ou no Azure Key Vault.

## Endpoints da API

### Autenticação

- **POST /api/auth/register**: Registra um novo usuário
  - Corpo da requisição:
    ```json
    {
      "username": "usuario",
      "email": "usuario@exemplo.com",
      "password": "senha123",
      "confirmPassword": "senha123"
    }
    ```

- **POST /api/auth/login**: Autentica um usuário
  - Corpo da requisição:
    ```json
    {
      "username": "usuario",
      "password": "senha123"
    }
    ```

- **GET /api/auth/validate?token={token}**: Valida um token JWT

- **GET /api/auth/user**: Obtém informações do usuário autenticado (requer autenticação)

## Como Usar

### Autenticação

1. Registre um usuário usando o endpoint `/api/auth/register`
2. Faça login usando o endpoint `/api/auth/login`
3. Use o token JWT retornado no cabeçalho `Authorization` das requisições subsequentes:
   ```
   Authorization: Bearer {seu_token_jwt}
   ```

## Segurança

- Os tokens JWT têm validade de 60 minutos por padrão
- As senhas são armazenadas como hash com salt para maior segurança
- Tokens de atualização (refresh tokens) permitem renovar a sessão sem precisar fazer login novamente