using MediatR;
using Training.Training.Domain.Entities;

namespace Training.Training.Handlers.OneRms.Commands;

public record SaveOneRmsCommand(string UserId, List<(long ExerciseId, double Value)> Entries) : IRequest;
