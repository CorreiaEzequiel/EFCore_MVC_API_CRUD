# EFCore_MVC_API_CRUD
## Descrição
Este projeto é uma aplicação ASP.NET Core que combina uma API RESTful com uma interface MVC. Ele permite gerenciar produtos, clientes e pedidos usando operações CRUD.

## Tecnologias Utilizadas
- ASP.NET Core
- Entity Framework Core
- SQL Server
- MVC

## Configuração
1. Clone o repositório.
2. Configure a string de conexão no `appsettings.json`.
3. Execute as migrações com `Update-Database` para criar o banco de dados.
4. Inicie a aplicação.

## Uso
- **GET /api/produtos**: Retorna todos os produtos.
- **POST /api/clientes**: Adiciona um novo cliente.
  
## Estrutura do Projeto
- **MyAppAPI**: Projeto da API com lógica de negócio e persistência de dados.
- **MyAppMVC**: Projeto MVC que consome a API e exibe os dados para o usuário.
