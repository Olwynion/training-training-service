using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Commands;

namespace Training.Training.Tests.Handlers;

public class CreateWorkoutPlanCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _repo;
    private readonly CreateWorkoutPlanCommandHandler _handler;

    public CreateWorkoutPlanCommandHandlerTests()
    {
        _repo = new Mock<IWorkoutPlanRepository>();
        _handler = new CreateWorkoutPlanCommandHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateAndReturnPlan()
    {
        var result = await _handler.Handle(
            new CreateWorkoutPlanCommand("user-1", "Push Pull Legs"),
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("user-1", result.UserId);
        Assert.Equal("Push Pull Legs", result.Name);
        Assert.Equal(1, result.CycleNumber);
        Assert.Equal(0, result.ProgressCounter);
        _repo.Verify(r => r.AddAsync(result, It.IsAny<CancellationToken>()), Times.Once);
    }
}
