using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Commands;

namespace Training.Training.Tests.Handlers;

public class DeleteWorkoutPlanCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly DeleteWorkoutPlanCommandHandler _handler;

    public DeleteWorkoutPlanCommandHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _handler = new DeleteWorkoutPlanCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeletePlan()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        _repo.Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        await _handler.Handle(
            new DeleteWorkoutPlanCommand(plan.Id, "user-1"),
            CancellationToken.None);

        _repo.Verify(r => r.Delete(plan), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        _repo.Setup(r => r.GetByIdAsync("missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutPlan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new DeleteWorkoutPlanCommand("missing", "user-1"),
                CancellationToken.None));
    }
}
