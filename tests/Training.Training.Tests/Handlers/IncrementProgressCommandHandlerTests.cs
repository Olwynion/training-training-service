using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Commands;

namespace Training.Training.Tests.Handlers;

public class IncrementProgressCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly IncrementProgressCommandHandler _handler;

    public IncrementProgressCommandHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _handler = new IncrementProgressCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldIncrementProgress()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        _repo.Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        await _handler.Handle(
            new IncrementProgressCommand(plan.Id, "user-1"),
            CancellationToken.None);

        Assert.Equal(1, plan.ProgressCounter);
        _repo.Verify(r => r.IncrementProgressAsync(plan.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        _repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutPlan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new IncrementProgressCommand(999, "user-1"),
                CancellationToken.None));
    }
}
