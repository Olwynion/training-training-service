namespace Training.Training.Domain.Entities;

public class OneRm
{
    public long Id { get; private set; }
    public string UserId { get; private set; } = null!;
    public long ExerciseId { get; private set; }
    public double Value { get; private set; }

    private OneRm() { }

    public static OneRm Create(string userId, long exerciseId, double value)
    {
        return new OneRm
        {
            UserId = userId,
            ExerciseId = exerciseId,
            Value = value
        };
    }

    public static OneRm Hydrate(long id, string userId, long exerciseId, double value)
    {
        return new OneRm
        {
            Id = id,
            UserId = userId,
            ExerciseId = exerciseId,
            Value = value
        };
    }
}
