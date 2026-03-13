/// @file MazeGenerator.cs
/// @brief Класс для генерации лабиринтов с использованием алгоритма Эллера
namespace Core
{
    /// <summary>
    /// @brief Статический класс для генерации лабиринтов алгоритмом Эллера
    /// 
    /// MazeGenerator реализует алгоритм Эллера (Eller's algorithm) для генерации
    /// идеальных лабиринтов (без циклов и изолированных областей). Алгоритм работает
    /// построчно, гарантируя связность лабиринта и равномерное распределение стен.
    /// </summary>
    public class MazeGenerator
    {
        /// <summary>
        /// @brief Генератор случайных чисел для принятия решений о размещении стен
        /// </summary>
        private static readonly Random _random = new Random();

        /// <summary>
        /// @brief Генерирует новый лабиринт заданного размера
        /// 
        /// Основной метод генерации, создающий лабиринт размером rows x cols.
        /// Процесс генерации:
        /// 1. Создание первой строки с уникальными множествами
        /// 2. Для каждой строки (кроме последней):
        ///    - Установка правых стен с объединением множеств
        ///    - Установка нижних стен
        ///    - Подготовка следующей строки
        /// 3. Обработка последней строки (все ячейки должны быть в одном множестве)
        /// </summary>
        /// <param name="rows">Количество строк в лабиринте</param>
        /// <param name="cols">Количество столбцов в лабиринте</param>
        /// <returns>Двумерный массив ячеек MazeCell, представляющих сгенерированный лабиринт</returns>
        public static MazeCell[][] MazeGeneration(int rows, int cols)
        {
            HashSet<int> activeSets = new HashSet<int>();
            MazeCell[][] field = new MazeCell[rows][];

            field[0] = CreateFirstRowField(cols, activeSets);
            SettingRightWalls(field[0], activeSets);
            SettingBottomWalls(field[0]);

            for (int i = 1; i < rows - 1; i++)
            {
                field[i] = CopyPrevRow(field[i - 1], activeSets);
                SettingRightWalls(field[i], activeSets);
                SettingBottomWalls(field[i]);
            }

            if (rows > 1)
            {
                field[rows - 1] = CopyPrevRow(field[rows - 2], activeSets);
                SettingRightWallsLastRow(field[rows - 1]);
                for (int j = 0; j < cols; j++)
                    field[rows - 1][j].WallB = true;
            }

            return field;
        }

        /// <summary>
        /// @brief Создает первую строку лабиринта с уникальными множествами
        /// </summary>
        /// <param name="cols">Количество столбцов</param>
        /// <param name="activeSets">Множество активных идентификаторов</param>
        /// <returns>Массив ячеек первой строки</returns>
        private static MazeCell[] CreateFirstRowField(int cols, HashSet<int> activeSets)
        {
            MazeCell[] row = new MazeCell[cols];
            for (int j = 0; j < cols; j++)
            {
                int newSet = j + 1;
                row[j] = new MazeCell(newSet);
                activeSets.Add(newSet);
            }
            return row;
        }

        /// <summary>
        /// @brief Изменяет идентификаторы множества в строке
        /// 
        /// Заменяет все вхождения старого множества на новое.
        /// </summary>
        /// <param name="fullRow">Строка ячеек</param>
        /// <param name="checkedSet">Старое множество для замены</param>
        /// <param name="newSet">Новое множество</param>
        /// <returns>true, если хотя бы одна ячейка была изменена</returns>
        private static bool RowChecks(MazeCell[] fullRow, int checkedSet, int newSet)
        {
            bool res = false;
            for (int i = 0; i < fullRow.Length; i++)
            {
                if (fullRow[i].SetId == checkedSet)
                {
                    fullRow[i].SetId = newSet;
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// @brief Проверяет возможность установки нижней стены
        /// 
        /// В множестве должна остаться хотя бы одна ячейка без нижней стены
        /// для обеспечения связности с следующим рядом.
        /// </summary>
        /// <param name="row">Текущая строка</param>
        /// <param name="actCell">Проверяемая ячейка</param>
        /// <returns>true, если можно установить стену</returns>
        private static bool CanPutBottomWall(MazeCell[] row, MazeCell actCell)
        {
            int BWalls = 0;
            int numOfSetMembers = 0;
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i].SetId == actCell.SetId)
                {
                    numOfSetMembers++;
                    if (row[i].WallB == true)
                        BWalls++;
                }
            }
            return (numOfSetMembers - BWalls) > 1 ? true : false;
        }

