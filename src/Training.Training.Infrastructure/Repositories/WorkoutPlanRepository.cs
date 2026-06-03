using Dapper;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Infrastructure.Factories;

namespace Training.Training.Infrastructure.Repositories;

public class WorkoutPlanRepository(IDbConnectionFactory connectionFactory) : IWorkoutPlanRepository
{
    public async Task<WorkoutPlan?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM workout_plans WHERE id = @Id",
                parameters: new { Id = id },
                cancellationToken: cancellationToken));

        return row != null ? MapToWorkoutPlan(row) : null;
    }

    public async Task<IReadOnlyList<WorkoutPlan>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var rows = await conn.QueryAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM workout_plans WHERE user_id = @UserId ORDER BY created_at DESC",
                parameters: new { UserId = userId },
                cancellationToken: cancellationToken));

        return rows.Select(MapToWorkoutPlan).ToList();
    }

    public async Task AddAsync(WorkoutPlan plan, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var id = await conn.ExecuteScalarAsync<long>(
            new CommandDefinition(
                commandText: @"
                    INSERT INTO workout_plans (user_id, name, cycle_number, progress_counter, created_at)
                    VALUES (@UserId, @Name, @CycleNumber, @ProgressCounter, @CreatedAt)
                    RETURNING id",
                parameters: new
                {
                    plan.UserId,
                    plan.Name,
                    plan.CycleNumber,
                    plan.ProgressCounter,
                    plan.CreatedAt
                },
                cancellationToken: cancellationToken));

        plan.SetId(id);
    }

    public void Update(WorkoutPlan plan)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Execute(@"
            UPDATE workout_plans SET
                name = @Name, cycle_number = @CycleNumber, progress_counter = @ProgressCounter
            WHERE id = @Id",
            new
            {
                plan.Id,
                plan.Name,
                plan.CycleNumber,
                plan.ProgressCounter
            });
    }

    public void Delete(WorkoutPlan plan)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Execute("DELETE FROM workout_plans WHERE id = @Id",
            new { plan.Id });
    }

    public async Task UpdateCycleAsync(long id, int cycleNumber, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            new CommandDefinition(
                commandText: "UPDATE workout_plans SET cycle_number = @CycleNumber WHERE id = @Id",
                parameters: new { Id = id, CycleNumber = cycleNumber },
                cancellationToken: cancellationToken));
    }

    public async Task IncrementProgressAsync(long id, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            new CommandDefinition(
                commandText: "UPDATE workout_plans SET progress_counter = progress_counter + 1 WHERE id = @Id",
                parameters: new { Id = id },
                cancellationToken: cancellationToken));
    }

    private static WorkoutPlan MapToWorkoutPlan(dynamic row)
    {
        return WorkoutPlan.Hydrate(
            (long)row.id,
            (string)row.user_id,
            (string)row.name,
            (int)row.cycle_number,
            (int)row.progress_counter,
            (DateTime)row.created_at);
    }
}
