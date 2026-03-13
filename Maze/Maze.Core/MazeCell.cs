/// @file MazeCell.cs
/// @brief Класс, представляющий ячейку лабиринта в процессе генерации
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// @brief Класс ячейки лабиринта, используемый при генерации алгоритмом Эллера
    /// 
    /// MazeCell представляет ячейку лабиринта на этапе его генерации.
    /// Хранит информацию о принадлежности к множеству (SetId) для алгоритма Эллера,
    /// а также флаги наличия правой и нижней стен.
    /// </summary>
    public class MazeCell
    {
        /// <summary>
        /// @brief Идентификатор множества для алгоритма генерации
        /// 
        /// Используется в алгоритме Эллера для отслеживания связности ячеек
        /// в текущем ряду лабиринта. Ячейки с одинаковым SetId принадлежат
        /// одному множеству и соединены проходами по горизонтали.
        /// </summary>
        public int SetId { get; set; }

        /// <summary>
        /// @brief Наличие правой стены у ячейки
        /// 
        /// true - стена существует (проход вправо закрыт)
        /// false - стена отсутствует (проход вправо открыт)
        /// </summary>
        public bool WallR { get; set; }

        /// <summary>
        /// @brief Наличие нижней стены у ячейки
        /// 
        /// true - стена существует (проход вниз закрыт)
        /// false - стена отсутствует (проход вниз открыт)
        /// </summary>
        public bool WallB { get; set; }

        /// <summary>
        /// @brief Конструктор, создающий ячейку с заданным идентификатором множества
        /// 
        /// Инициализирует ячейку без стен (WallR = false, WallB = false)
        /// и с указанным идентификатором множества.
        /// </summary>
        /// <param name="id">Идентификатор множества для ячейки</param>
        public MazeCell(int id)
        {
            SetId = id;
            WallR = false;
            WallB = false;
        }

        /// <summary>
        /// @brief Конструктор по умолчанию
        /// 
        /// Создает ячейку с идентификатором множества 0 и без стен.
        /// </summary>
        public MazeCell()
        {
            SetId = 0;
            WallR = false;
            WallB = false;
        }
    }
}
