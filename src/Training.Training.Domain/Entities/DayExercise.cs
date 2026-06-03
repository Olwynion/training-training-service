namespace Training.Training.Domain.Entities;

public class DayExercise
{
    public string Id { get; private set; } = null!;
    public string ExerciseId { get; private set; } = null!;
    public string ExerciseName { get; private set; } = null!;
    public int Sets { get; private set; }
    public int SortOrder { get; private set; }

    private DayExercise() { }

    public static DayExercise Create(string exerciseId, string exerciseName, int sets, int sortOrder)
    {
        return new DayExercise
        {
            Id = Guid.NewGuid().ToString(),
            ExerciseId = exerciseId,
            ExerciseName = exerciseName,
            Sets = sets,
            SortOrder = sortOrder
        };
    }

    public static DayExercise Hydrate(string id, string exerciseId, string exerciseName, int sets, int sortOrder)
    {
        return new DayExercise
        {
            Id = id, ExerciseId = exerciseId, ExerciseName = exerciseName,
            Sets = sets, SortOrder = sortOrder
        };
    }
}
