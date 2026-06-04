using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Domain.Services;

namespace Training.Training.Handlers.Plans.Commands;

public class GetCycleDataCommandHandler(
    IWorkoutPlanRepository planRepository,
    IExerciseRepository exerciseRepository,
    IOneRmRepository oneRmRepository,
    ICycleCalculator calculator) : IRequestHandler<GetCycleDataCommand, List<CycleDataResult>>
{
    public async Task<List<CycleDataResult>> Handle(GetCycleDataCommand request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
        var userExercises = await exerciseRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var builtInExercises = await exerciseRepository.GetBuiltInAsync(cancellationToken);
        var oneRms = await oneRmRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var oneRmMap = oneRms.ToDictionary(o => o.ExerciseId, o => o.Value);
        var exercises = userExercises.Concat(builtInExercises)
            .Select(e => oneRmMap.TryGetValue(e.Id, out var customOneRm) && customOneRm > 0
                ? Exercise.Hydrate(e.Id, e.Name, customOneRm, e.MuscleGroup, e.UserId, e.IsBuiltIn)
                : e)
            .ToList();

        return calculator.Calculate(plan, exercises, plan.CycleNumber, plan.ProgressCounter);
    }
}
