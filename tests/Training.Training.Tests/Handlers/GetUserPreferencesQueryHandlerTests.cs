using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Preferences.Queries;

namespace Training.Training.Tests.Handlers;

public class GetUserPreferencesQueryHandlerTests
{
    private readonly Mock<IUserPreferencesRepository> _repo;
    private readonly GetUserPreferencesQueryHandler _handler;

    public GetUserPreferencesQueryHandlerTests()
    {
        _repo = new Mock<IUserPreferencesRepository>();
        _handler = new GetUserPreferencesQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPreferences()
    {
        var prefs = UserPreferences.Hydrate("user-1", 4, "split", MuscleGroup.Chest);
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(prefs);

        var result = await _handler.Handle(new GetUserPreferencesQuery("user-1"), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(4, result.DaysPerWeek);
    }

    [Fact]
    public async Task Handle_WhenNoPreferences_ShouldReturnNull()
    {
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync((UserPreferences?)null);

        var result = await _handler.Handle(new GetUserPreferencesQuery("user-1"), CancellationToken.None);

        Assert.Null(result);
    }
}
