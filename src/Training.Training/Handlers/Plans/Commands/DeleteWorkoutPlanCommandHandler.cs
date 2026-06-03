using MediatR;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Plans.Commands;

public class DeleteWorkoutPlanCommandHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<DeleteWorkoutPlanCommand>
{
    public async Task Handle(DeleteWorkoutPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
        repository.Delete(plan);
    }
}
