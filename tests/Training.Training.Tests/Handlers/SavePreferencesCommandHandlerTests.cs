using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Preferences.Commands;

namespace Training.Training.Tests.Handlers;

public class SavePreferencesCommandHandlerTests
{
    private readonly Mock<IUserPreferencesRepository> _repo;
    private readonly SavePreferencesCommandHandler _handler;

    public SavePreferencesCommandHandlerTests()
    {
        _repo = new Mock<IUserPreferencesRepository>();
        _handler = new SavePreferencesCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpsertPreferences()
    {
        var command = new SavePreferencesCommand("user-1", 4, "split", MuscleGroup.Chest);

        await _handler.Handle(command, CancellationToken.None);

        _repo.Verify(r => r.UpsertAsync(It.IsAny<UserPreferences>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
