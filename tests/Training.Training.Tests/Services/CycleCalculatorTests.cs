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
        var (plan, exercises) = MakePlan([
            Exercise.Create("Жим лежа", 100, MuscleGroup.Chest, "user-1"),
            Exercise.Create("Тяга штанги", 80, MuscleGroup.Back, "user-1"),
        ]);

        var result = _calculator.Calculate(plan, exercises, 1, 0);

        Assert.Equal(3, result.Count);
        Assert.All(result, day => Assert.NotEmpty(day.Sets));
    }

    [Fact]
    public void Calculate_ShouldHaveCorrectDayLabels()
    {
        var (plan, _) = MakePlan([]);

        var result = _calculator.Calculate(plan, [], 1, 0);

        Assert.Contains(result, d => d.DayLabel.Contains("Понедельник"));
        Assert.Contains(result, d => d.DayLabel.Contains("Среда"));
        Assert.Contains(result, d => d.DayLabel.Contains("Пятница"));
    }

    [Fact]
    public void Calculate_ShouldSetFocusGroups()
    {
        var (plan, _) = MakePlan([]);

        var result = _calculator.Calculate(plan, [], 1, 0);

        Assert.Equal(MuscleGroup.Chest, result[0].FocusGroup);
        Assert.Equal(MuscleGroup.Back, result[1].FocusGroup);
        Assert.Equal(MuscleGroup.Legs, result[2].FocusGroup);
    }

    [Fact]
    public void Calculate_ShouldApplyProgressMultiplier()
    {
        var (plan, exercises) = MakePlan([
            Exercise.Create("Жим гантелей 30°", 100, MuscleGroup.Chest, "user-1")
        ]);

        var result = _calculator.Calculate(plan, exercises, 1, 1);

        var chestDay = result[0];
        var firstSet = chestDay.Sets.First();
        Assert.True(firstSet.OneRm > 100);
    }

    [Fact]
    public void Calculate_WithNoProgress_ShouldUseBaseWeights()
    {
        var (plan, exercises) = MakePlan([
            Exercise.Create("Жим гантелей 30°", 100, MuscleGroup.Chest, "user-1")
        ]);

        var result = _calculator.Calculate(plan, exercises, 1, 0);

        var firstSet = result[0].Sets.First();
        Assert.Equal(100, firstSet.OneRm, 0.1);
    }

    [Fact]
    public void Calculate_ShouldMarkFocusExercises()
    {
        var exercises = new List<Exercise>
        {
            Exercise.Hydrate(1, "Жим лежа", 100, MuscleGroup.Chest, "user-1", false),
            Exercise.Hydrate(2, "Тяга", 80, MuscleGroup.Back, "user-1", false),
        };
        var plan = WorkoutPlan.Create("user-1", "Фулбоди");
        plan.AddDay(PlanDay.Create("Понедельник", MuscleGroup.Chest, 0));
        plan.Days[0].AddExercise(DayExercise.Create(exercises[0].Id, exercises[0].Name, 3, 0));
        plan.Days[0].AddExercise(DayExercise.Create(exercises[1].Id, exercises[1].Name, 3, 1));

        var result = _calculator.Calculate(plan, exercises, 1, 0);

        var chestDay = result[0];
        Assert.Contains(chestDay.Sets, s => s.IsFocus);
        Assert.Contains(chestDay.Sets, s => !s.IsFocus);
    }

    private static (WorkoutPlan plan, List<Exercise> exercises) MakePlan(List<Exercise> src)
    {
        var exId = 1L;
        var exercises = src
            .Select(e => Exercise.Hydrate(exId++, e.Name, e.DefaultOneRm, e.MuscleGroup, e.UserId, false))
            .ToList();

        var chestExs = exercises
            .Where(e => e.MuscleGroup == MuscleGroup.Chest)
            .Select((e, i) => DayExercise.Create(e.Id, e.Name, 3, i))
            .ToList();
        if (chestExs.Count == 0)
            chestExs.Add(DayExercise.Create(1, "Жим лежа", 3, 0));

        var backExs = exercises
            .Where(e => e.MuscleGroup == MuscleGroup.Back)
            .Select((e, i) => DayExercise.Create(e.Id, e.Name, 3, i))
            .ToList();
        if (backExs.Count == 0)
            backExs.Add(DayExercise.Create(2, "Тяга", 3, 0));

        var legsExs = exercises
            .Where(e => e.MuscleGroup == MuscleGroup.Legs)
            .Select((e, i) => DayExercise.Create(e.Id, e.Name, 3, i))
            .ToList();
        if (legsExs.Count == 0)
            legsExs.Add(DayExercise.Create(3, "Присед", 3, 0));

        var plan = WorkoutPlan.Create("user-1", "Фулбоди");
        plan.AddDay(PlanDay.Create("Понедельник", MuscleGroup.Chest, 0));
        plan.AddDay(PlanDay.Create("Среда", MuscleGroup.Back, 1));
        plan.AddDay(PlanDay.Create("Пятница", MuscleGroup.Legs, 2));
        foreach (var ex in chestExs) plan.Days[0].AddExercise(ex);
        foreach (var ex in backExs) plan.Days[1].AddExercise(ex);
        foreach (var ex in legsExs) plan.Days[2].AddExercise(ex);
        return (plan, exercises);
    }
}
