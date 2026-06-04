using Training.Training.Domain.Entities;

namespace Training.Training.Domain.Repositories;

public interface IOneRmRepository
{
    Task<IReadOnlyList<OneRm>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task UpsertAsync(OneRm oneRm, CancellationToken cancellationToken = default);
}
