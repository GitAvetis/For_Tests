using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain
{
    /// Интерфейс для игровых объектов, которые могут находиться на карте
    public interface IGameObject
    {
        Position Position { get; set; }
        void MoveTo(int x, int y);
        void MoveTo(Position position);
    }
}
