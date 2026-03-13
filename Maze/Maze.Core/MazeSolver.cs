/// @file MazeSolver.cs
/// @brief Класс для поиска пути в лабиринте с использованием алгоритма BFS
using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// @brief Класс для решения лабиринтов методом поиска в ширину (BFS)
    /// 
    /// MazeSolver реализует алгоритм поиска кратчайшего пути в лабиринте
    /// от стартовой до конечной точки. Использует поиск в ширину (BFS),
    /// что гарантирует нахождение кратчайшего пути по количеству пройденных ячеек.
    /// </summary>
    public class MazeSolver
    {
        /// <summary>
        /// @brief Находит путь от начальной до конечной точки в лабиринте
        /// 
        /// Использует алгоритм BFS для поиска кратчайшего пути. Если путь существует,
        /// возвращает список точек от start до end включительно. Если путь не найден,
        /// возвращает пустой список.
        /// </summary>
        /// <param name="maze">Лабиринт, в котором осуществляется поиск</param>
        /// <param name="start">Начальная точка пути</param>
        /// <param name="end">Конечная точка пути</param>
        /// <returns>
        /// Список точек, представляющих путь от start до end, включая обе точки.
        /// Если start и end совпадают, возвращает список из одной точки.
        /// Если путь не найден, возвращает пустой список.
        /// </returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если maze равен null</exception>
        /// <exception cref="ArgumentException">Выбрасывается, если start или end выходят за границы лабиринта</exception>
        public List<Point> Solve(Maze maze, Point start, Point end)
        {
            if (maze == null)
                throw new ArgumentNullException(nameof(maze));

            if (!IsValidPoint(maze, start))
                throw new ArgumentException("Start point is out of bounds", nameof(start));

            if (!IsValidPoint(maze, end))
                throw new ArgumentException("End point is out of bounds", nameof(end));

            if (start == end)
                return new List<Point> { start };

            var queue = new Queue<Point>();
            var visited = new HashSet<Point>();
            var parent = new Dictionary<Point, Point>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == end)
                {
                    return ReconstructPath(parent, start, end);
                }

                var neighbors = GetNeighbors(maze, current);
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        parent[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return new List<Point>(); // No path found
        }

        /// <summary>
        /// @brief Проверяет, находится ли точка в пределах лабиринта
        /// </summary>
        /// <param name="maze">Лабиринт для проверки</param>
        /// <param name="point">Проверяемая точка</param>
        /// <returns>true, если точка находится в пределах лабиринта; иначе false</returns>
        private bool IsValidPoint(Maze maze, Point point)
        {
            return point.Row >= 0 && point.Row < maze.Rows &&
                   point.Col >= 0 && point.Col < maze.Cols;
        }

        /// <summary>
        /// @brief Получает список доступных соседей для текущей точки
        /// 
        /// Проверяет наличие проходов в четырех направлениях:
        /// - вверх (если нет нижней стены у ячейки сверху)
        /// - вниз (если нет нижней стены у текущей ячейки)
        /// - влево (если нет правой стены у ячейки слева)
        /// - вправо (если нет правой стены у текущей ячейки)
        /// </summary>
        /// <param name="maze">Лабиринт для анализа</param>
        /// <param name="point">Текущая точка</param>
        /// <returns>Список доступных для перемещения соседних точек</returns>
        private List<Point> GetNeighbors(Maze maze, Point point)
        {
            var neighbors = new List<Point>();

            if (point.Row > 0 && !maze.BottomWalls[point.Row - 1, point.Col])
            {
                neighbors.Add(new Point(point.Row - 1, point.Col));
            }

            if (point.Row < maze.Rows - 1 && !maze.BottomWalls[point.Row, point.Col])
            {
                neighbors.Add(new Point(point.Row + 1, point.Col));
            }

            if (point.Col > 0 && !maze.RightWalls[point.Row, point.Col - 1])
            {
                neighbors.Add(new Point(point.Row, point.Col - 1));
            }

            if (point.Col < maze.Cols - 1 && !maze.RightWalls[point.Row, point.Col])
            {
                neighbors.Add(new Point(point.Row, point.Col + 1));
            }

            return neighbors;
        }

        /// <summary>
        /// @brief Восстанавливает путь от старта до финиша, используя словарь родителей
        /// 
        /// Проходит по ссылкам от конечной точки к начальной, используя информацию
        /// о родительских узлах, затем разворачивает полученный путь.
        /// </summary>
        /// <param name="parent">Словарь, содержащий для каждой точки её родителя на пути</param>
        /// <param name="start">Начальная точка пути</param>
        /// <param name="end">Конечная точка пути</param>
        /// <returns>Список точек от start до end в правильном порядке</returns>
        private List<Point> ReconstructPath(Dictionary<Point, Point> parent, Point start, Point end)
        {
            var path = new List<Point>();
            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = parent[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }
    }
}
