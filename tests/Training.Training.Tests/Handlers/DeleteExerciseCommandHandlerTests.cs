using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Exercises.Commands;

namespace Training.Training.Tests.Handlers;

public class DeleteExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo;
    private readonly DeleteExerciseCommandHandler _handler;

    public DeleteExerciseCommandHandlerTests()
    {
        _repo = new Mock<IExerciseRepository>();
        _handler = new DeleteExerciseCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteExercise()
    {
        var exercise = Exercise.Create("Bench", 100, MuscleGroup.Chest, "user-1");
        _repo.Setup(r => r.GetByIdAsync(exercise.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        await _handler.Handle(
            new DeleteExerciseCommand(exercise.Id, "user-1"),
            CancellationToken.None);

        _repo.Verify(r => r.Delete(exercise), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        _repo.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new DeleteExerciseCommand("missing", "user-1"),
                CancellationToken.None));
    }
}
