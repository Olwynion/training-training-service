using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Plans.Queries;

public class GetWorkoutPlanQueryHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<GetWorkoutPlanQuery, WorkoutPlan>
{
    public async Task<WorkoutPlan> Handle(GetWorkoutPlanQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
    }
}
