using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;

namespace Training.Training.Tests.Domain;

public class WorkoutPlanTests
{
    [Fact]
    public void Create_ShouldGenerateIdAndSetProperties()
    {
        var plan = WorkoutPlan.Create("user-1", "Push Pull Legs");

        Assert.NotNull(plan.Id);
        Assert.NotEmpty(plan.Id);
        Assert.Equal("user-1", plan.UserId);
        Assert.Equal("Push Pull Legs", plan.Name);
        Assert.Equal(1, plan.CycleNumber);
        Assert.Equal(0, plan.ProgressCounter);
        Assert.Empty(plan.Days);
    }

    [Fact]
    public void Hydrate_ShouldSetAllProperties()
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var plan = WorkoutPlan.Hydrate("plan-1", "user-1", "My Plan", 3, 2, createdAt);

        Assert.Equal("plan-1", plan.Id);
        Assert.Equal("user-1", plan.UserId);
        Assert.Equal("My Plan", plan.Name);
        Assert.Equal(3, plan.CycleNumber);
        Assert.Equal(2, plan.ProgressCounter);
        Assert.Equal(createdAt, plan.CreatedAt);
    }

    [Fact]
    public void AddDay_ShouldAddPlanDay()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        var day = PlanDay.Hydrate("day-1", "Chest Day", MuscleGroup.Chest, 1);

        plan.AddDay(day);

        Assert.Single(plan.Days);
        Assert.Contains(day, plan.Days);
    }

    [Fact]
    public void AddDay_ShouldLinkPlanDayToPlan()
    {
        var plan = WorkoutPlan.Create("user-1", "Plan");
        var day = PlanDay.Hydrate("day-1", "Chest Day", MuscleGroup.Chest, 1);

        plan.AddDay(day);

        Assert.Single(plan.Days);
    }

    [Fact]
    public void PlanDay_Hydrate_ShouldSetProperties()
    {
        var day = PlanDay.Hydrate("day-1", "Leg Day", MuscleGroup.Legs, 2);

        Assert.Equal("day-1", day.Id);
        Assert.Equal("Leg Day", day.DayName);
        Assert.Equal(MuscleGroup.Legs, day.FocusGroup);
        Assert.Equal(2, day.SortOrder);
        Assert.Empty(day.Exercises);
    }

    [Fact]
    public void DayExercise_Hydrate_ShouldSetProperties()
    {
        var exercise = DayExercise.Hydrate("de-1", "ex-1", "Squat", 4, 1);

        Assert.Equal("de-1", exercise.Id);
        Assert.Equal("ex-1", exercise.ExerciseId);
        Assert.Equal("Squat", exercise.ExerciseName);
        Assert.Equal(4, exercise.Sets);
        Assert.Equal(1, exercise.SortOrder);
    }
}
