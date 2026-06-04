using Grpc.Core;
using MediatR;
using Training.Training.Proto;
using Training.Training.Handlers.Exercises.Commands;
using Training.Training.Handlers.Exercises.Queries;
using Training.Training.Handlers.Exercises.Queries.GetBuiltIn;
using Training.Training.Handlers.OneRms.Commands;
using Training.Training.Handlers.OneRms.Queries;
using Training.Training.Handlers.Plans.Commands;
using Training.Training.Handlers.Plans.Commands.UpdatePlanDays;
using Training.Training.Handlers.Plans.Queries;
using Training.Training.Handlers.Preferences.Commands;
using Training.Training.Handlers.Preferences.Queries;

namespace Training.Training.Grpc;

public class TrainingGrpcService(IMediator mediator) : TrainingService.TrainingServiceBase
{
    public override async Task<CreateExerciseResponse> CreateExercise(CreateExerciseRequest request, ServerCallContext context)
    {
        var exercise = await mediator.Send(
            new CreateExerciseCommand(request.Name, request.DefaultOneRm, (Domain.Enums.MuscleGroup)(int)request.MuscleGroup, request.UserId),
            context.CancellationToken);

        return new CreateExerciseResponse { Exercise = MapExercise(exercise) };
    }

    public override async Task<GetExercisesResponse> GetExercises(GetExercisesRequest request, ServerCallContext context)
    {
        var exercises = await mediator.Send(
            new GetExercisesQuery(request.UserId),
            context.CancellationToken);

        var response = new GetExercisesResponse();
        response.Exercises.AddRange(exercises.Select(MapExercise));
        return response;
    }

    public override async Task<UpdateExerciseResponse> UpdateExercise(UpdateExerciseRequest request, ServerCallContext context)
    {
        var exercise = await mediator.Send(
            new UpdateExerciseCommand(request.Id, request.Name, request.DefaultOneRm, (Domain.Enums.MuscleGroup)(int)request.MuscleGroup, request.UserId),
            context.CancellationToken);

        return new UpdateExerciseResponse { Exercise = MapExercise(exercise) };
    }

    public override async Task<DeleteExerciseResponse> DeleteExercise(DeleteExerciseRequest request, ServerCallContext context)
    {
        await mediator.Send(new DeleteExerciseCommand(request.Id, request.UserId), context.CancellationToken);
        return new DeleteExerciseResponse();
    }

    public override async Task<CreateWorkoutPlanResponse> CreateWorkoutPlan(CreateWorkoutPlanRequest request, ServerCallContext context)
    {
        var plan = await mediator.Send(
            new CreateWorkoutPlanCommand(request.UserId, request.Name),
            context.CancellationToken);

        return new CreateWorkoutPlanResponse { Plan = MapWorkoutPlan(plan) };
    }

    public override async Task<GetWorkoutPlanResponse> GetWorkoutPlan(GetWorkoutPlanRequest request, ServerCallContext context)
    {
        var plan = await mediator.Send(
            new GetWorkoutPlanQuery(request.Id, request.UserId),
            context.CancellationToken);

        return new GetWorkoutPlanResponse { Plan = MapWorkoutPlan(plan) };
    }

    public override async Task<GetUserWorkoutPlansResponse> GetUserWorkoutPlans(GetUserWorkoutPlansRequest request, ServerCallContext context)
    {
        var plans = await mediator.Send(
            new GetUserWorkoutPlansQuery(request.UserId),
            context.CancellationToken);

        var response = new GetUserWorkoutPlansResponse();
        response.Plans.AddRange(plans.Select(MapWorkoutPlan));
        return response;
    }

    public override async Task<UpdateWorkoutPlanResponse> UpdateWorkoutPlan(UpdateWorkoutPlanRequest request, ServerCallContext context)
    {
        var plan = await mediator.Send(
            new UpdateWorkoutPlanCommand(MapWorkoutPlanFromProto(request.Plan), request.UserId),
            context.CancellationToken);

        return new UpdateWorkoutPlanResponse { Plan = MapWorkoutPlan(plan) };
    }

    public override async Task<DeleteWorkoutPlanResponse> DeleteWorkoutPlan(DeleteWorkoutPlanRequest request, ServerCallContext context)
    {
        await mediator.Send(new DeleteWorkoutPlanCommand(request.Id, request.UserId), context.CancellationToken);
        return new DeleteWorkoutPlanResponse();
    }

    public override async Task<GetCycleDataResponse> GetCycleData(GetCycleDataRequest request, ServerCallContext context)
    {
        var data = await mediator.Send(
            new GetCycleDataCommand(request.PlanId, request.UserId),
            context.CancellationToken);

        var response = new GetCycleDataResponse();
        response.Data = MapCycleData(data, request.PlanId);
        return response;
    }

    public override async Task<SetCycleResponse> SetCycle(SetCycleRequest request, ServerCallContext context)
    {
        await mediator.Send(
            new SetCycleCommand(request.PlanId, request.CycleNumber, request.UserId),
            context.CancellationToken);

        return new SetCycleResponse();
    }

    public override async Task<IncrementProgressResponse> IncrementProgress(IncrementProgressRequest request, ServerCallContext context)
    {
        await mediator.Send(
            new IncrementProgressCommand(request.PlanId, request.UserId),
            context.CancellationToken);

        return new IncrementProgressResponse();
    }

