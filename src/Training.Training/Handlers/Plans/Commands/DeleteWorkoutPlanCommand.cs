using MediatR;

namespace Training.Training.Handlers.Plans.Commands;

public record DeleteWorkoutPlanCommand(string Id, string UserId) : IRequest;
