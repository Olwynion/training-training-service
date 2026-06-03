using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Plans.Queries;

public record GetWorkoutPlanQuery(long Id, string UserId) : IRequest<WorkoutPlan>;
