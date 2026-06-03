using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Exercises.Queries;

public record GetExercisesQuery(string UserId) : IRequest<IReadOnlyList<Exercise>>;
