using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Plans.Queries;

public class GetUserWorkoutPlansQueryHandler(IWorkoutPlanRepository repository)
    : IRequestHandler<GetUserWorkoutPlansQuery, IReadOnlyList<WorkoutPlan>>
{
    public async Task<IReadOnlyList<WorkoutPlan>> Handle(GetUserWorkoutPlansQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
