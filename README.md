# GameApi

GameApi is a small ASP.NET Core Web API for managing a catalog of games. The project uses a simple in-memory store, DTO-based request and response models, and a validation middleware for request-body checks.

## Overview

The solution contains one API project, `GamestoreApi`, which is organized around a few clear layers:

- `Controller` exposes HTTP endpoints.
- `Service` holds the game catalog logic and in-memory data.
- `Dto` defines the request and response shapes.
- `Middleware` contains request validation logic for create and update requests.
- `Model` contains the internal data model used by the service.

The current API exposes a `GET` endpoint for listing all games, while the service also already includes methods for getting, creating, updating, and deleting a game by id.

## Project Structure

```text
GamestoreApi/
 Program.cs
 Controller/
  GameStoreApiController.cs
 Service/
  GameStoreService.cs
  IGameStoreCharacterService.cs
 Dto/
  GameFlowResponse.cs
  GameFlowStoreApiCreateRequest.cs
  GameStoreApiUpdateRequest.cs
 Model/
  GameStoreApiModel.cs
 Middleware/
  GameStoreApiValidationMiddleware.cs
```

## Running The API

From the `GamestoreApi` folder, run:

```bash
dotnet run
```

The app starts with controller routing enabled and a root endpoint that returns `Hello World!`.

## API Endpoints

### Get all games

- `GET /v1/api/GameStoreApi`

Returns the current list of games from the in-memory store.

Example response:

```json
[
 {
  "id": 1,
  "name": "The Legend of Zelda: Breath of the Wild",
  "genre": "Action-Adventure",
  "price": 59.99,
  "releaseDate": "2017-03-03"
 }
]
```

## DTOs

### `GameFlowResponse`

Used as the response shape for game data returned by the service and controller.

Fields:

- `Id`
- `Name`
- `Genre`
- `Price`
- `ReleaseDate`

### `GameFlowStoreApiCreateRequest`

Used for create requests. The service expects the same core game fields when adding a new game.

### `GameStoreApiUpdateRequest`

Used for update requests. The current middleware requires the same fields for validation.

## Service Layer

`GameStoreService` is the in-memory implementation of `IGameStoreCharacterService`.

It currently supports:

- `GetAllGamesStoreAsync()`
- `GetGameByIdAsync(int id)`
- `CreateGameAsync(GameFlowStoreApiCreateRequest store)`
- `UpdateGameAsync(int id, GameStoreApiUpdateRequest store)`
- `DeleteGameAsync(int id)`

The service keeps game data in a list inside memory, so data resets when the application restarts.

## Validation Middleware

`GameStoreApiValidationMiddleware` inspects incoming `POST` and `PUT` requests and checks the body for required fields.

Current rules:

- `POST /v1/api/GameStoreApi`
- `PUT /v1/api/GameStoreApi/{id}`

Required fields:

- `name`
- `genre`
- `price`
- `releaseDate`

If validation fails, the middleware returns `400 Bad Request` with a JSON payload that includes the missing fields.

Example failure response:

```json
{
 "error": "Validation failed",
 "missing": ["name", "price"]
}
```

### Important note

The middleware class exists in the codebase, but it is not currently registered in `Program.cs`. If you want it to run for requests, it needs to be added to the middleware pipeline with `app.UseMiddleware<GameStoreApiValidationMiddleware>()` before `app.MapControllers()`.

## Configuration Notes

- The API uses `AddControllers()` and `MapControllers()`.
- `GameStoreService` is registered as a scoped service.
- The project targets `.NET 10.0`.
- Scalar.AspNetCore package provides interactive API documentation UI.
- Microsoft.AspNetCore.OpenApi package provides OpenAPI/Swagger support.

## Middleware Pipeline

The middleware is now active in the following order:

1. **Exception Handling** (`GameStoreapiExceptionHandlingMiddleware`) — Catches and logs unhandled exceptions.
2. **Request Logging** (`GameStoreApiRequestLoggingMiddleware`) — Logs incoming requests.
3. **Guard** (`GameStoreApiGuard`) — Enforces security requirements for protected routes.
4. **Validation** (`GameStoreApiValidationMiddleware`) — Validates request bodies for POST/PUT operations.

## Security Guard

The `GameStoreApiGuard` middleware protects the `/v1/api/GameStoreApi` route and requires **all** of the following query parameters:

- `secure=true`
- `token=valid-token`
- `input=<alphanumeric, max 100 chars, no script tags>`

Example valid request:

```
GET /v1/api/GameStoreApi?secure=true&token=valid-token&input=games
```

Without these parameters, the API returns:

- `401 Unauthorized` if `secure` is missing
- `403 Forbidden` if `token` is invalid
- `400 Bad Request` if `input` is invalid

### Exempt Paths

The following paths **bypass** the guard and do not require query parameters:

- `/scalar/*` — Interactive API documentation (Scalar UI)
- `/openapi/*` — OpenAPI/Swagger documentation
- `/swagger/*` — Swagger UI (if configured)
- `/health` — Health check endpoint
- `/` — Root endpoint

This allows clients to:

1. Access Scalar UI at `/scalar/v1` without guard parameters
2. Browse OpenAPI schema at `/openapi/v1.json` without guard parameters
3. Use Scalar to discover and test endpoints interactively
4. Call the actual `/v1/api/GameStoreApi` endpoints with the required guard parameters

## API Documentation

- **Swagger/OpenAPI**: Available at `/openapi/v1.json` (no guard required)
- **Scalar UI**: Available at `/scalar/v1` (no guard required) — Interactive documentation

**To test API endpoints via Scalar**, include the required guard parameters in the query string:

```
?secure=true&token=valid-token&input=test
```

## Recent Changes

- Fixed NRT (non-nullable reference type) warnings in DTOs by making string properties nullable in update requests.
- Implemented all interface methods in `GameStoreService` (`GetAllGamesStoreAsync`, `GetGameByIdAsync`, `CreateGameAsync`, `UpdateGameAsync`, `DeleteGameAsync`).
- Fixed LINQ predicate scope issue in `GameStoreApiValidationMiddleware`.
- Added security middleware requiring query parameters for protected routes.
- Integrated Scalar.AspNetCore for interactive API documentation.
- Integrated Microsoft.AspNetCore.OpenApi for OpenAPI/Swagger support.
- Fixed path validation in `GameStoreApiGuard` to include leading `/`.
- All middleware is now registered and active in the pipeline.
- **Fixed**: Scalar and documentation endpoints now bypass the guard middleware, allowing easy client access to API documentation while retaining security logic for actual API endpoints.

## Next Improvements

- Add `GET /v1/api/GameStoreApi/{id}`
- Add `POST`, `PUT`, and `DELETE` controller actions
- Optionally disable guard middleware for development
- Replace the in-memory list with a database-backed repository
