using MediatR;

namespace Training.Training.Handlers.Exercises.Commands;

public record DeleteExerciseCommand(string Id, string UserId) : IRequest;
