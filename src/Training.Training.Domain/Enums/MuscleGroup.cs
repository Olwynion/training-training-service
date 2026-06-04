using System.ComponentModel;

namespace Training.Training.Domain.Enums;

public enum MuscleGroup
{
    [Description("Грудь")]
    Chest,
    [Description("Спина")]
    Back,
    [Description("Ноги")]
    Legs,
    [Description("Плечи")]
    Shoulders,
    [Description("Бицепс")]
    Biceps,
    [Description("Трицепс")]
    Triceps,
    [Description("Пресс")]
    Core
}
