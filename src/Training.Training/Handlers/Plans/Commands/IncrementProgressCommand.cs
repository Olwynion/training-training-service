using MediatR;

namespace Training.Training.Handlers.Plans.Commands;

public record IncrementProgressCommand(string PlanId, string UserId) : IRequest;
