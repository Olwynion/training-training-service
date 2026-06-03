using MediatR;

namespace Training.Training.Handlers.Plans.Commands;

public record SetCycleCommand(string PlanId, int CycleNumber, string UserId) : IRequest;
