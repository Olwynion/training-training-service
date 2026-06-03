using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Plans.Queries;

public record GetUserWorkoutPlansQuery(string UserId) : IRequest<IReadOnlyList<WorkoutPlan>>;
