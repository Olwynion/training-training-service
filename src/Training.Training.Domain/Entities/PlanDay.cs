using Training.Training.Domain.Enums;

namespace Training.Training.Domain.Entities;

public class PlanDay
{
    public long Id { get; private set; }
    public string DayName { get; private set; } = null!;
    public MuscleGroup FocusGroup { get; private set; }
    public int SortOrder { get; private set; }
    public List<DayExercise> Exercises { get; private set; } = [];

    private PlanDay() { }

    public static PlanDay Create(string dayName, MuscleGroup focusGroup, int sortOrder)
    {
        return new PlanDay
        {
            DayName = dayName,
            FocusGroup = focusGroup,
            SortOrder = sortOrder,
            Exercises = []
        };
    }

    public static PlanDay Hydrate(long id, string dayName, MuscleGroup focusGroup, int sortOrder, List<DayExercise>? exercises = null)
    {
        return new PlanDay
        {
            Id = id, DayName = dayName, FocusGroup = focusGroup,
            SortOrder = sortOrder, Exercises = exercises ?? []
        };
    }

    public void AddExercise(DayExercise exercise)
    {
        Exercises.Add(exercise);
    }

    public void SetId(long id) => Id = id;
}
