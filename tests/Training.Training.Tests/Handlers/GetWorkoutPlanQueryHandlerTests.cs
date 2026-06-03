using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Queries;

namespace Training.Training.Tests.Handlers;

public class GetWorkoutPlanQueryHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly GetWorkoutPlanQueryHandler _handler;

    public GetWorkoutPlanQueryHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _handler = new GetWorkoutPlanQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPlan()
    {
        var plan = WorkoutPlan.Create("user-1", "My Plan");
        _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var result = await _handler.Handle(
            new GetWorkoutPlanQuery(1, "user-1"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("My Plan", result.Name);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        _repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutPlan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new GetWorkoutPlanQuery(999, "user-1"),
                CancellationToken.None));
    }
}
