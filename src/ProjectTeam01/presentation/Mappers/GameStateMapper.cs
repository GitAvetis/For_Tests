using System.Collections.Generic;
using System.Linq;
using ProjectTeam01.domain;
using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.generation;
using ProjectTeam01.domain.Items;
using ProjectTeam01.domain.Session;
using ProjectTeam01.presentation.ViewModels;

namespace ProjectTeam01.presentation.Mappers;

/// Этот класс НЕ предназначен для прямого использования фронтендом!
/// 
/// Фронтенд должен использовать GameController.GetGameStateViewModel() для получения GameStateViewModel.
/// Этот маппер используется внутри GameController и недоступен извне (internal class).
/// 
/// Этот класс преобразует внутренние объекты игры (GameState) в простые структуры данных (ViewModels),
/// которые содержат только информацию, необходимую для отображения на экране.
/// 
/// ДЛЯ ФРОНТЕНДА - используйте GameController:
/// 1. Вызовите GameController.GetGameStateViewModel() для получения GameStateViewModel
/// 2. Используйте свойства GameStateViewModel для отрисовки игры:
///    - Player - данные игрока (HP, позиция, характеристики)
///    - Enemies - список всех врагов на уровне
///    - Items - список всех предметов на уровне
///    - Level - геометрия уровня (комнаты, коридоры, выход)
///    - CurrentLevelNumber - номер текущего уровня

internal static class GameStateMapper
{
    /// Этот метод создает полное представление состояния игры.
    /// Используйте возвращаемый объект для отрисовки всех элементов игры.

    public static GameStateViewModel ToViewModel(GameState gameState)
    {
        return new GameStateViewModel
        {
            // Игрок - основная информация о персонаже
            Player = ToPlayerViewModel(gameState),
            
            // Список всех врагов на уровне (включая мертвых)
            // ВАЖНО: проверяйте IsDead перед отрисовкой
            // IsDead - флаг смерти врага
            // MimicRepresentation - представление Mimic
            // IsInvisible - флаг невидимости Ghost
            // Position - координаты врага на карте
            // EnemyType - тип врага
            Enemies = gameState.Enemies.Select(ToEnemyViewModel).ToList(),
            
            // Список всех предметов на уровне (оружие, еда, эликсиры, свитки, сокровища)
            // Type - тип предмета
            // Position - координаты предмета на карте
            // WeaponType - тип оружия
            // StrengthBonus - бонус к силе при экипировке
            // HealthValue - количество HP, которое восстанавливает еда (5-20)
            // ElixirType - тип эффекта (BuffStrength/BuffAgility/BuffMaxHp)
            // ScrollType - тип усиления (Strength/Agility/MaxHp)
            // Price - стоимость в золоте
            Items = gameState.Items.Select(ToItemViewModel).ToList(),   
            
            // Геометрия уровня - комнаты, коридоры, позиции выхода и старта
            // Rooms - список всех комнат на уровне
            // Corridors - список всех коридоров на уровне
            // ExitPosition - координаты выхода (отрисовывайте символ 'E')
            // LevelNumber - номер уровня (для отображения в UI)
            Level = ToLevelViewModel(gameState.LevelGeometry),
            
            // Номер текущего уровня
            // CurrentLevelNumber - номер текущего уровня
            CurrentLevelNumber = gameState.CurrentLevelNumber,
            
            // Инвентарь игрока (предметы без Position)
            // Используется для отображения списка предметов в UI при выборе (клавиши h, j, k, e)
            InventoryWeapons = gameState.PlayerWeapons.Select(ToInventoryItemViewModel).ToList(),
            InventoryFood = gameState.PlayerFood.Select(ToInventoryItemViewModel).ToList(),
            InventoryElixirs = gameState.PlayerElixirs.Select(ToInventoryItemViewModel).ToList(),
            InventoryScrolls = gameState.PlayerScrolls.Select(ToInventoryItemViewModel).ToList()
        };
    }

