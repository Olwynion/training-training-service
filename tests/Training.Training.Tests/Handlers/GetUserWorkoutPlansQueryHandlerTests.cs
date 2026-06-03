using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Queries;

namespace Training.Training.Tests.Handlers;

public class GetUserWorkoutPlansQueryHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly GetUserWorkoutPlansQueryHandler _handler;

    public GetUserWorkoutPlansQueryHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _handler = new GetUserWorkoutPlansQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPlans()
    {
        var plans = new List<WorkoutPlan>
        {
            WorkoutPlan.Create("user-1", "Plan A"),
            WorkoutPlan.Create("user-1", "Plan B")
        };
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(plans);

        var result = await _handler.Handle(
            new GetUserWorkoutPlansQuery("user-1"),
            CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WithNoPlans_ShouldReturnEmpty()
    {
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetUserWorkoutPlansQuery("user-1"),
            CancellationToken.None);

        Assert.Empty(result);
    }
}
