using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Exercises.Queries;

public class GetExercisesQueryHandler(IExerciseRepository repository)
    : IRequestHandler<GetExercisesQuery, IReadOnlyList<Exercise>>
{
    public async Task<IReadOnlyList<Exercise>> Handle(GetExercisesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
