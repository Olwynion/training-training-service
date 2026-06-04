using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.OneRms.Queries;

public class GetUserOneRmsQueryHandler(IOneRmRepository repository) : IRequestHandler<GetUserOneRmsQuery, IReadOnlyList<OneRm>>
{
    public async Task<IReadOnlyList<OneRm>> Handle(GetUserOneRmsQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
