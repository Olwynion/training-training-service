using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.OneRms.Commands;

namespace Training.Training.Tests.Handlers;

public class SaveOneRmsCommandHandlerTests
{
    private readonly Mock<IOneRmRepository> _repo;
    private readonly SaveOneRmsCommandHandler _handler;

    public SaveOneRmsCommandHandlerTests()
    {
        _repo = new Mock<IOneRmRepository>();
        _handler = new SaveOneRmsCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpsertAllEntries()
    {
        var command = new SaveOneRmsCommand("user-1",
        [
            (1, 100.0),
            (2, 150.0)
        ]);

        await _handler.Handle(command, CancellationToken.None);

        _repo.Verify(r => r.UpsertAsync(It.IsAny<OneRm>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithNoEntries_ShouldNotCallUpsert()
    {
        var command = new SaveOneRmsCommand("user-1", []);

        await _handler.Handle(command, CancellationToken.None);

        _repo.Verify(r => r.UpsertAsync(It.IsAny<OneRm>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