    /// СВОЙСТВА:
    /// - Position: координаты игрока на карте (X, Y)
    /// - Health: текущее здоровье игрока
    /// - MaxHealth: максимальное здоровье игрока
    /// - Agility: ловкость (влияет на шанс попадания)
    /// - Strength: сила (влияет на урон)
    /// - IsSleep: true если игрок спит (не может двигаться/атаковать)
    /// 
    private static PlayerViewModel ToPlayerViewModel(GameState gameState)
    {
        return new PlayerViewModel
        {
            Position = gameState.PlayerPosition,        // Координаты для отрисовки на карте
            Health = gameState.PlayerHealth,            // Текущее HP для статус-бара
            MaxHealth = gameState.PlayerMaxHealth,      // Максимальное HP для прогресс-бара
            Agility = gameState.PlayerAgility,          // Ловкость для отображения характеристик
            Strength = gameState.PlayerStrength,        // Сила для отображения характеристик
            IsSleep = gameState.PlayerIsSleep           // Статус сна (показывать предупреждение/блокировать действия)
        };
    }


    /// Используйте эти данные для:
    /// - Отрисовки врагов на карте (разные символы для разных EnemyType)
    /// - Скрытия невидимых призраков (IsInvisible = true)
    /// - Отображения Mimic как предмета (если MimicRepresentation != Mimic)
    /// 
    /// СВОЙСТВА:
    /// - Position: координаты врага на карте
    /// - EnemyType: тип врага (Zombie, Vampire, Ghost, Ogre, Snake, Mimic)
    /// - IsDead: true если враг мертв (не отрисовывайте мертвых врагов)
    /// - IsTriggered: true если враг активирован (движется к игроку)
    /// - MimicRepresentation: только для Mimic - как он выглядит (Food/Elixir/Scroll/Weapon/Mimic)
    /// - IsInvisible: только для Ghost - true если призрак невидим (не отрисовывайте)
    /// 
    /// ПРИМЕР ОТРИСОВКИ:
    /// if (enemy.IsDead) return; // Пропускаем мертвых
    /// if (enemy.IsInvisible == true) return; // Невидимых призраков не показываем
    /// char symbol = enemy.EnemyType switch {
    ///     EnemyTypeEnum.Zombie => 'z',
    ///     EnemyTypeEnum.Vampire => 'v',
    ///     // ...
    /// };
    /// DrawAt(enemy.Position.X, enemy.Position.Y, symbol);

    private static EnemyViewModel ToEnemyViewModel(Enemy enemy)
    {
        var viewModel = new EnemyViewModel
        {
            Position = enemy.Position,          // Координаты для отрисовки
            EnemyType = enemy.EnemyType,        // Тип для выбора символа/спрайта
            IsDead = enemy.IsDead,              // Флаг смерти (не отрисовывать)
            IsTriggered = enemy.IsTriggered      // Флаг активации (враг движется к игроку)
        };

        // ОСОБЕННОСТИ MIMIC:
        // Mimic может выглядеть как предмет (Food/Elixir/Scroll/Weapon),
        // пока не активирован. Когда IsTriggered = true, Representation меняется на Mimic.
        // ДЛЯ ФРОНТЕНДА: если MimicRepresentation != Mimic, отрисовывайте его как предмет.
        if (enemy is Mimic mimic)
        {
            viewModel.MimicRepresentation = mimic.Representation;
        }

        // ОСОБЕННОСТИ GHOST:
        // Призрак может быть невидимым. Когда IsInvisible = true, его не видно на карте.
        // ДЛЯ ФРОНТЕНДА: если IsInvisible == true, не отрисовывайте этого врага.
        if (enemy is Ghost ghost)
        {
            viewModel.IsInvisible = ghost.IsInvisible;
        }

        return viewModel;
    }


