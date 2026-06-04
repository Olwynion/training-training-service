using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.Preferences.Queries;

public record GetUserPreferencesQuery(string UserId) : IRequest<UserPreferences?>;
