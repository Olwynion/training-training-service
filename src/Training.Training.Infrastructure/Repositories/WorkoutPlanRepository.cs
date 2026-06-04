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

        var planRow = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition("SELECT * FROM workout_plans WHERE id = @Id",
                parameters: new { Id = id },
                cancellationToken: cancellationToken));

        if (planRow == null) return null;

        var plan = MapToWorkoutPlan(planRow);
        await LoadDaysAsync(conn, plan, cancellationToken);
        return plan;
    }

    public async Task<IReadOnlyList<WorkoutPlan>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();

        var rows = await conn.QueryAsync<dynamic>(
            new CommandDefinition("SELECT * FROM workout_plans WHERE user_id = @UserId ORDER BY created_at DESC",
                parameters: new { UserId = userId },
                cancellationToken: cancellationToken));

        var plans = rows.Select(MapToWorkoutPlan).ToList();

        foreach (var plan in plans)
            await LoadDaysAsync(conn, plan, cancellationToken);

        return plans;
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

    public async Task UpdateAsync(WorkoutPlan plan, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[Repo.UpdateAsync] PlanId={plan.Id}, days={plan.Days.Count}");

        using var conn = connectionFactory.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        await conn.ExecuteAsync(
            new CommandDefinition(@"
                UPDATE workout_plans SET
                    name = @Name, cycle_number = @CycleNumber, progress_counter = @ProgressCounter
                WHERE id = @Id",
                parameters: new { plan.Id, plan.Name, plan.CycleNumber, plan.ProgressCounter },
                transaction: tx,
                cancellationToken: cancellationToken));

        Console.WriteLine("[Repo.UpdateAsync] Deleted old plan_days");
        await conn.ExecuteAsync(
            new CommandDefinition("DELETE FROM plan_days WHERE workout_plan_id = @Id",
                parameters: new { plan.Id },
                transaction: tx,
                cancellationToken: cancellationToken));

        foreach (var day in plan.Days)
        {
            Console.WriteLine($"[Repo.UpdateAsync] Inserting day '{day.DayName}'");
            var dayId = await conn.ExecuteScalarAsync<long>(
                new CommandDefinition(@"
                    INSERT INTO plan_days (workout_plan_id, day_name, focus_group, sort_order)
                    VALUES (@WorkoutPlanId, @DayName, @FocusGroup, @SortOrder)
                    RETURNING id",
                    parameters: new
                    {
                        WorkoutPlanId = plan.Id,
                        day.DayName,
                        FocusGroup = (int)day.FocusGroup,
                        day.SortOrder
                    },
                    transaction: tx,
                    cancellationToken: cancellationToken));

            Console.WriteLine($"[Repo.UpdateAsync] Day inserted with id={dayId}, exercises={day.Exercises.Count}");
            day.SetId(dayId);

            foreach (var ex in day.Exercises)
            {
                Console.WriteLine($"[Repo.UpdateAsync] Inserting ex exerciseId={ex.ExerciseId}, name='{ex.ExerciseName}'");
                var exId = await conn.ExecuteScalarAsync<long>(
                    new CommandDefinition(@"
                        INSERT INTO day_exercises (plan_day_id, exercise_id, exercise_name, sets, sort_order)
                        VALUES (@PlanDayId, @ExerciseId, @ExerciseName, @Sets, @SortOrder)
                        RETURNING id",
                        parameters: new
                        {
                            PlanDayId = dayId,
                            ex.ExerciseId,
                            ex.ExerciseName,
                            ex.Sets,
                            ex.SortOrder
                        },
                        transaction: tx,
                        cancellationToken: cancellationToken));

                Console.WriteLine($"[Repo.UpdateAsync] Ex inserted with id={exId}");
                ex.SetId(exId);
            }
        }

        tx.Commit();
        Console.WriteLine("[Repo.UpdateAsync] Transaction committed OK");
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

    private static async Task LoadDaysAsync(System.Data.IDbConnection conn, WorkoutPlan plan, CancellationToken ct)
    {
        var dayRows = await conn.QueryAsync<dynamic>(
            new CommandDefinition(
                "SELECT * FROM plan_days WHERE workout_plan_id = @Id ORDER BY sort_order",
                parameters: new { plan.Id },
                cancellationToken: ct));

        foreach (var d in dayRows)
        {
            var planDay = PlanDay.Hydrate(
                (long)d.id,
                (string)d.day_name,
                (Domain.Enums.MuscleGroup)(int)d.focus_group,
                (int)d.sort_order);

            var exRows = await conn.QueryAsync<dynamic>(
                new CommandDefinition(
                    "SELECT * FROM day_exercises WHERE plan_day_id = @PlanDayId ORDER BY sort_order",
                    parameters: new { PlanDayId = planDay.Id },
                    cancellationToken: ct));

            foreach (var e in exRows)
            {
                var dayEx = DayExercise.Hydrate(
                    (long)e.id,
                    (long)e.exercise_id,
                    (string)e.exercise_name,
                    (int)e.sets,
                    (int)e.sort_order);
                planDay.AddExercise(dayEx);
            }

            plan.AddDay(planDay);
        }
    }
}
