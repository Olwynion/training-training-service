using MediatR;
using Training.Training.Domain.Services;

namespace Training.Training.Handlers.Plans.Commands;

public record GetCycleDataCommand(long PlanId, string UserId) : IRequest<List<CycleDataResult>>;
