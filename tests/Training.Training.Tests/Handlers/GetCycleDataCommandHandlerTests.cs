using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Domain.Services;
using Training.Training.Handlers.Plans.Commands;

namespace Training.Training.Tests.Handlers;

public class GetCycleDataCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _planRepo;
    private readonly Mock<IExerciseRepository> _exerciseRepo;
    private readonly Mock<IOneRmRepository> _oneRmRepo;
    private readonly Mock<ICycleCalculator> _calculator;
    private readonly GetCycleDataCommandHandler _handler;

    public GetCycleDataCommandHandlerTests()
    {
        _planRepo = new Mock<IWorkoutPlanRepository>();
        _exerciseRepo = new Mock<IExerciseRepository>();
        _oneRmRepo = new Mock<IOneRmRepository>();
        _calculator = new Mock<ICycleCalculator>();
        _handler = new GetCycleDataCommandHandler(_planRepo.Object, _exerciseRepo.Object, _oneRmRepo.Object, _calculator.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCycleData()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        _planRepo.Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _exerciseRepo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _exerciseRepo.Setup(r => r.GetBuiltInAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _oneRmRepo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var expected = new List<CycleDataResult>
        {
            new() { DayLabel = "Day 1", FocusGroup = global::Training.Training.Domain.Enums.MuscleGroup.Chest, Sets = [] }
        };
        _calculator.Setup(c => c.Calculate(plan, It.IsAny<IReadOnlyList<Exercise>>(), plan.CycleNumber, plan.ProgressCounter))
            .Returns(expected);

        var result = await _handler.Handle(
            new GetCycleDataCommand(plan.Id, "user-1"),
            CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Day 1", result[0].DayLabel);
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ShouldThrow()
    {
        _planRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutPlan?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(
                new GetCycleDataCommand(999, "user-1"),
                CancellationToken.None));
    }
}