    /// Используйте эти данные для:
    /// - Отрисовки предметов на карте (разные символы для разных Type)
    /// - Показывания информации о предмете при наведении/подборе
    /// - Отображения характеристик предмета (HealthValue для еды, Price для сокровищ и т.д.)
    /// 
    /// СВОЙСТВА:
    /// - Position: координаты предмета на карте
    /// - Type: тип предмета (Weapon, Food, Elixir, Scroll, Treasure)
    /// 
    /// СПЕЦИФИЧНЫЕ СВОЙСТВА (заполняются только для соответствующих типов):
    /// 
    /// ДЛЯ ОРУЖИЯ (Type = Weapon):
    /// - WeaponType: тип оружия (Sword, Bow, Staff)
    /// - StrengthBonus: бонус к силе при экипировке
    /// 
    /// ДЛЯ ЕДЫ (Type = Food):
    /// - HealthValue: количество HP, которое восстанавливает еда (5-20)
    /// 
    /// ДЛЯ ЭЛИКСИРА (Type = Elixir):
    /// - ElixirType: тип эффекта (BuffStrength, BuffAgility, BuffMaxHp)
    /// 
    /// ДЛЯ СВИТКА (Type = Scroll):
    /// - ScrollType: тип свитка (Strength, Agility, MaxHp)
    /// 
    /// ДЛЯ СОКРОВИЩА (Type = Treasure):
    /// - Price: стоимость в золоте


    private static ItemViewModel ToItemViewModel(Item item)
    {
        var viewModel = new ItemViewModel
        {
            Position = item.Position,    // Координаты для отрисовки
            Type = item.Type             // Тип для выбора символа/логики
        };

        // Заполняем специфичные свойства в зависимости от типа предмета
        switch (item)
        {
            // ОРУЖИЕ: показывает тип и бонус к силе
            case Weapon weapon:
                viewModel.WeaponType = weapon.WeaponType;           // Тип оружия для отображения
                viewModel.StrengthBonus = weapon.StrengthBonus;      // Бонус силы для UI
                break;

            // ЕДА: показывает количество восстанавливаемого HP
            case Food food:
                viewModel.HealthValue = food.HealthValue;            // HP для восстановления (5-20)
                break;

            // ЭЛИКСИР: показывает тип эффекта
            case Elixir elixir:
                viewModel.ElixirType = elixir.ElixirType;          // Тип эффекта (BuffStrength/BuffAgility/BuffMaxHp)
                break;

            // СВИТОК: показывает тип усиления
            case Scroll scroll:
                viewModel.ScrollType = scroll.ScrollType;          // Тип усиления (Strength/Agility/MaxHp)
                break;

            // СОКРОВИЩЕ: показывает стоимость
            case Treasure treasure:
                viewModel.Price = treasure.Price;                   // Стоимость в золоте
                break;
        }

        return viewModel;
    }

    /// Преобразует предмет из инвентаря в InventoryItemViewModel (без Position)
    /// Используется для отображения предметов в инвентаре игрока
    /// РЮКЗАК 
    private static InventoryItemViewModel ToInventoryItemViewModel(Item item)
    {
        var viewModel = new InventoryItemViewModel
        {
            Type = item.Type  // Тип для выбора символа/логики
        };

        // Заполняем специфичные свойства в зависимости от типа предмета
        switch (item)
        {
            // ОРУЖИЕ: показывает тип и бонус к силе
            case Weapon weapon:
                viewModel.WeaponType = weapon.WeaponType;           // Тип оружия для отображения
                viewModel.StrengthBonus = weapon.StrengthBonus;      // Бонус силы для UI
                break;

            // ЕДА: показывает количество восстанавливаемого HP
            case Food food:
                viewModel.HealthValue = food.HealthValue;            // HP для восстановления (5-20)
                break;

            // ЭЛИКСИР: показывает тип эффекта
            case Elixir elixir:
                viewModel.ElixirType = elixir.ElixirType;          // Тип эффекта (BuffStrength/BuffAgility/BuffMaxHp)
                break;

            // СВИТОК: показывает тип усиления
            case Scroll scroll:
                viewModel.ScrollType = scroll.ScrollType;          // Тип усиления (Strength/Agility/MaxHp)
                break;

            // СОКРОВИЩЕ: показывает стоимость
            case Treasure treasure:
                viewModel.Price = treasure.Price;                   // Стоимость в золоте
                break;
        }

        return viewModel;
    }


