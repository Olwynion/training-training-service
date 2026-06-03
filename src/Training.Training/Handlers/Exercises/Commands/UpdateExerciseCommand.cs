using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;

namespace Training.Training.Handlers.Exercises.Commands;

public record UpdateExerciseCommand(
    string Id, string Name, double DefaultOneRm,
    MuscleGroup MuscleGroup, string UserId) : IRequest<Exercise>;
