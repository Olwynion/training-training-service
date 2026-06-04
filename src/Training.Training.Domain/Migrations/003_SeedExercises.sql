-- +goose Up
INSERT INTO exercises (name, default_one_rm, muscle_group, user_id, is_built_in)
VALUES
    -- Chest (1)
    ('Жим штанги на наклонной скамье', 60, 1, 'built-in', true),
    ('Жим гантелей на горизонтальной скамье', 55, 1, 'built-in', true),
    ('Сведение рук в кроссовере', 30, 1, 'built-in', true),
    -- Back (2)
    ('Тяга штанги в наклоне', 65, 2, 'built-in', true),
    ('Тяга верхнего блока к груди', 55, 2, 'built-in', true),
    ('Тяга гантели к поясу', 35, 2, 'built-in', true),
    -- Legs (3)
    ('Приседания со штангой', 80, 3, 'built-in', true),
    ('Румынская тяга', 70, 3, 'built-in', true),
    ('Жим ногами', 120, 3, 'built-in', true),
    -- Shoulders (4)
    ('Жим гантелей сидя', 30, 4, 'built-in', true),
    ('Махи гантелями в стороны', 12, 4, 'built-in', true),
    ('Тяга штанги к подбородку', 30, 4, 'built-in', true),
    -- Biceps (5)
    ('Сгибание рук со штангой', 25, 5, 'built-in', true),
    ('Молотки', 15, 5, 'built-in', true),
    ('Сгибание рук с гантелями сидя', 14, 5, 'built-in', true),
    -- Triceps (6)
    ('Разгибание рук на блоке', 20, 6, 'built-in', true),
    ('Французский жим лёжа', 20, 6, 'built-in', true),
    ('Разгибание рук с гантелью из-за головы', 12, 6, 'built-in', true),
    -- Core (7)
    ('Скручивания', 0, 7, 'built-in', true),
    ('Подъём ног в висе', 0, 7, 'built-in', true),
    ('Планка', 0, 7, 'built-in', true)
ON CONFLICT DO NOTHING;

-- +goose Down
DELETE FROM exercises WHERE user_id = 'built-in';
