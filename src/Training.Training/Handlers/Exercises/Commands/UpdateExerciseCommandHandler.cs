using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;


namespace Training.Training.Handlers.Exercises.Commands;

public class UpdateExerciseCommandHandler(IExerciseRepository repository)
    : IRequestHandler<UpdateExerciseCommand, Exercise>
{
    public async Task<Exercise> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Exercise not found");

        exercise.Update(request.Name, request.DefaultOneRm, request.MuscleGroup);
        repository.Update(exercise);
        return exercise;
    }
}
