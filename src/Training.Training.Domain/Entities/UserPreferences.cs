using Training.Training.Domain.Enums;

namespace Training.Training.Domain.Entities;

public class UserPreferences
{
    public string UserId { get; private set; } = null!;
    public int DaysPerWeek { get; private set; }
    public string ProgramType { get; private set; } = null!;
    public MuscleGroup FocusGroup { get; private set; }

    private UserPreferences() { }

    public static UserPreferences Create(string userId, int daysPerWeek, string programType, MuscleGroup focusGroup)
    {
        return new UserPreferences
        {
            UserId = userId,
            DaysPerWeek = daysPerWeek,
            ProgramType = programType,
            FocusGroup = focusGroup
        };
    }

    public static UserPreferences Hydrate(string userId, int daysPerWeek, string programType, MuscleGroup focusGroup)
    {
        return new UserPreferences
        {
            UserId = userId,
            DaysPerWeek = daysPerWeek,
            ProgramType = programType,
            FocusGroup = focusGroup
        };
    }
}
