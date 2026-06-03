using MediatR;

namespace Training.Training.Handlers.Plans.Commands;

public record IncrementProgressCommand(long PlanId, string UserId) : IRequest;
