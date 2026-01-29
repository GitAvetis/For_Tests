using ProjectTeam01.datalayer.Mappers;
using ProjectTeam01.datalayer.Models;
using ProjectTeam01.domain;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Session;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectTeam01.datalayer
{
    /// Сервис для работы с сохранением и загрузкой игровых данных.
    /// Отвечает за сериализацию/десериализацию игрового состояния в JSON.
    internal class GameDataService
    {
        public static GameSave CreateSave(Hero hero, List<Enemy> enemies, List<Item> items, Level level, GameStatistics statistics)
        {
            var save = new GameSave
            {
                Hero = HeroMapper.ToSave(hero),
                Enemies = enemies.Select(e => EnemyMapper.ToSave(e)).ToList(),
                Items = items.Select(i => ItemMapper.ToSave(i)).ToList(),
                Level = LevelMapper.ToSave(level),
                Statistics = GameStatisticsMapper.ToSave(statistics),
                GameLevel = level.LevelNumber
            };
            return save;
        }

        /// Сохранить GameSave в JSON файл
        public static void SaveToFile(GameSave save, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            string jsonString = JsonSerializer.Serialize(save, options);
            File.WriteAllText(filePath, jsonString);
        }

        /// Загрузить GameSave из JSON файла (внутренний метод)
        private static void LoadFromFile(string filePath, out GameSave save)
        {
            string jsonString = File.ReadAllText(filePath);
            save = JsonSerializer.Deserialize<GameSave>(jsonString) ?? throw new Exception("Failed to deserialize game save.");
        }

        /// Загрузить игру из файла и восстановить все объекты (низкоуровневый метод).
        internal static void LoadSave(string filePath, out Hero hero, out List<Enemy> enemies, out List<Item> items, out Level level, out GameStatistics statistics)
        {
            LoadFromFile(filePath, out GameSave gameSave);
            hero = HeroMapper.FromSave(gameSave.Hero);
            enemies = gameSave.Enemies.Select(es => EnemyMapper.FromSave(es)).ToList();
            items = gameSave.Items.Select(isave => ItemMapper.FromSave(isave)).ToList();
            level = LevelMapper.FromSave(gameSave.Level);
            statistics = GameStatisticsMapper.FromSave(gameSave.Statistics);
        }

        /// Загрузить игру из файла и вернуть GameSession.
        /// Восстанавливает все данные (герой, враги, предметы, уровень, статистика).
        public static GameSession LoadGame(string filePath)
        {
            // Загружаем данные
            Hero hero;
            List<Enemy> enemies;
            List<Item> items;
            Level level;
            GameStatistics statistics;

            LoadSave(filePath, out hero, out enemies, out items, out level, out statistics);

            // Добавляем сущности на уровень
            level.AddEntity(hero);
            foreach (var enemy in enemies)
            {
                level.AddEntity(enemy);
            }
            foreach (var item in items)
            {
                level.AddEntity(item);
            }

            // Создаем и возвращаем GameSession с загруженной статистикой
            return new GameSession(level, hero, level.LevelNumber, statistics);
        }

        // ========== РАБОТА СО СТАТИСТИКОЙ И ТАБЛИЦЕЙ ЛИДЕРОВ ==========

        /// Добавить попытку прохождения в таблицу лидеров (вызывается при смерти/победе)
        public static void AddAttemptToScoreboard(GameStatistics statistics, string scoreboardPath)
        {
            // Загружаем существующую таблицу лидеров или создаем новую
            ScoreboardSave scoreboard;
            if (File.Exists(scoreboardPath))
            {
                string jsonString = File.ReadAllText(scoreboardPath);
                scoreboard = JsonSerializer.Deserialize<ScoreboardSave>(jsonString) ?? new ScoreboardSave();
            }
            else
            {
                scoreboard = new ScoreboardSave();
            }

            // Добавляем новую попытку
            var attemptSave = GameStatisticsMapper.ToSave(statistics);
            scoreboard.SessionStats.Add(attemptSave);

            // Сохраняем обновленную таблицу
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            string updatedJson = JsonSerializer.Serialize(scoreboard, options);
            File.WriteAllText(scoreboardPath, updatedJson);
        }

        /// Загрузить таблицу лидеров из файла
        public static ScoreboardSave LoadScoreboard(string scoreboardPath)
        {
            if (!File.Exists(scoreboardPath))
            {
                return new ScoreboardSave();
            }

            string jsonString = File.ReadAllText(scoreboardPath);
            return JsonSerializer.Deserialize<ScoreboardSave>(jsonString) ?? new ScoreboardSave();
        }

        /// Получить лучшие попытки прохождения, отсортированные по количеству сокровищ
        public static List<GameStatisticsSave> GetTopAttempts(string scoreboardPath, int count = 0)
        {
            var scoreboard = LoadScoreboard(scoreboardPath);
            
            // Сортируем по количеству сокровищ (по убыванию)
            var sorted = scoreboard.SessionStats
                .OrderByDescending(s => s.TreasuresCollected)
                .ToList();

            // Возвращаем топ N или все, если count <= 0
            if (count > 0 && count < sorted.Count)
            {
                return sorted.Take(count).ToList();
            }

            return sorted;
        }
    }
}
