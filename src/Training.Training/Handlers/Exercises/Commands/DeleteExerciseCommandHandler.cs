using MediatR;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Exercises.Commands;

public class DeleteExerciseCommandHandler(IExerciseRepository repository)
    : IRequestHandler<DeleteExerciseCommand>
{
    public async Task Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Exercise not found");
        repository.Delete(exercise);
    }
}
