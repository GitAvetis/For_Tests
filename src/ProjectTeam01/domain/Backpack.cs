using ProjectTeam01.domain.Characters;
using ProjectTeam01.domain.Items;
using ProjectTeam01.domain.Items.Interfaces;

namespace ProjectTeam01.domain
{
    // отвечает за логику рюкзака
    internal class Backpack
    {
        private readonly Dictionary<ItemType, List<Item>> _items;
        private const int MaxElementsOfOneType = 9;//максимальное количество предметов одного типа

        public Backpack()
        {
            _items = new Dictionary<ItemType, List<Item>>();
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
                _items[type] = new List<Item>();
        }

        // чтобы проходить по всем предметам в рюкзаке вне зависимости от их типа
        public IEnumerable<Item> AllItems
        {
            get
            {
                return _items.SelectMany(kv => kv.Value);
            }
        }

        public IEnumerable<Item> UsableItems
        {
            get
            {
                return _items.SelectMany(kv => kv.Value).Where(item => item is IUsableItem);//список расходуемых предметов
            }
        }

        public bool EquipWeapon(Weapon weapon, Hero hero)
        {
            if (AllItems.Contains(weapon))
            {
                hero.WeaponManager.EquipWeapon(weapon);
                return true;
            }
            return false;
        }


        public bool IsEmpty
        {
            get
            {
                return !AllItems.Any();
            }
        }

        public bool Add(Item item)
        {
            List<Item> list = _items[item.Type];
            if (item is Treasure newTreasure)//проверяем, если клад
            {
                AddTreasure(list, newTreasure);
                return true;
            }

            if (list.Count < MaxElementsOfOneType)
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static void AddTreasure(List<Item> list, Treasure newTreasure)
        {
            if (list.Count == 0)
            {
                list.Add(newTreasure);
            }
            else
            {
                if (list[0] is Treasure existingTreasure)
                    existingTreasure.Price += newTreasure.Price;
            }
        }

        public void DeleteItem(Item item)
        {
            List<Item> list = _items[item.Type];
            list.Remove(item);
        }

        public bool UseItem(Item item, Hero hero)
        {
            if (item is not IUsableItem usableItem)
                return false;

            List<Item> list = _items[item.Type];
            if (!list.Contains(item))
                return false;

            usableItem.Use(hero);
            DeleteItem(item);
            return true;
        }

        /// Суммарная стоимость сокровищ в рюкзаке
        public int GetTotalTreasureValue()
        {
            var treasureList = _items[ItemType.Treasure];
            if (treasureList.Count == 0)
                return 0;

            return treasureList[0] is Treasure treasure ? treasure.Price : 0;
        }
    }
}
