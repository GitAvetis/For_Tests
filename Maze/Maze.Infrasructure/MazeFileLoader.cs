/// @file MazeFileLoader.cs
/// @brief Класс для загрузки лабиринтов из файлов
using Core;
using System;
using System.IO;

namespace Infrasructure
{
    /// <summary>
    /// @brief Класс для загрузки лабиринтов из текстовых файлов
    /// 
    /// MazeFileLoader предоставляет функциональность для чтения лабиринтов из файлов
    /// с проверкой формата и валидацией данных. Поддерживает загрузку лабиринтов
    /// размером до 50x50 ячеек.
    /// </summary>
    public class MazeFileLoader
    {
        /// <summary>
        /// @brief Максимально допустимый размер лабиринта (включительно)
        /// </summary>
        private const int MaxSize = 50;

        /// <summary>
        /// @brief Загружает лабиринт из файла
        /// 
        /// Метод читает файл следующего формата:
        /// - Первая строка: два числа через пробел (количество строк и столбцов)
        /// - Далее rows строк с матрицей правых стен (rows x cols значений 0 или 1)
        /// - Далее rows строк с матрицей нижних стен (rows x cols значений 0 или 1)
        /// 
        /// Формат матриц стен:
        /// - 0 - отсутствие стены
        /// - 1 - наличие стены
        /// </summary>
        /// <param name="path">Путь к файлу с лабиринтом</param>
        /// <returns>Загруженный объект Maze</returns>
        /// <exception cref="FileNotFoundException">Выбрасывается, если файл не найден</exception>
        /// <exception cref="InvalidDataException">Выбрасывается при неверном формате файла, 
        /// некорректных размерах или значениях стен</exception>
        public Maze Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Maze file not found", path);

            using (var reader = new StreamReader(path))
            {
                string[] dimensions = reader.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (dimensions == null || dimensions.Length != 2)
                    throw new InvalidDataException("Invalid file format: expected dimensions");

                if (!int.TryParse(dimensions[0], out int rows) ||
                    !int.TryParse(dimensions[1], out int cols))
                    throw new InvalidDataException("Invalid dimensions format");

                if (rows <= 1 || cols <= 1 || rows > MaxSize || cols > MaxSize)
                    throw new InvalidDataException($"Maze dimensions must be between 2 and {MaxSize}");

                var maze = new Maze(rows, cols);

                ReadWallMatrix(reader, maze.RightWalls, rows, cols);

                ReadWallMatrix(reader, maze.BottomWalls, rows, cols);

                return maze;
            }
        }

        /// <summary>
        /// @brief Читает матрицу стен из файла
        /// 
        /// Вспомогательный метод для чтения одной матрицы стен (правых или нижних).
        /// Ожидает rows строк, каждая из которых содержит cols значений 0 или 1,
        /// разделенных пробелами.
        /// </summary>
        /// <param name="reader">StreamReader для чтения файла</param>
        /// <param name="walls">Матрица стен для заполнения</param>
        /// <param name="rows">Количество строк в матрице</param>
        /// <param name="cols">Количество столбцов в матрице</param>
        /// <exception cref="InvalidDataException">Выбрасывается при неожиданном конце файла,
        /// неверном количестве значений в строке или некорректном значении (не 0 и не 1)</exception>
        private void ReadWallMatrix(StreamReader reader, bool[,] walls, int rows, int cols)
        {
            string line;
            int row = 0;

            while (row < rows)
            {
                line = reader.ReadLine();
                if (line == null)
                    throw new InvalidDataException($"Unexpected end of file at row {row}");

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] cells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (cells.Length != cols)
                    throw new InvalidDataException($"Expected {cols} values at row {row}, got {cells.Length}");

                for (int col = 0; col < cols; col++)
                {
                    if (cells[col] != "0" && cells[col] != "1")
                        throw new InvalidDataException($"Invalid value at [{row},{col}]: expected 0 or 1");

                    walls[row, col] = cells[col] == "1";
                }

                row++;
            }
        }
    }
}