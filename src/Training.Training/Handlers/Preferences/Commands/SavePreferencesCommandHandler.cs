using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Preferences.Commands;

public class SavePreferencesCommandHandler(IUserPreferencesRepository repository) : IRequestHandler<SavePreferencesCommand>
{
    public async Task Handle(SavePreferencesCommand request, CancellationToken cancellationToken)
    {
        var preferences = UserPreferences.Create(request.UserId, request.DaysPerWeek, request.ProgramType, request.FocusGroup);
        await repository.UpsertAsync(preferences, cancellationToken);
    }
}
