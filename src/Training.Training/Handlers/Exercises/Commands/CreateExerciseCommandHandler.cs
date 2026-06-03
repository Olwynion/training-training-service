using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Exercises.Commands;

public class CreateExerciseCommandHandler(IExerciseRepository repository)
    : IRequestHandler<CreateExerciseCommand, Exercise>
{
    public async Task<Exercise> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = Exercise.Create(request.Name, request.DefaultOneRm, request.MuscleGroup, request.UserId);
        await repository.AddAsync(exercise, cancellationToken);
        return exercise;
    }
}
