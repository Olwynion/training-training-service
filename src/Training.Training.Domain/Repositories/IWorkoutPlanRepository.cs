using Training.Training.Domain.Entities;

namespace Training.Training.Domain.Repositories;

public interface IWorkoutPlanRepository
{
    Task<WorkoutPlan?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutPlan>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkoutPlan plan, CancellationToken cancellationToken = default);
    void Update(WorkoutPlan plan);
    void Delete(WorkoutPlan plan);
    Task UpdateCycleAsync(long id, int cycleNumber, CancellationToken cancellationToken = default);
    Task IncrementProgressAsync(long id, CancellationToken cancellationToken = default);
}
