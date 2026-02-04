using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items;

namespace ProjectTeam01.datalayer.Models
{
    internal class EnemySave
    {
        public EnemyTypeEnum EnemyType { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int ActualHp { get; set; }
        public bool IsTriggered { get; set; }

        //ogre
        public bool? OgreCooldown { get; set; }
        //ghost
        public bool? IsInvisible { get; set; }
        //vampire
        public bool? EvadedFirstAttack { get; set; }
        //mimic
        public ItemType? Representation { get; set; }
    }
}
