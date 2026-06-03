using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Commands;

namespace Training.Training.Tests.Handlers;

public class SetCycleCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly SetCycleCommandHandler _handler;

    public SetCycleCommandHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _handler = new SetCycleCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldSetCycle()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        _repo.Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        await _handler.Handle(
            new SetCycleCommand(plan.Id, 3, "user-1"),
            CancellationToken.None);

        Assert.Equal(3, plan.CycleNumber);
        _repo.Verify(r => r.UpdateCycleAsync(plan.Id, 3, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        _repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutPlan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new SetCycleCommand(999, 1, "user-1"),
                CancellationToken.None));
    }
}
