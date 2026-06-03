using MediatR;

namespace Training.Training.Handlers.Plans.Commands;

public record SetCycleCommand(long PlanId, int CycleNumber, string UserId) : IRequest;
