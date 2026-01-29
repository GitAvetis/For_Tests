using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.datalayer.Mappers;

internal static class PositionMapper
{
    public static PositionSave ToSave(Position position)
    {
        return new PositionSave
        {
            X = position.X,
            Y = position.Y
        };
    }

    public static Position FromSave(PositionSave save)
    {
        return new Position(save.X, save.Y);
    }
}

