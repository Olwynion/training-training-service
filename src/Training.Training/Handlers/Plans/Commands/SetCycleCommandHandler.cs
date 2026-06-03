using MediatR;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Plans.Commands;

public class SetCycleCommandHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<SetCycleCommand>
{
    public async Task Handle(SetCycleCommand request, CancellationToken cancellationToken)
    {
        var plan = await repository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
        plan.SetCycle(request.CycleNumber);
        await repository.UpdateCycleAsync(plan.Id, request.CycleNumber, cancellationToken);
    }
}
