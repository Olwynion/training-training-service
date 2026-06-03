using Training.Training.Domain.Enums;

namespace Training.Training.Domain.Entities;

public class WorkoutPlan
{
    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int CycleNumber { get; private set; }
    public int ProgressCounter { get; private set; }
    public List<PlanDay> Days { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }

    private WorkoutPlan() { }

    public static WorkoutPlan Create(string userId, string name)
    {
        return new WorkoutPlan
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Name = name,
            CycleNumber = 1,
            ProgressCounter = 0,
            Days = [],
            CreatedAt = DateTime.UtcNow
        };
    }

    public static WorkoutPlan Hydrate(string id, string userId, string name, int cycleNumber, int progressCounter, DateTime createdAt, List<PlanDay>? days = null)
    {
        return new WorkoutPlan
        {
            Id = id, UserId = userId, Name = name,
            CycleNumber = cycleNumber, ProgressCounter = progressCounter,
            CreatedAt = createdAt, Days = days ?? []
        };
    }

    public void SetCycle(int cycleNumber)
    {
        if (cycleNumber < 1 || cycleNumber > 4)
            throw new ArgumentOutOfRangeException(nameof(cycleNumber), "Cycle must be 1-4");
        CycleNumber = cycleNumber;
    }

    public void IncrementProgress()
    {
        ProgressCounter++;
    }

    public void Update(string name)
    {
        Name = name;
    }

    public void AddDay(PlanDay day)
    {
        Days.Add(day);
    }

    public bool IsLightWeek => CycleNumber switch
    {
        1 or 3 => true,
        2 or 4 => false,
        _ => true
    };
}
