using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Exercises.Queries.GetBuiltIn;

public class GetBuiltInExercisesQueryHandler(IExerciseRepository repository) : IRequestHandler<GetBuiltInExercisesQuery, IReadOnlyList<Exercise>>
{
    public async Task<IReadOnlyList<Exercise>> Handle(GetBuiltInExercisesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetBuiltInAsync(cancellationToken);
    }
}