    /// - Отрисовки геометрии уровня (комнаты, коридоры)
    /// - Отображения стартовой позиции и выхода
    /// - Проверки проходимости клеток (IsWalkable)
    /// 
    /// СВОЙСТВА:
    /// - Rooms: список всех комнат на уровне
    /// - Corridors: список всех коридоров на уровне
    /// - ExitPosition: координаты выхода (отрисовывайте символ 'E')
    /// - StartPosition: координаты стартовой позиции (отрисовывайте символ 'S')
    /// - LevelNumber: номер уровня (для отображения в UI)
    /// 
    /// ПРИМЕР ОТРИСОВКИ:
    /// 1. Отрисуйте все комнаты (стены и пол)
    /// 2. Отрисуйте все коридоры (пол)
    /// 3. Отрисуйте двери в комнатах
    /// 4. Отрисуйте StartPosition и ExitPosition
  
    private static LevelViewModel ToLevelViewModel(Level level)
    {
        return new LevelViewModel
        {
            Rooms = level.Rooms.Select(ToRoomViewModel).ToList(),           // Комнаты для отрисовки
            Corridors = level.Corridors.Select(ToCorridorViewModel).ToList(), // Коридоры для отрисовки
            ExitPosition = level.ExitPosition,                               // Позиция выхода (символ 'E')
            StartPosition = level.StartPosition,                             // Стартовая позиция (символ 'S')
            LevelNumber = level.LevelNumber                                   // Номер уровня (для UI)
        };
    }


    /// - Отрисовки прямоугольной комнаты (стены по периметру, пол внутри)
    /// - Отображения дверей 
    /// - Выделения стартовой и конечной комнат
    /// 
    /// СВОЙСТВА:
    /// - TopLeft: левый верхний угол комнаты (X, Y)
    /// - BottomRight: правый нижний угол комнаты (X, Y)
    /// - Doors: массив позиций дверей (отрисовывайте символ '+' на каждой позиции)
    /// - IsStartRoom: true если это стартовая комната (можно выделить визуально)
    /// - IsEndRoom: true если это конечная комната (можно выделить визуально)
 
    private static RoomViewModel ToRoomViewModel(Room room)
    {
        return new RoomViewModel
        {
            TopLeft = room.TopLeft,              // Левый верхний угол для отрисовки
            BottomRight = room.BottomRight,      // Правый нижний угол для отрисовки
            // Фильтруем пустые двери (где X=0 и Y=0) - они не используются
            Doors = room.Doors.Where(d => d.X != 0 || d.Y != 0).ToArray(),
            IsStartRoom = room.IsStartRoom,      // Флаг стартовой комнаты (для выделения)
            IsEndRoom = room.IsEndRoom           // Флаг конечной комнаты (для выделения)
        };
    }


    /// - Отрисовки коридора
    /// - Определения типа коридора (для разных стилей отрисовки)
    /// 
    /// СВОЙСТВА:
    /// - Type: тип коридора (Horizontal, Vertical, LShape, TShape, Cross)
    /// - Cells: список всех клеток коридора (отрисовывайте символ '.' на каждой)

    private static CorridorViewModel ToCorridorViewModel(Corridor corridor)
    {
        return new CorridorViewModel
        {
            Type = corridor.Type,                    // Тип коридора (для стилизации)
            Cells = corridor.Cells.ToList()          // Все клетки коридора для отрисовки
        };
    }
}

