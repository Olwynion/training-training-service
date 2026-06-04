namespace Training.Training.Domain.Entities;

public class DayExercise
{
    public long Id { get; private set; }
    public long ExerciseId { get; private set; }
    public string ExerciseName { get; private set; } = null!;
    public int Sets { get; private set; }
    public int SortOrder { get; private set; }

    private DayExercise() { }

    public static DayExercise Create(long exerciseId, string exerciseName, int sets, int sortOrder)
    {
        return new DayExercise
        {
            ExerciseId = exerciseId,
            ExerciseName = exerciseName,
            Sets = sets,
            SortOrder = sortOrder
        };
    }

    public static DayExercise Hydrate(long id, long exerciseId, string exerciseName, int sets, int sortOrder)
    {
        return new DayExercise
        {
            Id = id, ExerciseId = exerciseId, ExerciseName = exerciseName,
            Sets = sets, SortOrder = sortOrder
        };
    }

    public void SetId(long id) => Id = id;
}
