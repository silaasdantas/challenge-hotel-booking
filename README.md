# Hotel Booking API

API para gerenciar reservas de hotel para um quarto. O projeto expõe endpoints para listar quartos, consultar disponibilidade, criar reservas, atualizar datas, cancelar reservas e realizar checkout.

A API usa uma base em memória para facilitar execução local e validação de regras de negócio. O contrato HTTP detalhado para clientes frontend está em [`docs/api-contract.md`](docs/api-contract.md).

## Funcionalidades

- Listar quartos ativos.
- Buscar quarto por id.
- Listar reservas.
- Buscar reserva por id.
- Criar reserva.
- Atualizar datas de uma reserva.
- Cancelar reserva.
- Realizar checkout de uma reserva.
- Checar disponibilidade de um quarto para um período.

## Regras de negócio principais

- A reserva deve começar no mínimo no dia seguinte ao dia atual.
- A estadia não pode ser maior que 3 dias.
- A reserva não pode iniciar mais de 30 dias no futuro.
- Conflitos de agenda retornam `409 Conflict`.
- Apenas reservas com status `ActiveBooking` bloqueiam disponibilidade.
- O checkout altera o status da reserva para `CheckedOut`, liberando o quarto para novas reservas conforme a regra operacional de disponibilidade.

Status de reserva usados pela API:

- `ActiveBooking`
- `BookingCanceled`
- `CheckedOut`

## Stack

- .NET 10
- ASP.NET Core
- Entity Framework Core InMemory
- Swagger / Swashbuckle
- API Versioning
- Rate limiting em endpoints sensíveis
- xUnit
- Moq
- AutoFixture
- Shouldly
- coverlet

## Estrutura do projeto

```text
src/
  Hotel.Booking.Api/          # Controllers, configuração HTTP, Swagger, CORS, rate limiting e health check
  Hotel.Booking.Core/         # DTOs, entidades, interfaces, serviços, validadores e regras de domínio
  Hotel.Booking.Infra/        # DbContext EF InMemory, seeds e repositórios
tests/
  Hotel.Booking.UnitTest/     # Testes unitários dos serviços e regras principais
  Hotel.Booking.IntegrationTest/ # Projeto de testes de integração
docs/
  api-contract.md             # Contrato HTTP para frontend e consumidores da API
  mutation-testing.md         # Notas sobre mutation testing
```

Mantenha regras de domínio em `Hotel.Booking.Core`, persistência em `Hotel.Booking.Infra` e responsabilidades HTTP em `Hotel.Booking.Api`.

## Pré-requisitos

- .NET SDK 10 instalado.

Para conferir a instalação:

```powershell
dotnet --version
```

## Como executar localmente

Na raiz do repositório:

```powershell
dotnet restore
dotnet build Hotel.Booking.sln
dotnet run --project src/Hotel.Booking.Api/Hotel.Booking.Api.csproj --launch-profile Hotel.Booking
```

URLs locais do perfil `Hotel.Booking`:

- Swagger: `https://localhost:7209/swagger`
- API HTTPS: `https://localhost:7209`
- API HTTP local para frontend: `http://localhost:5209`
- Health check: `http://localhost:5209/self`

Em ambiente `Development`, a API permite CORS para:

- `http://localhost:5173`
- `http://127.0.0.1:5173`

Isso permite executar o frontend Vite localmente contra `http://localhost:5209`.

## Como testar

Para executar todos os testes:

```powershell
dotnet test Hotel.Booking.sln
```

Em fluxos com agentes, quando quiser saída mais compacta para comandos verbosos, use RTK:

```powershell
rtk dotnet test Hotel.Booking.sln
```

## Exemplos com curl

### Listar quartos

```powershell
curl.exe -i http://localhost:5209/api/v1/room
```

### Checar disponibilidade

Substitua `<ROOM_ID>` pelo id retornado na listagem de quartos.

```powershell
curl.exe -i -X POST http://localhost:5209/api/v1/booking/check-availability `
  -H "Content-Type: application/json" `
  -d '{
    "roomId": "<ROOM_ID>",
    "checkIn": "2026-07-16T00:00:00",
    "checkOut": "2026-07-18T00:00:00"
  }'
```

### Criar reserva

```powershell
curl.exe -i -X POST http://localhost:5209/api/v1/booking `
  -H "Content-Type: application/json" `
  -d '{
    "roomId": "<ROOM_ID>",
    "checkIn": "2026-07-16T00:00:00",
    "checkOut": "2026-07-18T00:00:00",
    "guestName": "Ada Lovelace"
  }'
```

## Contrato de resposta

As respostas da API são encapsuladas em `CustomResult`.

Sucesso:

```json
{
  "statusCode": 200,
  "success": true,
  "data": {},
  "errors": null
}
```

Erro:

```json
{
  "statusCode": 400,
  "success": false,
  "data": null,
  "errors": ["CheckIn is required."]
}
```

Para detalhes de modelos, endpoints, status HTTP e recomendações para frontend, consulte [`docs/api-contract.md`](docs/api-contract.md).

## Integração com frontend

O frontend local deve apontar para:

```env
VITE_API_BASE_URL=http://localhost:5209
```

Antes de validar o frontend, confirme que a API está rodando e que o endpoint de quartos responde:

```powershell
curl.exe -i http://localhost:5209/api/v1/room -H "Origin: http://127.0.0.1:5173"
```

A resposta deve retornar `200` e incluir `Access-Control-Allow-Origin` para a origem do frontend.

## Observações de desenvolvimento

- A base de dados é EF Core InMemory e é populada via seed na inicialização da API.
- Dados locais não devem ser tratados como persistentes.
- Não commite secrets, tokens ou configurações locais sensíveis.
- Mudanças em contrato HTTP devem ser refletidas em `docs/api-contract.md`.
