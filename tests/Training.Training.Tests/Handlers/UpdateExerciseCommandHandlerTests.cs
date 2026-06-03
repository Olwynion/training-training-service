using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Exercises.Commands;

namespace Training.Training.Tests.Handlers;

public class UpdateExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo;
    private readonly UpdateExerciseCommandHandler _handler;

    public UpdateExerciseCommandHandlerTests()
    {
        _repo = new Mock<IExerciseRepository>();
        _handler = new UpdateExerciseCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAndReturnExercise()
    {
        var exercise = Exercise.Create("Bench Press", 100, MuscleGroup.Chest, "user-1");
        _repo.Setup(r => r.GetByIdAsync("ex-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var result = await _handler.Handle(
            new UpdateExerciseCommand("ex-1", "Incline Bench", 90, MuscleGroup.Chest, "user-1"),
            CancellationToken.None);

        Assert.Equal("Incline Bench", result.Name);
        Assert.Equal(90, result.DefaultOneRm);
        _repo.Verify(r => r.Update(exercise), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        _repo.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new UpdateExerciseCommand("missing", "X", 100, MuscleGroup.Chest, "user-1"),
                CancellationToken.None));
    }
}
