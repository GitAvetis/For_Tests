/// @file Maze.cs
/// @brief Класс, представляющий модель лабиринта
using System;
using Core;

namespace Core
{
    /// <summary>
    /// @brief Модель лабиринта, содержащая информацию о его размерах и стенах
    /// 
    /// Класс Maze представляет собой основную модель лабиринта в системе.
    /// Хранит информацию о количестве строк и столбцов, а также о наличии
    /// правых и нижних стен для каждой ячейки. Поддерживает создание лабиринта
    /// через генерацию или из массива ячеек.
    /// </summary>
    public class Maze
    {
        /// <summary>
        /// @brief Количество строк (рядов) в лабиринте
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// @brief Количество столбцов в лабиринте
        /// </summary>
        public int Cols { get; }

        /// <summary>
        /// @brief Матрица правых стен лабиринта
        /// 
        /// Двумерный массив размером [Rows, Cols], где:
        /// - true - наличие правой стены у ячейки [row, col]
        /// - false - отсутствие правой стены (проход)
        /// </summary>
        public bool[,] RightWalls { get; }

        /// <summary>
        /// @brief Матрица нижних стен лабиринта
        /// 
        /// Двумерный массив размером [Rows, Cols], где:
        /// - true - наличие нижней стены у ячейки [row, col]
        /// - false - отсутствие нижней стены (проход)
        /// </summary>
        public bool[,] BottomWalls { get; }

        /// <summary>
        /// @brief Конструктор, создающий лабиринт заданного размера со всеми стенами
        /// 
        /// Инициализирует лабиринт с указанным количеством строк и столбцов.
        /// По умолчанию все стены установлены в true (закрыты).
        /// </summary>
        /// <param name="rows">Количество строк в лабиринте</param>
        /// <param name="cols">Количество столбцов в лабиринте</param>
        public Maze(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            RightWalls = new bool[Rows, Cols];
            BottomWalls = new bool[Rows, Cols];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    RightWalls[r, c] = true;
                    BottomWalls[r, c] = true;
                }
            }
        }

        /// <summary>
        /// @brief Конструктор, создающий лабиринт из двумерного массива ячеек
        /// 
        /// Преобразует массив объектов MazeCell в модель лабиринта,
        /// извлекая информацию о стенах из каждой ячейки.
        /// </summary>
        /// <param name="cells">Двумерный массив ячеек (массив массивов MazeCell)</param>
        /// <exception cref="ArgumentException">Выбрасывается, если:
        /// - cells равен null или пуст
        /// - любая строка имеет длину, отличную от первой строки
        /// </exception>
        public Maze(MazeCell[][] cells)
        {
            if (cells == null || cells.Length == 0)
                throw new ArgumentException("Cells array cannot be null or empty", nameof(cells));

            Rows = cells.Length;
            Cols = cells[0].Length;
            RightWalls = new bool[Rows, Cols];
            BottomWalls = new bool[Rows, Cols];

            for (int i = 0; i < Rows; i++)
            {
                if (cells[i] == null || cells[i].Length != Cols)
                    throw new ArgumentException($"Row {i} has invalid length", nameof(cells));

                for (int j = 0; j < Cols; j++)
                {
                    RightWalls[i, j] = cells[i][j].WallR;
                    BottomWalls[i, j] = cells[i][j].WallB;
                }
            }
        }

        /// <summary>
        /// @brief Генерирует новый лабиринт заданного размера
        /// 
        /// Статический метод, использующий алгоритм генерации лабиринтов
        /// для создания нового экземпляра Maze.
        /// </summary>
        /// <param name="rows">Количество строк в генерируемом лабиринте</param>
        /// <param name="cols">Количество столбцов в генерируемом лабиринте</param>
        /// <returns>Новый экземпляр Maze со сгенерированной структурой</returns>
        public static Maze Generate(int rows, int cols)
        {
            var cells = MazeGenerator.MazeGeneration(rows, cols);
            return new Maze(cells);
        }
    }
}
