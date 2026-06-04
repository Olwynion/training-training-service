using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Exercises.Queries.GetBuiltIn;

public record GetBuiltInExercisesQuery : IRequest<IReadOnlyList<Exercise>>;
