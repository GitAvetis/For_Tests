using ProjectTeam01.domain.Session;

namespace ProjectTeam01.domain
{

    internal class Balance
    {
        private const float MinDifficulty = 0.85f;
        private const float MaxDifficulty = 1.35f;

        public float Calculate(GameStatistics stats)
        {
            float difficulty = 1.0f;

            difficulty += CombatAccuracyFactor(stats);
            difficulty += DamageTakenFactor(stats);
            difficulty += ResourceUsageFactor(stats);

            return ResultCheck(difficulty, MinDifficulty, MaxDifficulty);
        }

        private float CombatAccuracyFactor(GameStatistics stats)
        {
            int totalAttacks = stats.HitsLanded + stats.HitsMissed;
            if (totalAttacks < 10)
                return 0f; // слишком мало данных

            float accuracy = (float)stats.HitsLanded / totalAttacks;

            if (accuracy > 0.75f) return +0.15f;
            if (accuracy < 0.45f) return -0.15f;

            return 0f;
        }

        private float DamageTakenFactor(GameStatistics stats)
        {
            if (stats.EnemiesDefeated == 0)
                return 0f;

            float hitsTakenPerKill =
                (float)stats.HitsTaken / stats.EnemiesDefeated;

            if (hitsTakenPerKill > 3.0f) return -0.15f;
            if (hitsTakenPerKill < 1.0f) return +0.1f;

            return 0f;
        }

        private float ResourceUsageFactor(GameStatistics stats)
        {
            int consumablesUsed =
                stats.FoodConsumed +
                stats.ElixirsConsumed +
                stats.ScrollsConsumed;

            if (consumablesUsed == 0)
                return +0.05f; // вообще не тратит — уверенно идёт

            float perLevel =
                (float)consumablesUsed / stats.MaxLevelReached;

            if (perLevel > 3.0f) return -0.1f;

            return 0f;
        }

        private float ResultCheck(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
