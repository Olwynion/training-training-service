using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.Plans.Commands.UpdatePlanDays;

namespace Training.Training.Tests.Handlers;

public class UpdatePlanDaysCommandHandlerTests
{
    private readonly Mock<IWorkoutPlanRepository> _planRepo;
    private readonly Mock<IExerciseRepository> _exerciseRepo;
    private readonly UpdatePlanDaysCommandHandler _handler;

    public UpdatePlanDaysCommandHandlerTests()
    {
        _planRepo = new Mock<IWorkoutPlanRepository>();
        _exerciseRepo = new Mock<IExerciseRepository>();
        _handler = new UpdatePlanDaysCommandHandler(_planRepo.Object, _exerciseRepo.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePlanDays()
    {
        var plan = WorkoutPlan.Hydrate(1, "user-1", "Test Plan", 1, 0, DateTime.UtcNow);
        _planRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(plan);

        var command = new UpdatePlanDaysCommand(1, "user-1",
        [
            new DayInfo(10, "Day 1", MuscleGroup.Chest, 1,
            [
                new ExerciseInfo(20, 5, 3, 1, "Bench Press")
            ])
        ]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Single(result.Days);
        Assert.Equal("Day 1", result.Days[0].DayName);
        Assert.Single(result.Days[0].Exercises);
        Assert.Equal("Bench Press", result.Days[0].Exercises[0].ExerciseName);
        _planRepo.Verify(r => r.UpdateAsync(plan, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ShouldThrow()
    {
        _planRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((WorkoutPlan?)null);

        var command = new UpdatePlanDaysCommand(99, "user-1", []);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnPlan_ShouldThrow()
    {
        var plan = WorkoutPlan.Hydrate(1, "other-user", "Test Plan", 1, 0, DateTime.UtcNow);
        _planRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(plan);

        var command = new UpdatePlanDaysCommand(1, "user-1", []);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithExerciseNameLookup_ShouldFetchName()
    {
        var plan = WorkoutPlan.Hydrate(1, "user-1", "Test Plan", 1, 0, DateTime.UtcNow);
        _planRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(plan);

        var exercise = Exercise.Create("Squat", 150, MuscleGroup.Legs, "built-in");
        _exerciseRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(exercise);

        var command = new UpdatePlanDaysCommand(1, "user-1",
        [
            new DayInfo(10, "Leg Day", MuscleGroup.Legs, 1,
            [
                new ExerciseInfo(20, 5, 4, 1, "")
            ])
        ]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Squat", result.Days[0].Exercises[0].ExerciseName);
    }

    [Fact]
    public async Task Handle_WithUnknownExerciseId_ShouldUseUnknownName()
    {
        var plan = WorkoutPlan.Hydrate(1, "user-1", "Test Plan", 1, 0, DateTime.UtcNow);
        _planRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(plan);
        _exerciseRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Exercise?)null);

        var command = new UpdatePlanDaysCommand(1, "user-1",
        [
            new DayInfo(10, "Day 1", MuscleGroup.Chest, 1,
            [
                new ExerciseInfo(20, 999, 3, 1, "")
            ])
        ]);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Unknown", result.Days[0].Exercises[0].ExerciseName);
    }
}
