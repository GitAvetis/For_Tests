using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.datalayer.Mappers;

internal static class RoomMapper
{
    public static RoomSave ToSave(Room room)
    {
        return new RoomSave
        {
            Sector = room.Sector,
            TopLeft = PositionMapper.ToSave(room.TopLeft),
            BottomRight = PositionMapper.ToSave(room.BottomRight),
            Doors = room.Doors.Where(d => d.X != 0 || d.Y != 0)
                .Select(PositionMapper.ToSave)
                .ToList(),
            IsStartRoom = room.IsStartRoom,
            IsEndRoom = room.IsEndRoom
        };
    }

    public static Room FromSave(RoomSave save)
    {
        var room = new Room
        {
            Sector = save.Sector,
            TopLeft = PositionMapper.FromSave(save.TopLeft),
            BottomRight = PositionMapper.FromSave(save.BottomRight),
            IsStartRoom = save.IsStartRoom,
            IsEndRoom = save.IsEndRoom
        };

        // Восстанавливаем двери
        room.Doors = new Position[4];
        for (int i = 0; i < save.Doors.Count && i < 4; i++)
        {
            room.Doors[i] = PositionMapper.FromSave(save.Doors[i]);
        }

        // Connections не сохраняем, так как они будут восстановлены при загрузке уровня
        room.Connections = new Room[4];

        return room;
    }
}

