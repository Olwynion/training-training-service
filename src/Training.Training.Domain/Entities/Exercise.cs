using Training.Training.Domain.Enums;

namespace Training.Training.Domain.Entities;

public class Exercise
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public double DefaultOneRm { get; private set; }
    public MuscleGroup MuscleGroup { get; private set; }
    public string UserId { get; private set; } = null!;
    public bool IsBuiltIn { get; private set; }

    private Exercise() { }

    public static Exercise Create(string name, double defaultOneRm, MuscleGroup muscleGroup, string userId)
    {
        return new Exercise
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            DefaultOneRm = defaultOneRm,
            MuscleGroup = muscleGroup,
            UserId = userId,
            IsBuiltIn = false
        };
    }

    public static Exercise Hydrate(string id, string name, double defaultOneRm, MuscleGroup muscleGroup, string userId, bool isBuiltIn)
    {
        return new Exercise
        {
            Id = id, Name = name, DefaultOneRm = defaultOneRm,
            MuscleGroup = muscleGroup, UserId = userId, IsBuiltIn = isBuiltIn
        };
    }

    public void Update(string name, double defaultOneRm, MuscleGroup muscleGroup)
    {
        Name = name;
        DefaultOneRm = defaultOneRm;
        MuscleGroup = muscleGroup;
    }
}
