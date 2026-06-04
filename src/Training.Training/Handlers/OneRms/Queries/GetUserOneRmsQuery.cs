using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.OneRms.Queries;

public record GetUserOneRmsQuery(string UserId) : IRequest<IReadOnlyList<OneRm>>;
