using ProjectTeam01.domain.Characters;

namespace ProjectTeam01.domain.Effects
{
    internal class ActiveEffectManager(List<ActiveEffect> activeEffects, Hero hero)
    {
        public List<ActiveEffect> ActiveEffects { get; } = activeEffects;
        private Hero Hero { get; set; } = hero;

        public void AddActiveEffect(ActiveEffect effect)
        {
            ActiveEffects.Add(effect);
            ApplyStateEffect(effect);
        }

        // для эффектов силы, ловкости, здоровья
        public int ApplyStatEffect(ActiveEffect effect)
        {
            switch (effect.Type)
            {
                case EffectTypeEnum.BuffStrength:
                    return effect.Value; // возвращаем значение эффекта
                case EffectTypeEnum.BuffAgility:
                    return effect.Value;
                case EffectTypeEnum.BuffMaxHp:
                    return effect.Value;
                default:
                    return 0;
            }

        }

        public int GetTotalStatBonus(EffectTypeEnum statType)
        {
            return ActiveEffects
                .Where(e => e.Type == statType)
                .Sum(e => ApplyStatEffect(e));
        }
        // для эффектов сна и других состояний
        private void ApplyStateEffect(ActiveEffect effect)
        {
            switch (effect.Type)
            {
                case EffectTypeEnum.Sleep:
                    Hero.ApplySleep();
                    break;
                case EffectTypeEnum.BuffMaxHp:
                    Hero.Heal(effect.Value);
                    break;
                default:
                    break;
            }
        }
        private void RemoveStateEffect(ActiveEffect effect)
        {
            switch (effect.Type)
            {
                case EffectTypeEnum.Sleep:
                    Hero.WakeUp(); // пробуждаем героя
                    break;
            }
        }

        public void TickEffects()//обновляем эффекты каждый тик
        {
            int counter = ActiveEffects.Count;
            for (int i = counter - 1; i >= 0; i--)
            {
                ActiveEffect effect = ActiveEffects[i];
                effect.Tick();
                if (effect.IsEffectOver)
                {
                    // Сохраняем тип эффекта для проверки после удаления
                    bool wasBuffMaxHp = effect.Type == EffectTypeEnum.BuffMaxHp;

                    RemoveStateEffect(effect);
                    ActiveEffects.Remove(effect);

                    if (wasBuffMaxHp && Hero.ActualHp <= 0)
                    {
                        Hero.Heal(1 - Hero.ActualHp); // лечим героя на 1 HP
                    }
                }
            }
        }
    }
}
