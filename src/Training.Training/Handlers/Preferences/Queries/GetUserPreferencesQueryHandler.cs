using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Preferences.Queries;

public class GetUserPreferencesQueryHandler(IUserPreferencesRepository repository) : IRequestHandler<GetUserPreferencesQuery, UserPreferences?>
{
    public async Task<UserPreferences?> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
