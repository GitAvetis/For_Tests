/// @file MazeFileService.cs
/// @brief Фасадный сервис для работы с файлами лабиринтов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Infrasructure
{
    /// <summary>
    /// @brief Фасадный сервис для загрузки и сохранения лабиринтов
    /// 
    /// MazeFileService объединяет функциональность загрузки и сохранения лабиринтов
    /// в едином интерфейсе. Использует паттерн "Фасад" для упрощения работы с
    /// классами MazeFileLoader и MazeFileSaver, предоставляя клиентам простой
    /// и единообразный способ работы с файлами лабиринтов.
    /// </summary>
    public class MazeFileService
    {
        /// <summary>
        /// @brief Экземпляр загрузчика лабиринтов
        /// </summary>
        private readonly MazeFileLoader _loader;
        /// <summary>
        /// @brief Экземпляр сохранителя лабиринтов
        /// </summary>
        private readonly MazeFileSaver _saver;

        /// <summary>
        /// @brief Конструктор сервиса
        /// 
        /// Инициализирует внутренние компоненты для загрузки и сохранения лабиринтов.
        /// </summary>
        public MazeFileService()
        {
            _loader = new MazeFileLoader();
            _saver = new MazeFileSaver();
        }

        /// <summary>
        /// @brief Загружает лабиринт из файла
        /// 
        /// Делегирует операцию загрузки внутреннему экземпляру MazeFileLoader.
        /// </summary>
        /// <param name="path">Путь к файлу с лабиринтом</param>
        /// <returns>Загруженный объект Maze</returns>
        /// <exception cref="System.IO.FileNotFoundException">Выбрасывается, если файл не найден</exception>
        /// <exception cref="System.IO.InvalidDataException">Выбрасывается при неверном формате файла</exception>
        public Maze Load(string path) => _loader.Load(path);

        /// <summary>
        /// @brief Сохраняет лабиринт в файл
        /// 
        /// Делегирует операцию сохранения внутреннему экземпляру MazeFileSaver.
        /// </summary>
        /// <param name="path">Путь для сохранения файла</param>
        /// <param name="maze">Сохраняемый лабиринт</param>
        /// <exception cref="System.ArgumentNullException">Выбрасывается, если maze равен null</exception>
        /// <exception cref="System.ArgumentException">Выбрасывается, если размеры лабиринта некорректны</exception>
        public void Save(string path, Maze maze) => _saver.Save(path, maze);
    }
}