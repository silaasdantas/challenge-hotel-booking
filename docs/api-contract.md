# API Contract for Frontend

This document describes the current HTTP contract for the hotel booking API. Use it as the practical source for building a frontend client. Swagger remains the technical contract.

## Run Locally

```powershell
dotnet run --project src/Hotel.Booking.Api/Hotel.Booking.Api.csproj
```

Default URLs:

- Swagger: `https://localhost:7209/swagger`
- Rooms: `https://localhost:7209/api/v1/room`
- Bookings: `https://localhost:7209/api/v1/booking`

## Response Envelope

All API responses are wrapped in `CustomResult`.

Success:

```json
{
  "statusCode": 200,
  "success": true,
  "data": {},
  "errors": null
}
```

Error:

```json
{
  "statusCode": 400,
  "success": false,
  "data": null,
  "errors": ["CheckIn is required."]
}
```

Frontend rule: always check `success`. Use `data` only when `success` is `true`; show `errors` when `success` is `false`.

## Data Models

### RoomResponse

```json
{
  "id": "0b5786eb-cb60-4e89-bb4a-212d58d5efcd",
  "name": "MIO Cancun Hotel Boutique, Queen Suite"
}
```

### BookingRequest

Used to create a booking and check availability.

```json
{
  "roomId": "0b5786eb-cb60-4e89-bb4a-212d58d5efcd",
  "checkIn": "2026-07-10T00:00:00",
  "checkOut": "2026-07-12T00:00:00",
  "guestName": "Ada Lovelace"
}
```

For `check-availability`, the service does not use `guestName`, but the API currently receives the same DTO and may validate it through `ModelState`. Send a non-empty `guestName` until a dedicated availability request DTO exists.

### UpdateBookingRequest

```json
{
  "bookingId": "d234a714-2298-4b7d-a957-cc4c3cc28786",
  "checkIn": "2026-07-10T00:00:00",
  "checkOut": "2026-07-12T00:00:00"
}
```

### BookingResponse

```json
{
  "id": "d234a714-2298-4b7d-a957-cc4c3cc28786",
  "checkIn": "2026-07-10T00:00:00",
  "checkOut": "2026-07-12T00:00:00",
  "guestName": "Ada Lovelace",
  "status": "ActiveBooking",
  "createdAt": "2026-07-07T10:00:00",
  "room": {
    "id": "0b5786eb-cb60-4e89-bb4a-212d58d5efcd",
    "name": "MIO Cancun Hotel Boutique, Queen Suite"
  }
}
```

Booking status values:

- `ActiveBooking`
- `BookingCanceled`
- `CheckedOut`

Availability status values:

- `None`
- `Available`
- `Booked`

## Endpoints

### List active rooms

```http
GET /api/v1/room
```

Success `200`:

```json
{
  "statusCode": 200,
  "success": true,
  "data": [
    {
      "id": "0b5786eb-cb60-4e89-bb4a-212d58d5efcd",
      "name": "MIO Cancun Hotel Boutique, Queen Suite"
    }
  ],
  "errors": null
}
```

Errors:

- `404`: no active rooms found.
- `400`: unexpected request/runtime error.

### Get room by id

```http
GET /api/v1/room/{id}
```

Success `200`: `data` is a `RoomResponse`.

Errors:

- `404`: room not found.
- `400`: unexpected request/runtime error.

### List bookings

```http
GET /api/v1/booking
```

Success `200`: `data` is `BookingResponse[]`.

Errors:

- `404`: no bookings found.
- `400`: unexpected request/runtime error.

### Get booking by id

```http
GET /api/v1/booking/{id}
```

Success `200`: `data` is a `BookingResponse`.

Errors:

- `404`: booking not found.
- `400`: unexpected request/runtime error.

### Create booking

```http
POST /api/v1/booking
Content-Type: application/json
```

Request: `BookingRequest`.

Success `201`: `data` is a `BookingResponse`.

Errors:

- `400`: invalid request or date rule violation.
- `404`: room not found.
- `409`: room unavailable for the requested dates.

### Update booking dates

```http
PUT /api/v1/booking
Content-Type: application/json
```

Request: `UpdateBookingRequest`.

Success `200`: `data` is a `BookingResponse`.

Errors:

- `400`: invalid request or date rule violation.
- `404`: booking not found.
- `409`: requested dates conflict with another active booking.

### Cancel booking

```http
PUT /api/v1/booking/cancel/{id}
```

Success `200`:

```json
{
  "statusCode": 200,
  "success": true,
  "data": "Booking successfully canceled",
  "errors": null
}
```

Errors:

- `404`: booking not found.
- `400`: unexpected request/runtime error.

### Checkout booking

```http
PUT /api/v1/booking/checkout/{id}
```

Success `200`:

```json
{
  "statusCode": 200,
  "success": true,
  "data": "Booking successfully checked out",
  "errors": null
}
```

Errors:

- `400`: canceled booking cannot be checked out.
- `404`: booking not found.

### Check room availability

```http
POST /api/v1/booking/check-availability
Content-Type: application/json
```

Request: `BookingRequest`.

Success `200`:

```json
{
  "statusCode": 200,
  "success": true,
  "data": {
    "status": "Available",
    "message": "Room available to book"
  },
  "errors": null
}
```

Errors:

- `400`: invalid request or date rule violation.
- `404`: room not found.
- `409`: room unavailable for the requested dates.

## Domain Rules for UI

- `roomId` cannot be empty.
- `bookingId` cannot be empty when updating.
- `guestName` is required to create a booking.
- `checkIn` and `checkOut` cannot be default dates.
- `checkIn` must be at least tomorrow.
- `checkOut` must be greater than or equal to `checkIn`.
- Booking cannot start more than 30 days in advance.
- Stay length cannot be greater than 3 days.
- Date conflict returns `409 Conflict`.
- Missing resource returns `404 Not Found`.
- Validation errors return `400 Bad Request`.

## Suggested Frontend Screens

- Rooms list.
- Availability check form.
- Booking creation form.
- Bookings list.
- Booking details.
- Update booking dates.
- Cancel booking action.
- Checkout booking action.

## Frontend Prompt

```text
Leia docs/api-contract.md.
Crie um frontend para consumir esta API de reserva de hotel.
Implemente telas para:
- listar quartos disponíveis;
- checar disponibilidade;
- criar reserva;
- listar reservas;
- atualizar reserva;
- cancelar reserva;
- realizar checkout.

Regras:
- consumir apenas os endpoints documentados;
- tratar o envelope CustomResult;
- exibir erros 400, 404 e 409 de forma clara;
- manter UI simples, responsiva e operacional;
- não inventar endpoints;
- se faltar contrato, pergunte antes de implementar.
```

## Notes for Client Implementation

- Prefer a typed API client module instead of calling `fetch` directly from components.
- Centralize envelope handling in one helper.
- Keep HTTP status handling explicit: `400`, `404`, `409`, success.
- Do not assume `data` exists when `success` is `false`.
- Dates should be sent as ISO strings.
