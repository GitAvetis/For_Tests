namespace ProjectTeam01.domain.Effects
{
    public enum EffectTypeEnum
    {
        BuffStrength,
        BuffAgility,
        BuffMaxHp,
        Sleep
    }

    internal class ActiveEffect
    {
        public ActiveEffect(EffectTypeEnum effectType)
        {
            Type = effectType;
            RemainingTicks = DurationVal(effectType);
            Value = ValuesByType(effectType);
        }

        public ActiveEffect(EffectTypeEnum effectType, int value, int durationTicks)
        {
            Type = effectType;
            Value = value;
            RemainingTicks = durationTicks;
        }

        // Конструктор для загрузки сохранений (порядок параметров: effectType, remainingTicks, value)
        public static ActiveEffect FromSave(EffectTypeEnum effectType, int remainingTicks, int value)
        {
            return new ActiveEffect(effectType, value, remainingTicks);
        }

        public EffectTypeEnum Type { get; }

        public int Value { get; }

        public int RemainingTicks { get; private set; }//оставшиеся тики действия эффекта

        public static int DurationVal(EffectTypeEnum effect)
        {
            if (effect == EffectTypeEnum.Sleep)
                return 1;
            else return 5;
        }

        public static int ValuesByType(EffectTypeEnum effect)
        {
            switch (effect)
            {
                case EffectTypeEnum.BuffStrength:
                    return 5;
                case EffectTypeEnum.BuffAgility:
                    return 5;
                case EffectTypeEnum.BuffMaxHp:
                    return 10;
                default:
                    return 0;
            }
        }
        public void Tick()
        {
            if (!IsEffectOver)
            {
                RemainingTicks -= 1;
                if (RemainingTicks <= 0)
                    IsEffectOver = true;
            }
        }
        public bool IsEffectOver { get; private set; }


    }
}
