using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Exercises.Queries;

namespace Training.Training.Tests.Handlers;

public class GetExercisesQueryHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo;
    private readonly GetExercisesQueryHandler _handler;

    public GetExercisesQueryHandlerTests()
    {
        _repo = new Mock<IExerciseRepository>();
        _handler = new GetExercisesQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnExercises()
    {
        var exercises = new List<Exercise>
        {
            Exercise.Create("Bench", 100, MuscleGroup.Chest, "user-1"),
            Exercise.Create("Squat", 150, MuscleGroup.Legs, "user-1")
        };
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        var result = await _handler.Handle(
            new GetExercisesQuery("user-1"),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WithNoExercises_ShouldReturnEmpty()
    {
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetExercisesQuery("user-1"),
            CancellationToken.None);

        Assert.Empty(result);
    }
}
