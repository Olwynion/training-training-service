using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Plans.Commands;

public record UpdateWorkoutPlanCommand(WorkoutPlan Plan, string UserId) : IRequest<WorkoutPlan>;
