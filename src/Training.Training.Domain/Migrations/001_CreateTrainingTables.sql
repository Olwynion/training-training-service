-- +goose Up
CREATE TABLE IF NOT EXISTS exercises (
    id TEXT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    default_one_rm DOUBLE PRECISION NOT NULL,
    muscle_group INTEGER NOT NULL,
    user_id TEXT NOT NULL,
    is_built_in BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS workout_plans (
    id TEXT PRIMARY KEY,
    user_id TEXT NOT NULL,
    name VARCHAR(255) NOT NULL,
    cycle_number INTEGER NOT NULL DEFAULT 1,
    progress_counter INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS plan_days (
    id TEXT PRIMARY KEY,
    workout_plan_id TEXT NOT NULL REFERENCES workout_plans(id) ON DELETE CASCADE,
    day_name VARCHAR(255) NOT NULL,
    focus_group INTEGER NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS day_exercises (
    id TEXT PRIMARY KEY,
    plan_day_id TEXT NOT NULL REFERENCES plan_days(id) ON DELETE CASCADE,
    exercise_id TEXT NOT NULL,
    exercise_name VARCHAR(255) NOT NULL,
    sets INTEGER NOT NULL DEFAULT 0,
    sort_order INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_exercises_user_id ON exercises (user_id);
CREATE INDEX IF NOT EXISTS idx_workout_plans_user_id ON workout_plans (user_id);
CREATE INDEX IF NOT EXISTS idx_plan_days_workout_plan_id ON plan_days (workout_plan_id);
CREATE INDEX IF NOT EXISTS idx_day_exercises_plan_day_id ON day_exercises (plan_day_id);

-- +goose Down
DROP TABLE IF EXISTS day_exercises;
DROP TABLE IF EXISTS plan_days;
DROP TABLE IF EXISTS workout_plans;
DROP TABLE IF EXISTS exercises;