        /// <summary>
        /// @brief Получает следующий доступный идентификатор множества
        /// </summary>
        /// <param name="activeSets">Множество активных идентификаторов</param>
        /// <returns>Новый уникальный идентификатор</returns>
        private static int GetNextSet(HashSet<int> activeSets)
        {
            int nextSet = 1;
            while (activeSets.Contains(nextSet))
            {
                nextSet++;
            }
            activeSets.Add(nextSet);
            return nextSet;
        }

        /// <summary>
        /// @brief Создает новую строку на основе предыдущей
        /// 
        /// Ячейки, имеющие нижнюю стену в предыдущей строке, получают новые множества,
        /// остальные наследуют множества из предыдущей строки.
        /// </summary>
        /// <param name="prevRow">Предыдущая строка</param>
        /// <param name="activeSets">Множество активных идентификаторов</param>
        /// <returns>Новая строка ячеек</returns>
        private static MazeCell[] CopyPrevRow(MazeCell[] prevRow, HashSet<int> activeSets)
        {
            int length = prevRow.Length;
            MazeCell[] newRow = new MazeCell[length];
            for (int i = 0; i < length; i++)
            {
                newRow[i] = new MazeCell();
                if (prevRow[i].WallB)
                    newRow[i].SetId = GetNextSet(activeSets);
                else
                    newRow[i].SetId = prevRow[i].SetId;
                newRow[i].WallB = false;
                newRow[i].WallR = false;
            }
            return newRow;
        }

        /// <summary>
        /// @brief Принимает случайное решение о размещении стены
        /// </summary>
        /// <returns>true - ставить стену, false - не ставить</returns>
        private static bool MakeWalls()
        {
            int randNum = _random.Next(0, 2);
            return randNum % 2 == 0;
        }

        /// <summary>
        /// @brief Устанавливает правые стены для строки (кроме последней)
        /// 
        /// Если соседние ячейки принадлежат разным множествам, может объединить их
        /// или поставить стену случайным образом.
        /// </summary>
        /// <param name="row">Обрабатываемая строка</param>
        /// <param name="activeSets">Множество активных идентификаторов</param>
        private static void SettingRightWalls(MazeCell[] row, HashSet<int> activeSets)
        {
            for (int j = 0; j < row.Length - 1; j++)
            {
                MazeCell actCell = row[j];
                MazeCell nextCell = row[j + 1];

                if (actCell.SetId == nextCell.SetId)
                {
                    row[j].WallR = true;
                    continue;
                }

                if (MakeWalls())
                    row[j].WallR = true;
                else
                {
                    int oldSet = nextCell.SetId;
                    int newSet = actCell.SetId;

                    row[j].WallR = false;
                    if (RowChecks(row, oldSet, newSet))
                        activeSets.Remove(oldSet);
                }
            }
            row[row.Length - 1].WallR = true;
        }

        /// <summary>
        /// @brief Устанавливает правые стены для последней строки
        /// 
        /// В последней строке все множества должны быть объединены в одно,
        /// чтобы лабиринт был связным.
        /// </summary>
        /// <param name="row">Последняя строка лабиринта</param>
        private static void SettingRightWallsLastRow(MazeCell[] row)
        {
            for (int j = 0; j < row.Length - 1; j++)
            {
                if (row[j].SetId != row[j + 1].SetId)
                {
                    int oldSet = row[j + 1].SetId;
                    int newSet = row[j].SetId;

                    for (int i = 0; i < row.Length; i++)
                        if (row[i].SetId == oldSet)
                            row[i].SetId = newSet;

                    row[j].WallR = false;
                }
                else
                {
                    row[j].WallR = true;
                }
            }

            row[row.Length - 1].WallR = true;
        }

        /// <summary>
        /// @brief Устанавливает нижние стены для строки
        /// 
        /// Случайным образом размещает нижние стены, гарантируя,
        /// что в каждом множестве останется хотя бы один проход вниз.
        /// </summary>
        /// <param name="row">Обрабатываемая строка</param>
        private static void SettingBottomWalls(MazeCell[] row)
        {
            for (int j = 0; j < row.Length; j++)
            {
                MazeCell actCell = row[j];

                if (MakeWalls() && CanPutBottomWall(row, actCell))
                {
                    actCell.WallB = true;
                }
                else
                    actCell.WallB = false;
            }
        }
    }
}
