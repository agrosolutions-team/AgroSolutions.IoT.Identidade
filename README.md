# AgroSolutions.IoT.Identidade

API de autenticação e gestão de usuários construída com .NET 8, DDD (Domain-Driven Design) e Clean Architecture.

## 🏗️ Arquitetura

A solução segue os princípios de Clean Architecture com separação clara de responsabilidades:

- **Domain**: Entidades, interfaces de repositórios e regras de negócio
- **Application**: Casos de uso, DTOs e interfaces de serviços
- **Infrastructure**: Implementações de repositórios, EF Core, segurança (JWT, BCrypt)
- **Api**: Controllers e configurações

## 📋 Pré-requisitos

- .NET 8.0 SDK ou superior
- PostgreSQL 12 ou superior
- Visual Studio 2022 / VS Code / Rider

## 🚀 Como executar

### 1. Configurar o banco de dados

Edite o arquivo `appsettings.json` no projeto Api com suas credenciais do PostgreSQL:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=AgroSolutionsIdentidade;Username=seu_usuario;Password=sua_senha"
}
```

### 2. Executar as migrations

```bash
cd AgroSolutions.IoT.Identidade.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../AgroSolutions.IoT.Identidade.Api
dotnet ef database update --startup-project ../AgroSolutions.IoT.Identidade.Api
```

### 3. Executar a aplicação

```bash
cd AgroSolutions.IoT.Identidade.Api
dotnet run
```

A API estará disponível em `https://localhost:5001` (ou a porta configurada).

## 📚 Endpoints

### Auth Controller

#### POST /api/auth/registrar
Registra um novo usuário.

**Request:**
```json
{
  "nome": "João Silva",
  "email": "joao@example.com",
  "senha": "senha123"
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "João Silva",
  "email": "joao@example.com",
  "dataCriacao": "2026-01-20T10:30:00Z"
}
```

#### POST /api/auth/login
Autentica um usuário e retorna um token JWT.

**Request:**
```json
{
  "email": "joao@example.com",
  "senha": "senha123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nome": "João Silva",
    "email": "joao@example.com",
    "dataCriacao": "2026-01-20T10:30:00Z"
  }
}
```

### Usuarios Controller

#### GET /api/usuarios
Lista todos os usuários (requer autenticação JWT).

**Headers:**
```
Authorization: Bearer {seu-token-jwt}
```

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "nome": "João Silva",
    "email": "joao@example.com",
    "dataCriacao": "2026-01-20T10:30:00Z"
  }
]
```

## 🔐 Autenticação

A API utiliza JWT (JSON Web Token) para autenticação. Após o login, inclua o token no header:

```
Authorization: Bearer {token}
```

## 🧪 Testando com Swagger

Acesse `https://localhost:5001/swagger` para testar os endpoints através da interface do Swagger.

Para testar endpoints protegidos:
1. Registre um usuário em `/api/auth/registrar`
2. Faça login em `/api/auth/login` e copie o token
3. Clique em "Authorize" no Swagger e cole o token
4. Agora você pode acessar `/api/usuarios`

## 🛠️ Tecnologias utilizadas

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 8.0
- PostgreSQL
- BCrypt.Net (hashing de senhas)
- JWT Bearer Authentication
- Swagger/OpenAPI

## 📁 Estrutura do projeto

```
AgroSolutions.IoT.Identidade/
├── AgroSolutions.IoT.Identidade.Domain/
│   ├── Entities/
│   │   └── Usuario.cs
│   ├── Repositories/
│   │   └── IUsuarioRepository.cs
│   └── Exceptions/
│       └── DomainException.cs
├── AgroSolutions.IoT.Identidade.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── Exceptions/
├── AgroSolutions.IoT.Identidade.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Configurations/
│   ├── Repositories/
│   └── Security/
└── AgroSolutions.IoT.Identidade.Api/
    ├── Controllers/
    ├── Program.cs
    └── appsettings.json
```

## ✅ Boas práticas implementadas

- ✅ Domain-Driven Design (DDD)
- ✅ Clean Architecture
- ✅ SOLID Principles
- ✅ Entidades ricas com validações
- ✅ Separação de responsabilidades por camadas
- ✅ Injeção de dependências
- ✅ DTOs para comunicação entre camadas
- ✅ Hashing de senhas com BCrypt
- ✅ Autenticação JWT
- ✅ Documentação com Swagger
- ✅ UUIDs como chave primária
