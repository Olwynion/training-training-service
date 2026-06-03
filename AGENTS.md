# training-training-service

Микросервис управления тренировочными планами и упражнениями.

## Назначение

Управление упражнениями (CRUD), тренировочными планами (CRUD), расчёт тренировочных циклов и прогресса. Использует PostgreSQL через Dapper.

## Как работает

Onion-архитектура (Domain → Infrastructure → Services → API). 
gRPC запросы принимаются на порту 5003, проходят через MediatR CQRS Handlers, бизнес-логика выполняется в репозиториях (Dapper SQL) и сервисах (CycleCalculator).

## Команды

```bash
# Build
dotnet build

# Test
dotnet test

# Run (требуется Docker с БД)
dotnet run --project src/Training.Training

# Миграции
goose -dir src/Training.Training.Domain/Migrations postgres "host=localhost port=5434 user=postgres password=postgres dbname=training_training sslmode=disable" up
```

Подробнее: см. README.md
