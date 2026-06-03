using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Plans.Commands;

public class CreateWorkoutPlanCommandHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<CreateWorkoutPlanCommand, WorkoutPlan>
{
    public async Task<WorkoutPlan> Handle(CreateWorkoutPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = WorkoutPlan.Create(request.UserId, request.Name);
        await repository.AddAsync(plan, cancellationToken);
        return plan;
    }
}
