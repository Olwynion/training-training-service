using MediatR;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Plans.Commands;

public class IncrementProgressCommandHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<IncrementProgressCommand>
{
    public async Task Handle(IncrementProgressCommand request, CancellationToken cancellationToken)
    {
        var plan = await repository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
        plan.IncrementProgress();
        await repository.IncrementProgressAsync(plan.Id, cancellationToken);
    }
}
