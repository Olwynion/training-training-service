using Training.Training.Domain.Entities;

namespace Training.Training.Domain.Repositories;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exercise>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default);
    void Update(Exercise exercise);
    void Delete(Exercise exercise);
}
