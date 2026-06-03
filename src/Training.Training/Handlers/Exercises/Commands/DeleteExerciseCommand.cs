using MediatR;

namespace Training.Training.Handlers.Exercises.Commands;

public record DeleteExerciseCommand(long Id, string UserId) : IRequest;
