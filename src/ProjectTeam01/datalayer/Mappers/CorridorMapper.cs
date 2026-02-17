using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain.generation;

namespace ProjectTeam01.datalayer.Mappers;

internal static class CorridorMapper
{
    public static CorridorSave ToSave(Corridor corridor)
    {
        return new CorridorSave
        {
            Type = corridor.Type,
            Points = corridor.Points.Select(PositionMapper.ToSave).ToList(),
            Cells = corridor.Cells.Select(PositionMapper.ToSave).ToList()
        };
    }

    public static Corridor FromSave(CorridorSave save)
    {
        var corridor = new Corridor
        {
            Type = save.Type,
            Points = save.Points.Select(PositionMapper.FromSave).ToList(),
            Cells = save.Cells.Select(PositionMapper.FromSave).ToList()
        };
        
        if (corridor.Points != null && corridor.Points.Count >= 2 && corridor.Cells != null)
        {
            corridor.CellToSegments = BresenhamUtils.GetCellToSegmentsMapping(corridor.Points, corridor.Cells);
        }
        
        return corridor;
    }
}

