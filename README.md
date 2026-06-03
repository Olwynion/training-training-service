# Training Training Service

Микросервис управления тренировочными планами и упражнениями. Входит в экосистему Training.

## Назначение

- CRUD для упражнений (exercises)
- CRUD для тренировочных планов (workout plans)
- Расчёт циклов тренировок (cycle calculation)
- Управление прогрессом (cycle/progress counter)

## Архитектура

Onion (Hexagonal) архитектура:

```
┌─────────────────────────────────────────┐
│  API (Training.Training)                │
│  - gRPC сервис (Grpc/)                  │
│  - CQRS Handlers (Handlers/)            │
│  - Startup/DI/Program                   │
├─────────────────────────────────────────┤
│  Services (Training.Training.Services)   │
│  - Proto-файлы                          │
│  - Внешние интеграции                   │
├─────────────────────────────────────────┤
│  Infrastructure (Training.Training.Infrastructure)│
│  - Репозитории (Dapper + PostgreSQL)     │
│  - CycleCalculator                       │
│  - DbConnectionFactory                   │
├─────────────────────────────────────────┤
│  Domain (Training.Training.Domain)       │
│  - Entities (Exercise, WorkoutPlan...)   │
│  - Interfaces (IExerciseRepository...)   │
│  - Domain Services (ICycleCalculator)    │
│  - Enums (MuscleGroup)                  │
└─────────────────────────────────────────┘
```

## Как работает

1. Клиент (gateway/frontend) отправляет gRPC запрос на порт 5003
2. `TrainingGrpcService` принимает запрос, парсит proto-сообщение
3. Через `IMediator` отправляет Command/Query в CQRS Handler
4. Handler выполняет бизнес-логику через Repository (Dapper → PostgreSQL)
5. Результат маппится обратно в proto-ответ

## Стек

- .NET 9
- gRPC (Grpc.AspNetCore)
- MediatR (CQRS)
- Dapper + Npgsql
- PostgreSQL (через Docker)
- xUnit + Moq (тесты)

## Запуск

```bash
# Поднять БД
docker compose up -d

# Накатить миграции
./goose.exe -dir src/Training.Training.Domain/Migrations postgres "host=localhost port=5434 user=postgres password=postgres dbname=training_training sslmode=disable" up

# Запустить сервис
dotnet run --project src/Training.Training/Training.Training.csproj
```

Сервис слушает:
- gRPC: порт 5003
- HTTP/health: порт 5000

## Proto

Proto-контракты в подмодуле `proto/contracts/` (общий репозиторий [training-contracts](https://github.com/Olwynion/training-contracts)).
