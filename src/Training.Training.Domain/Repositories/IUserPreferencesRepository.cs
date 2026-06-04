using Training.Training.Domain.Entities;

namespace Training.Training.Domain.Repositories;

public interface IUserPreferencesRepository
{
    Task<UserPreferences?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task UpsertAsync(UserPreferences preferences, CancellationToken cancellationToken = default);
}
