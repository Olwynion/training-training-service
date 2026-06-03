using MediatR;

namespace Training.Training.Handlers.Plans.Commands;

public record DeleteWorkoutPlanCommand(long Id, string UserId) : IRequest;
