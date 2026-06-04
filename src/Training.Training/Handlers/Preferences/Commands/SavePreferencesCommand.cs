using MediatR;
using Training.Training.Domain.Enums;

namespace Training.Training.Handlers.Preferences.Commands;

public record SavePreferencesCommand(string UserId, int DaysPerWeek, string ProgramType, MuscleGroup FocusGroup) : IRequest;
