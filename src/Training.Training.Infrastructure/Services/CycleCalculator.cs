using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;
using Training.Training.Domain.Services;

namespace Training.Training.Infrastructure.Services;

public class CycleCalculator : ICycleCalculator
{
    public List<CycleDataResult> Calculate(
        WorkoutPlan plan, IReadOnlyList<Exercise> exercises,
        int cycleNumber, int progressCounter)
    {
        var multiplier = 1.0 + progressCounter * 0.10;
        var isLight = IsLightWeek(cycleNumber);

        return plan.Days.Select(day =>
        {
            var focusGroup = day.FocusGroup;
            return new CycleDataResult
            {
                DayLabel = day.DayName,
                FocusGroup = focusGroup,
                Sets = day.Exercises.Select(de =>
                {
                    var exercise = exercises.FirstOrDefault(e =>
                        e.Id == de.ExerciseId);
                    var oneRm = (exercise?.DefaultOneRm ?? 0) * multiplier;
                    var isFocus = exercise?.MuscleGroup == focusGroup;

                    var percentage = GetPercentage(isFocus, isLight, cycleNumber);
                    var reps = CalculateReps(isLight, percentage);
                    var workingWeight = Math.Max(oneRm * percentage, oneRm * 0.6);

                    return new ExerciseSetResult
                    {
                        ExerciseName = de.ExerciseName,
                        OneRm = Math.Round(oneRm, 1),
                        Sets = de.Sets,
                        Percentage = Math.Round(percentage, 2),
                        Reps = reps,
                        WorkingWeight = Math.Round(workingWeight, 1),
                        IsFocus = isFocus
                    };
                }).ToList()
            };
        }).ToList();
    }

    private static bool IsLightWeek(int cycle) => cycle switch
    {
        1 or 3 => true,
        2 or 4 => false,
        _ => true
    };

    private static double GetPercentage(bool isFocus, bool isLight, int cycle) => (isFocus, isLight, cycle) switch
    {
        (true, true, 1) => 0.60,
        (true, true, 2) => 0.65,
        (true, true, 3) => 0.70,
        (true, true, 4) => 0.65,
        (true, false, 1) => 0.70,
        (true, false, 2) => 0.80,
        (true, false, 3) => 0.90,
        (true, false, 4) => Random.Shared.Next(85, 96) / 100.0,
        (false, true, _) => new[] { 0.55, 0.60, 0.65, 0.60 }[cycle - 1]
            + Random.Shared.Next(-5, 6) / 100.0,
        (false, false, _) => new[] { 0.65, 0.75, 0.85, 0.80 }[cycle - 1]
            + Random.Shared.Next(-5, 6) / 100.0,
        _ => throw new ArgumentOutOfRangeException(nameof(cycle)),
    };

    private static int CalculateReps(bool isLight, double percentage)
    {
        if (isLight)
            return Math.Clamp((int)Math.Round(12 - (percentage - 0.60) * 100), 7, 12);
        return Math.Clamp((int)Math.Round(8 - (percentage - 0.70) * 100), 4, 6);
    }
}