    public override async Task<SavePreferencesResponse> SavePreferences(SavePreferencesRequest request, ServerCallContext context)
    {
        await mediator.Send(
            new SavePreferencesCommand(
                request.UserId,
                request.DaysPerWeek,
                request.ProgramType,
                (Domain.Enums.MuscleGroup)(int)request.FocusGroup),
            context.CancellationToken);

        return new SavePreferencesResponse();
    }

    public override async Task<GetUserPreferencesResponse> GetUserPreferences(GetUserPreferencesRequest request, ServerCallContext context)
    {
        var preferences = await mediator.Send(
            new GetUserPreferencesQuery(request.UserId),
            context.CancellationToken);

        if (preferences == null)
            return new GetUserPreferencesResponse();

        return new GetUserPreferencesResponse
        {
            Preferences = new UserPreferences
            {
                UserId = preferences.UserId,
                DaysPerWeek = preferences.DaysPerWeek,
                ProgramType = preferences.ProgramType,
                FocusGroup = (MuscleGroup)(int)preferences.FocusGroup
            }
        };
    }

    public override async Task<SaveOneRmsResponse> SaveOneRms(SaveOneRmsRequest request, ServerCallContext context)
    {
        var entries = request.Entries.Select(e => (e.ExerciseId, e.OneRm)).ToList();
        await mediator.Send(new SaveOneRmsCommand(request.UserId, entries), context.CancellationToken);
        return new SaveOneRmsResponse();
    }

    public override async Task<GetBuiltInExercisesResponse> GetBuiltInExercises(GetBuiltInExercisesRequest request, ServerCallContext context)
    {
        var exercises = await mediator.Send(new GetBuiltInExercisesQuery(), context.CancellationToken);

        var response = new GetBuiltInExercisesResponse();
        response.Exercises.AddRange(exercises.Select(MapExercise));
        return response;
    }

    public override async Task<UpdatePlanDaysResponse> UpdatePlanDays(UpdatePlanDaysRequest request, ServerCallContext context)
    {
        var days = request.Days.Select(d => new DayInfo(
            d.Id,
            d.DayName,
            (Domain.Enums.MuscleGroup)(int)d.FocusGroup,
            d.SortOrder,
            d.Exercises.Select(e => new ExerciseInfo(e.Id, e.ExerciseId, e.Sets, e.SortOrder)).ToList()
        )).ToList();

        var plan = await mediator.Send(
            new UpdatePlanDaysCommand(request.PlanId, request.UserId, days),
            context.CancellationToken);

        return new UpdatePlanDaysResponse { Plan = MapWorkoutPlan(plan) };
    }

    private static Exercise MapExercise(Domain.Entities.Exercise ex)
    {
        return new Exercise
        {
            Id = ex.Id,
            Name = ex.Name,
            DefaultOneRm = ex.DefaultOneRm,
            MuscleGroup = (MuscleGroup)(int)ex.MuscleGroup,
            UserId = ex.UserId,
            IsBuiltIn = ex.IsBuiltIn
        };
    }

    private static WorkoutPlan MapWorkoutPlan(Domain.Entities.WorkoutPlan plan)
    {
        var result = new WorkoutPlan
        {
            Id = plan.Id,
            UserId = plan.UserId,
            Name = plan.Name,
            CycleNumber = plan.CycleNumber,
            ProgressCounter = plan.ProgressCounter
        };

        result.Days.AddRange(plan.Days.Select(d =>
        {
            var protoDay = new PlanDay
            {
                Id = d.Id,
                DayName = d.DayName,
                FocusGroup = (MuscleGroup)(int)d.FocusGroup,
                SortOrder = d.SortOrder
            };
            protoDay.Exercises.AddRange(d.Exercises.Select(e => new DayExercise
            {
                Id = e.Id,
                ExerciseId = e.ExerciseId,
                ExerciseName = e.ExerciseName,
                Sets = e.Sets,
                SortOrder = e.SortOrder
            }));
            return protoDay;
        }));

        return result;
    }

    private static Domain.Entities.WorkoutPlan MapWorkoutPlanFromProto(WorkoutPlan proto)
    {
        var plan = Domain.Entities.WorkoutPlan.Hydrate(
            proto.Id, proto.UserId, proto.Name,
            proto.CycleNumber, proto.ProgressCounter, DateTime.UtcNow);

        foreach (var day in proto.Days)
        {
            var planDay = Domain.Entities.PlanDay.Hydrate(
                day.Id, day.DayName,
                (Domain.Enums.MuscleGroup)(int)day.FocusGroup,
                day.SortOrder);
            plan.AddDay(planDay);
        }

        return plan;
    }

    private static CycleData MapCycleData(List<Domain.Services.CycleDataResult> data, long planId)
    {
        var result = new CycleData
        {
            PlanId = planId
        };
        foreach (var day in data)
        {
            var dayData = new DayData
            {
                DayLabel = day.DayLabel,
                FocusGroup = (MuscleGroup)(int)day.FocusGroup
            };

            dayData.Sets.AddRange(day.Sets.Select(s => new ExerciseSet
            {
                ExerciseName = s.ExerciseName,
                OneRm = s.OneRm,
                Sets = s.Sets,
                Percentage = s.Percentage,
                Reps = s.Reps,
                WorkingWeight = s.WorkingWeight,
                IsFocus = s.IsFocus
            }));

            result.Days.Add(dayData);
        }

        return result;
    }
}
