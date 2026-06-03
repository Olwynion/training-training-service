using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Infrastructure.Services;

namespace Training.Training.Tests.Services;

public class CycleCalculatorTests
{
    private readonly CycleCalculator _calculator = new();

    [Fact]
    public void Calculate_ShouldReturnThreeDays()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        var exercises = new List<Exercise>
        {
            Exercise.Create("Жим лежа", 100, MuscleGroup.Chest, "user-1"),
            Exercise.Create("Тяга штанги", 80, MuscleGroup.Back, "user-1"),
        };

        var result = _calculator.Calculate(plan, exercises, 1, 0);

        Assert.Equal(3, result.Count);
        Assert.All(result, day => Assert.NotEmpty(day.Sets));
    }

    [Fact]
    public void Calculate_ShouldHaveCorrectDayLabels()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");

        var result = _calculator.Calculate(plan, [], 1, 0);

        Assert.Contains(result, d => d.DayLabel.Contains("ПН"));
        Assert.Contains(result, d => d.DayLabel.Contains("СР"));
        Assert.Contains(result, d => d.DayLabel.Contains("ПТ"));
    }

    [Fact]
    public void Calculate_ShouldSetFocusGroups()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");

        var result = _calculator.Calculate(plan, [], 1, 0);

        Assert.Equal(MuscleGroup.Chest, result[0].FocusGroup);
        Assert.Equal(MuscleGroup.Back, result[1].FocusGroup);
        Assert.Equal(MuscleGroup.Legs, result[2].FocusGroup);
    }

    [Fact]
    public void Calculate_ShouldApplyProgressMultiplier()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        var exercises = new List<Exercise>
        {
            Exercise.Create("Жим гантелей 30°", 100, MuscleGroup.Chest, "user-1")
        };

        var result = _calculator.Calculate(plan, exercises, 1, 1);

        var chestDay = result[0];
        var firstSet = chestDay.Sets.First();
        Assert.True(firstSet.OneRm > 100);
    }

    [Fact]
    public void Calculate_WithNoProgress_ShouldUseBaseWeights()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        var exercises = new List<Exercise>
        {
            Exercise.Create("Жим гантелей 30°", 100, MuscleGroup.Chest, "user-1")
        };

        var result = _calculator.Calculate(plan, exercises, 1, 0);

        var firstSet = result[0].Sets.First();
        Assert.Equal(100, firstSet.OneRm, 0.1);
    }

    [Fact]
    public void Calculate_ShouldMarkFocusExercises()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        var exercises = new List<Exercise>
        {
            Exercise.Create("Жим лежа", 100, MuscleGroup.Chest, "user-1"),
            Exercise.Create("Тяга", 80, MuscleGroup.Back, "user-1"),
        };

        var result = _calculator.Calculate(plan, exercises, 1, 0);

        var chestDay = result[0];
        Assert.Contains(chestDay.Sets, s => s.IsFocus);
        Assert.Contains(chestDay.Sets, s => !s.IsFocus);
    }
}
