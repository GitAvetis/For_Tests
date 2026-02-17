using System.Collections.Generic;
using System.Linq;

namespace ProjectTeam01.domain.generation;

/// Утилиты для работы с алгоритмом Брезенхема и сегментами коридоров
public static class BresenhamUtils
{
    /// Проверить, находится ли позиция в сегменте между двумя точками
    public static bool IsPositionInSegment(Position pos, Position start, Position end)
    {
        if ((pos.X == start.X && pos.Y == start.Y) || (pos.X == end.X && pos.Y == end.Y))
        {
            return true;
        }
        
        int minX = System.Math.Min(start.X, end.X);
        int maxX = System.Math.Max(start.X, end.X);
        int minY = System.Math.Min(start.Y, end.Y);
        int maxY = System.Math.Max(start.Y, end.Y);
        
        if (start.Y == end.Y)
        {
            return pos.Y == start.Y && pos.X >= minX && pos.X <= maxX;
        }
        else if (start.X == end.X)
        {
            return pos.X == start.X && pos.Y >= minY && pos.Y <= maxY;
        }
        else
        {
            return IsOnBresenhamLine(pos, start, end);
        }
    }
    
    /// Проверить, находится ли позиция на линии Брезенхема между двумя точками
    private static bool IsOnBresenhamLine(Position pos, Position start, Position end)
    {
        int x0 = start.X;
        int y0 = start.Y;
        int x1 = end.X;
        int y1 = end.Y;
        
        int dx = System.Math.Abs(x1 - x0);
        int dy = System.Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        
        int x = x0;
        int y = y0;
        
        while (true)
        {
            if (x == pos.X && y == pos.Y)
                return true;
            
            if (x == x1 && y == y1)
                break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
        
        return false;
    }
    
    /// Определить индексы сегментов коридора для позиции
    public static List<int> GetSegmentIndicesForPosition(List<Position> points, Position position)
    {
        var indices = new List<int>();
        
        if (points == null || points.Count < 2)
            return indices;
        
        bool isBendPoint = points.Any(p => p.X == position.X && p.Y == position.Y);
        
        if (isBendPoint)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X == position.X && points[i].Y == position.Y)
                {
                    if (i > 0)
                    {
                        indices.Add(i - 1);
                    }
                    if (i < points.Count - 1)
                    {
                        indices.Add(i);
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                var start = points[i];
                var end = points[i + 1];
                
                if (IsPositionInSegment(position, start, end))
                {
                    indices.Add(i);
                }
            }
        }
        
        return indices;
    }
    
    /// Определить, к каким сегментам относятся все клетки коридора
    public static Dictionary<Position, HashSet<int>> GetCellToSegmentsMapping(List<Position> points, List<Position> cells)
    {
        var cellToSegments = new Dictionary<Position, HashSet<int>>();
        
        if (points == null || points.Count < 2 || cells == null)
            return cellToSegments;
        
        var bendPoints = new HashSet<Position>();
        foreach (var point in points)
        {
            bendPoints.Add(point);
        }
        
        for (int segmentIndex = 0; segmentIndex < points.Count - 1; segmentIndex++)
        {
            var start = points[segmentIndex];
            var end = points[segmentIndex + 1];
            
            foreach (var cell in cells)
            {
                if (bendPoints.Contains(cell))
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (points[i].X == cell.X && points[i].Y == cell.Y)
                        {
                            if (i > 0 && i - 1 == segmentIndex)
                            {
                                if (!cellToSegments.ContainsKey(cell))
                                    cellToSegments[cell] = new HashSet<int>();
                                cellToSegments[cell].Add(segmentIndex);
                            }
                            if (i < points.Count - 1 && i == segmentIndex)
                            {
                                if (!cellToSegments.ContainsKey(cell))
                                    cellToSegments[cell] = new HashSet<int>();
                                cellToSegments[cell].Add(segmentIndex);
                            }
                            break;
                        }
                    }
                }
                else if (IsPositionInSegment(cell, start, end))
                {
                    if (!cellToSegments.ContainsKey(cell))
                        cellToSegments[cell] = new HashSet<int>();
                    cellToSegments[cell].Add(segmentIndex);
                }
            }
        }
        
        return cellToSegments;
    }
}
