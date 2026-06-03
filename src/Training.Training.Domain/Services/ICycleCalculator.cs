using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;

namespace Training.Training.Domain.Services;

public class CycleDataResult
{
    public required string DayLabel { get; init; }
    public required MuscleGroup FocusGroup { get; init; }
    public required List<ExerciseSetResult> Sets { get; init; }
}

public class ExerciseSetResult
{
    public required string ExerciseName { get; init; }
    public double OneRm { get; init; }
    public int Sets { get; init; }
    public double Percentage { get; init; }
    public int Reps { get; init; }
    public double WorkingWeight { get; init; }
    public bool IsFocus { get; init; }
}

public interface ICycleCalculator
{
    List<CycleDataResult> Calculate(
        WorkoutPlan plan,
        IReadOnlyList<Exercise> exercises,
        int cycleNumber,
        int progressCounter);
}
