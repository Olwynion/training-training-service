using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.Plans.Commands.UpdatePlanDays;

public class UpdatePlanDaysCommandHandler(
    IWorkoutPlanRepository planRepository,
    IExerciseRepository exerciseRepository)
    : IRequestHandler<UpdatePlanDaysCommand, WorkoutPlan>
{
    public async Task<WorkoutPlan> Handle(UpdatePlanDaysCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[UpdatePlanDays] PlanId={request.PlanId}, Days count={request.Days.Count}");

        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException($"Plan {request.PlanId} not found");

        if (plan.UserId != request.UserId)
            throw new UnauthorizedAccessException("User does not own this plan");

        plan.Days.Clear();

        foreach (var dayInfo in request.Days)
        {
            Console.WriteLine($"[UpdatePlanDays] Day: '{dayInfo.DayName}', focus={dayInfo.FocusGroup}, exercises={dayInfo.Exercises.Count}");

            var planDay = PlanDay.Create(dayInfo.DayName, dayInfo.FocusGroup, dayInfo.SortOrder);
            planDay.SetId(dayInfo.Id);

            foreach (var exInfo in dayInfo.Exercises)
            {
                Console.WriteLine($"[UpdatePlanDays]   Ex: id={exInfo.ExerciseId}, sets={exInfo.Sets}, name='{exInfo.ExerciseName}'");

                if (string.IsNullOrWhiteSpace(exInfo.ExerciseName) && exInfo.ExerciseId > 0)
                {
                    var exercise = await exerciseRepository.GetByIdAsync(exInfo.ExerciseId, cancellationToken);
                    Console.WriteLine($"[UpdatePlanDays]   Found exercise: {(exercise?.Name ?? "null")}");
                }

                var exerciseName = !string.IsNullOrWhiteSpace(exInfo.ExerciseName)
                    ? exInfo.ExerciseName
                    : exInfo.ExerciseId > 0
                        ? (await exerciseRepository.GetByIdAsync(exInfo.ExerciseId, cancellationToken))?.Name ?? "Unknown"
                        : "Unknown";

                var dayEx = DayExercise.Create(exInfo.ExerciseId, exerciseName, exInfo.Sets, exInfo.SortOrder);
                dayEx.SetId(exInfo.Id);
                planDay.AddExercise(dayEx);
            }

            plan.AddDay(planDay);
        }

        Console.WriteLine($"[UpdatePlanDays] Saving plan with {plan.Days.Count} days...");
        foreach (var d in plan.Days)
            Console.WriteLine($"[UpdatePlanDays]   Day '{d.DayName}' has {d.Exercises.Count} exercises");

        await planRepository.UpdateAsync(plan, cancellationToken);
        Console.WriteLine($"[UpdatePlanDays] Save OK");

        return plan;
    }
}
