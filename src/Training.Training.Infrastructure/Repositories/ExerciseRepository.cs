using Dapper;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Infrastructure.Factories;

namespace Training.Training.Infrastructure.Repositories;

public class ExerciseRepository(IDbConnectionFactory connectionFactory) : IExerciseRepository
{
    public async Task<Exercise?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM exercises WHERE id = @Id",
                parameters: new { Id = id },
                cancellationToken: cancellationToken));

        return row != null ? MapToExercise(row) : null;
    }

    public async Task<IReadOnlyList<Exercise>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var rows = await conn.QueryAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM exercises WHERE user_id = @UserId OR is_built_in = true ORDER BY name",
                parameters: new { UserId = userId },
                cancellationToken: cancellationToken));

        return rows.Select(MapToExercise).ToList();
    }

    public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            new CommandDefinition(
                commandText: @"
                    INSERT INTO exercises (id, name, default_one_rm, muscle_group, user_id, is_built_in)
                    VALUES (@Id, @Name, @DefaultOneRm, @MuscleGroup, @UserId, @IsBuiltIn)",
                parameters: new
                {
                    exercise.Id,
                    exercise.Name,
                    exercise.DefaultOneRm,
                    MuscleGroup = (int)exercise.MuscleGroup,
                    exercise.UserId,
                    exercise.IsBuiltIn
                },
                cancellationToken: cancellationToken));
    }

    public void Update(Exercise exercise)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Execute(@"
            UPDATE exercises SET
                name = @Name, default_one_rm = @DefaultOneRm,
                muscle_group = @MuscleGroup
            WHERE id = @Id",
            new
            {
                exercise.Id,
                exercise.Name,
                exercise.DefaultOneRm,
                MuscleGroup = (int)exercise.MuscleGroup
            });
    }

    public void Delete(Exercise exercise)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Execute("DELETE FROM exercises WHERE id = @Id",
            new { exercise.Id });
    }

    private static Exercise MapToExercise(dynamic row)
    {
        return Exercise.Hydrate(
            (string)row.id,
            (string)row.name,
            (double)row.default_one_rm,
            (Domain.Enums.MuscleGroup)(int)row.muscle_group,
            (string)row.user_id,
            (bool)row.is_built_in);
    }
}
