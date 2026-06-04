using Dapper;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Infrastructure.Factories;

namespace Training.Training.Infrastructure.Repositories;

public class UserPreferencesRepository(IDbConnectionFactory connectionFactory) : IUserPreferencesRepository
{
    public async Task<UserPreferences?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        var row = await conn.QuerySingleOrDefaultAsync<dynamic>(
            new CommandDefinition(
                commandText: "SELECT * FROM user_preferences WHERE user_id = @UserId",
                parameters: new { UserId = userId },
                cancellationToken: cancellationToken));

        return row != null ? MapToPreferences(row) : null;
    }

    public async Task UpsertAsync(UserPreferences preferences, CancellationToken cancellationToken = default)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            new CommandDefinition(
                commandText: @"
                    INSERT INTO user_preferences (user_id, days_per_week, program_type, focus_group)
                    VALUES (@UserId, @DaysPerWeek, @ProgramType, @FocusGroup)
                    ON CONFLICT (user_id) DO UPDATE SET
                        days_per_week = @DaysPerWeek,
                        program_type = @ProgramType,
                        focus_group = @FocusGroup,
                        updated_at = NOW()",
                parameters: new
                {
                    preferences.UserId,
                    preferences.DaysPerWeek,
                    preferences.ProgramType,
                    FocusGroup = (int)preferences.FocusGroup
                },
                cancellationToken: cancellationToken));
    }

    private static UserPreferences MapToPreferences(dynamic row)
    {
        return UserPreferences.Hydrate(
            (string)row.user_id,
            (int)row.days_per_week,
            (string)row.program_type,
            (Domain.Enums.MuscleGroup)(int)row.focus_group);
    }
}
