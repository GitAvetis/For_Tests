using ProjectTeam01.domain.generation;

namespace ProjectTeam01.domain
{
    /// Общий класс для всех игровых объектов
    internal abstract class GameObject : IGameObject
    {
        public Position Position { get; set; }

        protected GameObject(int x, int y)
        {
            Position = new Position(x, y);
        }

        protected GameObject(Position position)
        {
            Position = position;
        }

        public virtual void MoveTo(int x, int y)
        {
            Position = new Position(x, y);
        }

        public virtual void MoveTo(Position position)
        {
            Position = position;
        }
    }
}
