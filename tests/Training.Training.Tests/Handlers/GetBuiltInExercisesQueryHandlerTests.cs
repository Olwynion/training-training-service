using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Exercises.Queries.GetBuiltIn;

namespace Training.Training.Tests.Handlers;

public class GetBuiltInExercisesQueryHandlerTests
{
    private readonly Mock<IExerciseRepository> _repo;
    private readonly GetBuiltInExercisesQueryHandler _handler;

    public GetBuiltInExercisesQueryHandlerTests()
    {
        _repo = new Mock<IExerciseRepository>();
        _handler = new GetBuiltInExercisesQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBuiltInExercises()
    {
        var exercises = new List<Exercise>
        {
            Exercise.Create("Bench", 100, MuscleGroup.Chest, "built-in"),
            Exercise.Create("Squat", 150, MuscleGroup.Legs, "built-in")
        };
        _repo.Setup(r => r.GetBuiltInAsync(It.IsAny<CancellationToken>())).ReturnsAsync(exercises);

        var result = await _handler.Handle(new GetBuiltInExercisesQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Name == "Bench");
    }

    [Fact]
    public async Task Handle_WhenEmpty_ShouldReturnEmpty()
    {
        _repo.Setup(r => r.GetBuiltInAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _handler.Handle(new GetBuiltInExercisesQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
