using ProjectTeam01.domain.Combat;

namespace ProjectTeam01.domain.Characters.Behavior
{
    /// Поведение игрока
    /// Инкапсулирует логику атаки игрока
    internal class HeroBehavior : CharacterBehavior
{
    private readonly Hero _hero;
    
    public HeroBehavior(Hero hero) : base(hero)
    {
        _hero = hero;
    }
    
    /// Атака игрока с учетом оружия, эффектов и специальных правил
    public override bool Attack(Character target)
    {
        if (target is not Enemy enemy)
            return false;
            
        bool playerHit = BattleService.HitSuccess(_hero.Agility, enemy.BaseAgility);
        
        if (enemy is Vampire vampire && !vampire.EvadedFirstAttack)
        {
            vampire.EvadedFirstAttack = true;
            playerHit = false;
        }
        
        if (playerHit)
        {
            int damage = PlayerDamageCalculator.CalculateDamage(_hero);
            enemy.TakeDamage(damage);
            return true;
        }
        return false;
    }
    }
}
