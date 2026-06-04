using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Plans.Commands;

public class UpdateWorkoutPlanCommandHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<UpdateWorkoutPlanCommand, WorkoutPlan>
{
    public async Task<WorkoutPlan> Handle(UpdateWorkoutPlanCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Plan.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
        await repository.UpdateAsync(request.Plan, cancellationToken);
        return request.Plan;
    }
}
