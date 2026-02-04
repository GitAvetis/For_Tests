using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.datalayer.Mappers;

internal static class LevelMapper
{
    public static LevelSave ToSave(Level level)
    {
        return new LevelSave
        {
            LevelNumber = level.LevelNumber,
            Rooms = level.Rooms.Select(RoomMapper.ToSave).ToList(),
            Corridors = level.Corridors.Select(CorridorMapper.ToSave).ToList(),
            StartPosition = PositionMapper.ToSave(level.StartPosition),
            ExitPosition = PositionMapper.ToSave(level.ExitPosition)
        };
    }

    public static Level FromSave(LevelSave save)
    {
        var level = new Level
        {
            LevelNumber = save.LevelNumber,
            StartPosition = PositionMapper.FromSave(save.StartPosition),
            ExitPosition = PositionMapper.FromSave(save.ExitPosition)
        };

        // Восстанавливаем комнаты
        level.Rooms = save.Rooms.Select(RoomMapper.FromSave).ToList();

        // Восстанавливаем коридоры
        level.Corridors = save.Corridors.Select(CorridorMapper.FromSave).ToList();

        return level;
    }
}

