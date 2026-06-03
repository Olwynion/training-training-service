using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Plans.Commands;

public record CreateWorkoutPlanCommand(string UserId, string Name) : IRequest<WorkoutPlan>;
