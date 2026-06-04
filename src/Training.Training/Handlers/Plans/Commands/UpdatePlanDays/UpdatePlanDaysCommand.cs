using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Enums;

namespace Training.Training.Handlers.Plans.Commands.UpdatePlanDays;

public record UpdatePlanDaysCommand(long PlanId, string UserId, List<DayInfo> Days) : IRequest<WorkoutPlan>;

public record DayInfo(long Id, string DayName, MuscleGroup FocusGroup, int SortOrder, List<ExerciseInfo> Exercises);

public record ExerciseInfo(long Id, long ExerciseId, int Sets, int SortOrder);
