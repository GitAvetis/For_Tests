/// @file MazeFileSaver.cs
/// @brief Класс для сохранения лабиринтов в файлы
using Core;
using System.IO;
using System.Text;

namespace Infrasructure
{
    /// <summary>
    /// @brief Класс для сохранения лабиринтов в текстовые файлы
    /// 
    /// MazeFileSaver предоставляет функциональность для записи лабиринтов в файлы
    /// в стандартизированном текстовом формате. Выполняет проверку корректности
    /// размеров лабиринта перед сохранением.
    /// </summary>
    public class MazeFileSaver
    {
        /// <summary>
        /// @brief Максимально допустимый размер сохраняемого лабиринта (включительно)
        /// </summary>
        private const int MaxSize = 50;

        /// <summary>
        /// @brief Сохраняет лабиринт в файл
        /// 
        /// Метод сохраняет лабиринт в текстовый файл следующего формата:
        /// - Первая строка: два числа через пробел (количество строк и столбцов)
        /// - Пустая строка (для читаемости)
        /// - rows строк с матрицей правых стен (rows x cols значений 0 или 1)
        /// - Пустая строка (для разделения матриц)
        /// - rows строк с матрицей нижних стен (rows x cols значений 0 или 1)
        /// 
        /// Формат матриц стен:
        /// - 0 - отсутствие стены
        /// - 1 - наличие стены
        /// Значения в строках разделяются пробелами.
        /// </summary>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="maze">Сохраняемый лабиринт</param>
        /// <exception cref="System.ArgumentNullException">Выбрасывается, если maze равен null</exception>
        /// <exception cref="System.ArgumentException">Выбрасывается, если размеры лабиринта 
        /// выходят за допустимые пределы (меньше 2 или больше MaxSize)</exception>
        public void Save(string path, Maze maze)
        {
            if (maze == null)
                throw new System.ArgumentNullException(nameof(maze));

            if (maze.Rows <= 1 || maze.Cols <= 1 || maze.Rows > MaxSize || maze.Cols > MaxSize)
                throw new System.ArgumentException($"Maze dimensions must be between 2 and {MaxSize}");

            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine($"{maze.Rows} {maze.Cols}");
                writer.WriteLine(); 

                WriteWallMatrix(writer, maze.RightWalls, maze.Rows, maze.Cols);
                writer.WriteLine(); 

                WriteWallMatrix(writer, maze.BottomWalls, maze.Rows, maze.Cols);
            }
        }

        /// <summary>
        /// @brief Записывает матрицу стен в файл
        /// 
        /// Вспомогательный метод для записи одной матрицы стен (правых или нижних).
        /// Создает строки с пробелами между значениями для лучшей читаемости.
        /// </summary>
        /// <param name="writer">StreamWriter для записи в файл</param>
        /// <param name="walls">Матрица стен для сохранения</param>
        /// <param name="rows">Количество строк в матрице</param>
        /// <param name="cols">Количество столбцов в матрице</param>
        private void WriteWallMatrix(StreamWriter writer, bool[,] walls, int rows, int cols)
        {
            for (int i = 0; i < rows; i++)
            {
                var line = new StringBuilder();
                for (int j = 0; j < cols; j++)
                {
                    line.Append(walls[i, j] ? "1" : "0");
                    if (j < cols - 1)
                        line.Append(' ');
                }
                writer.WriteLine(line.ToString());
            }
        }
    }
}