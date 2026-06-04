using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Commands;

namespace Training.Training.Tests.Handlers;

public class UpdateWorkoutPlanCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly UpdateWorkoutPlanCommandHandler _handler;

    public UpdateWorkoutPlanCommandHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _repo.Setup(r => r.UpdateAsync(It.IsAny<WorkoutPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _handler = new UpdateWorkoutPlanCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAndReturnPlan()
    {
        var existing = WorkoutPlan.Create("user-1", "Old Name");
        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var updated = WorkoutPlan.Hydrate(existing.Id, "user-1", "New Name", 2, 1, DateTime.UtcNow);

        var result = await _handler.Handle(
            new UpdateWorkoutPlanCommand(updated, "user-1"),
            CancellationToken.None);

        Assert.Equal("New Name", result.Name);
        _repo.Verify(r => r.UpdateAsync(updated, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldThrow()
    {
        var plan = WorkoutPlan.Hydrate(999, "user-1", "Plan", 1, 0, DateTime.UtcNow);
        _repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutPlan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new UpdateWorkoutPlanCommand(plan, "user-1"),
                CancellationToken.None));
    }
}
