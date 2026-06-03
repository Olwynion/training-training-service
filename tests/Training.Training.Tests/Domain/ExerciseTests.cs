using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;

namespace Training.Training.Tests.Domain;

public class ExerciseTests
{
    [Fact]
    public void Create_ShouldGenerateIdAndSetProperties()
    {
        var exercise = Exercise.Create("Bench Press", 100.0, MuscleGroup.Chest, "user-1");

        Assert.Equal(0, exercise.Id);
        Assert.Equal("Bench Press", exercise.Name);
        Assert.Equal(100.0, exercise.DefaultOneRm);
        Assert.Equal(MuscleGroup.Chest, exercise.MuscleGroup);
        Assert.Equal("user-1", exercise.UserId);
        Assert.False(exercise.IsBuiltIn);
    }

    [Fact]
    public void Hydrate_ShouldSetAllProperties()
    {
        var exercise = Exercise.Hydrate(1, "Squat", 150.0, MuscleGroup.Legs, "user-1", true);

        Assert.Equal(1, exercise.Id);
        Assert.Equal("Squat", exercise.Name);
        Assert.Equal(150.0, exercise.DefaultOneRm);
        Assert.Equal(MuscleGroup.Legs, exercise.MuscleGroup);
        Assert.Equal("user-1", exercise.UserId);
        Assert.True(exercise.IsBuiltIn);
    }

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var exercise = Exercise.Create("Bench Press", 100.0, MuscleGroup.Chest, "user-1");
        exercise.Update("Incline Bench Press", 90.0, MuscleGroup.Chest);

        Assert.Equal("Incline Bench Press", exercise.Name);
        Assert.Equal(90.0, exercise.DefaultOneRm);
        Assert.Equal(MuscleGroup.Chest, exercise.MuscleGroup);
    }

    [Fact]
    public void Create_ShouldSetDefaultId()
    {
        var ex1 = Exercise.Create("A", 100, MuscleGroup.Chest, "u1");
        var ex2 = Exercise.Create("B", 100, MuscleGroup.Back, "u1");

        Assert.Equal(0, ex1.Id);
        Assert.Equal(0, ex2.Id);
    }
}
