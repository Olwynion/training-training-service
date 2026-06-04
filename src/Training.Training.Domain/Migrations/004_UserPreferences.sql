-- +goose Up
CREATE TABLE IF NOT EXISTS user_preferences (
    user_id TEXT PRIMARY KEY,
    days_per_week INTEGER NOT NULL DEFAULT 3,
    program_type TEXT NOT NULL DEFAULT 'fullbody',
    focus_group INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS one_rms (
    id BIGSERIAL PRIMARY KEY,
    user_id TEXT NOT NULL,
    exercise_id BIGINT NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
    one_rm DOUBLE PRECISION NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UNIQUE(user_id, exercise_id)
);

CREATE INDEX IF NOT EXISTS idx_one_rms_user_id ON one_rms (user_id);

-- +goose Down
DROP TABLE IF EXISTS one_rms;
DROP TABLE IF EXISTS user_preferences;
