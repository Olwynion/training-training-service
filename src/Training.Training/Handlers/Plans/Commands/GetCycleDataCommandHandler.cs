using MediatR;
using Training.Training.Domain.Repositories;

using Training.Training.Domain.Services;

namespace Training.Training.Handlers.Plans.Commands;

public class GetCycleDataCommandHandler(
    IWorkoutPlanRepository planRepository,
    IExerciseRepository exerciseRepository,
    ICycleCalculator calculator) : IRequestHandler<GetCycleDataCommand, List<CycleDataResult>>
{
    public async Task<List<CycleDataResult>> Handle(GetCycleDataCommand request, CancellationToken cancellationToken)
    {
        var plan = await planRepository.GetByIdAsync(request.PlanId, cancellationToken)
            ?? throw new KeyNotFoundException("Plan not found");
        var exercises = await exerciseRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return calculator.Calculate(plan, exercises, plan.CycleNumber, plan.ProgressCounter);
    }
}
