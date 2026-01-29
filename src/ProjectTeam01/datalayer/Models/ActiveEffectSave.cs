using ProjectTeam01.domain.Effects;

namespace ProjectTeam01.datalayer.Models
{
    internal class ActiveEffectSave
    {
        public EffectTypeEnum Type { get; set; }
        public int RemainingTicks { get; set; }
        public int Value { get; set; }
    }
}
