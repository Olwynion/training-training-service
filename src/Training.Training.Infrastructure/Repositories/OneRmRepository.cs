using Dapper;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Infrastructure.Factories;

namespace Training.Training.Infrastructure.Repositories;

public class OneRmRepository(IDbConnectionFactory connectionFactory) : IOneRmRepository
{
    public async Task<IReadOnlyList<OneRm>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var rows = await conn.QueryAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM one_rms WHERE user_id = @UserId ORDER BY exercise_id",
                parameters: new { UserId = userId },
                cancellationToken: cancellationToken));

        return rows.Select(MapToOneRm).ToList();
    }

    public async Task UpsertAsync(OneRm oneRm, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            new CommandDefinition(
                commandText: @"
                    INSERT INTO one_rms (user_id, exercise_id, one_rm)
                    VALUES (@UserId, @ExerciseId, @Value)
                    ON CONFLICT (user_id, exercise_id) DO UPDATE SET
                        one_rm = @Value,
                        updated_at = NOW()",
                parameters: new
                {
                    oneRm.UserId,
                    oneRm.ExerciseId,
                    oneRm.Value
                },
                cancellationToken: cancellationToken));
    }

    private static OneRm MapToOneRm(dynamic row)
    {
        return OneRm.Hydrate(
            (long)row.id,
            (string)row.user_id,
            (long)row.exercise_id,
            (double)row.one_rm);
    }
}
