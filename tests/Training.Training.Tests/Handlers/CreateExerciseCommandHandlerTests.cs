using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Exercises.Commands;

namespace Training.Training.Tests.Handlers;

public class CreateExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo;
    private readonly CreateExerciseCommandHandler _handler;

    public CreateExerciseCommandHandlerTests()
    {
        _repo = new Mock<IExerciseRepository>();
        _handler = new CreateExerciseCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateAndReturnExercise()
    {
        var result = await _handler.Handle(
            new CreateExerciseCommand("Bench Press", 100, MuscleGroup.Chest, "user-1"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Bench Press", result.Name);
        Assert.Equal(100, result.DefaultOneRm);
        Assert.Equal(MuscleGroup.Chest, result.MuscleGroup);
        Assert.Equal("user-1", result.UserId);
        _repo.Verify(r => r.AddAsync(result, It.IsAny<CancellationToken>()), Times.Once);
    }
}
