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
        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException($"Plan {request.PlanId} not found");

        if (plan.UserId != request.UserId)
            throw new UnauthorizedAccessException("User does not own this plan");

        plan.Days.Clear();

        foreach (var dayInfo in request.Days)
        {
            var planDay = PlanDay.Create(dayInfo.DayName, dayInfo.FocusGroup, dayInfo.SortOrder);
            planDay.SetId(dayInfo.Id);

            foreach (var exInfo in dayInfo.Exercises)
            {
                var exercise = await exerciseRepository.GetByIdAsync(exInfo.ExerciseId, cancellationToken);
                var exerciseName = exercise?.Name ?? "Unknown";

                var dayEx = DayExercise.Create(exInfo.ExerciseId, exerciseName, exInfo.Sets, exInfo.SortOrder);
                dayEx.SetId(exInfo.Id);
                planDay.AddExercise(dayEx);
            }

            plan.AddDay(planDay);
        }

        planRepository.Update(plan);
        return plan;
    }
}
