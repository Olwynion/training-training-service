using Training.Training.Domain.Entities;

namespace Training.Training.Domain.Repositories;

public interface IWorkoutPlanRepository
{
    Task<WorkoutPlan?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutPlan>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkoutPlan plan, CancellationToken cancellationToken = default);
    void Update(WorkoutPlan plan);
    void Delete(WorkoutPlan plan);
    Task UpdateCycleAsync(string id, int cycleNumber, CancellationToken cancellationToken = default);
    Task IncrementProgressAsync(string id, CancellationToken cancellationToken = default);
}
