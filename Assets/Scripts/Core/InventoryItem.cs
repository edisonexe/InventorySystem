using System;

namespace Core
{
    [Serializable]
    public struct InventoryItem
    {
        private ItemConfig _itemConfig;
        private int _count;
        
        public ItemConfig Config => _itemConfig;
        public int Count => _count;
        
        public bool IsEmpty => _itemConfig == null || _count <= 0;
        public int MaxStack => _itemConfig != null ? _itemConfig.MaxStackCount : 0;
        public bool Stackable => _itemConfig != null && _itemConfig.Stackable;
        
        public static InventoryItem Empty => new InventoryItem (null, 0);
        
        public InventoryItem(ItemConfig config, int count)
        {
            _itemConfig = config;
            _count = count;
        }
        
        public InventoryItem WithDecreasedCount(int amount = 1)
        {
            int newCount = Math.Max(0, _count - amount);
            return new InventoryItem(_itemConfig, newCount);
        }
    }
}