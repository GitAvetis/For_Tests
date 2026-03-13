/// @file Point.cs
/// @brief Структура, представляющая координаты ячейки в лабиринте
using System;

namespace Core
{
    /// <summary>
    /// @brief Структура для хранения координат ячейки лабиринта
    /// 
    /// Point представляет неизменяемую структуру (value type) для хранения
    /// позиции ячейки в лабиринте. Реализует интерфейс IEquatable&lt;Point&gt;
    /// для эффективного сравнения точек, что необходимо для работы алгоритмов
    /// поиска пути и коллекций (HashSet, Dictionary).
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// @brief Номер строки (ряда) ячейки в лабиринте
        /// 
        /// Индексация начинается с 0.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// @brief Номер столбца ячейки в лабиринте
        /// 
        /// Индексация начинается с 0.
        /// </summary>
        public int Col { get; }

        /// <summary>
        /// @brief Создает новую точку с заданными координатами
        /// </summary>
        /// <param name="row">Номер строки (ряда)</param>
        /// <param name="col">Номер столбца</param>
        public Point(int row, int col)
        {
            Row = row;
            Col = col;
        }

        /// <summary>
        /// @brief Сравнивает текущую точку с другой точкой типа Point
        /// </summary>
        /// <param name="other">Другая точка для сравнения</param>
        /// <returns>true, если точки имеют одинаковые координаты; иначе false</returns>
        public bool Equals(Point other)
        {
            return Row == other.Row && Col == other.Col;
        }

        /// <summary>
        /// @brief Сравнивает текущую точку с объектом
        /// </summary>
        /// <param name="obj">Объект для сравнения</param>
        /// <returns>true, если объект является точкой Point с теми же координатами; иначе false</returns>
        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }

        /// <summary>
        /// @brief Возвращает хеш-код точки
        /// 
        /// Использует комбинацию хеш-кодов координат Row и Col.
        /// </summary>
        /// <returns>Хеш-код для использования в коллекциях</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        /// <summary>
        /// @brief Оператор сравнения на равенство двух точек
        /// </summary>
        /// <param name="left">Левая точка</param>
        /// <param name="right">Правая точка</param>
        /// <returns>true, если точки равны; иначе false</returns>
        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// @brief Оператор сравнения на неравенство двух точек
        /// </summary>
        /// <param name="left">Левая точка</param>
        /// <param name="right">Правая точка</param>
        /// <returns>true, если точки не равны; иначе false</returns>
        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
    }
}
