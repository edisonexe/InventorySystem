using System;

namespace Core
{
    public class InventoryModel
    {
        public event Action<int> OnSlotChanged;

        private readonly InventoryItem[] _slots;
        
        public InventoryModel(int width, int height)
        {
            _slots = new InventoryItem[width * height];
            for (int i = 0; i < _slots.Length; i++) _slots[i] = InventoryItem.Empty;
        }
        
        public int SlotCount => _slots.Length;
        public InventoryItem Get(int index) => _slots[index];
        
        public void Set(int index, InventoryItem item)
        {
            _slots[index] = item;
            OnSlotChanged?.Invoke(index);
        }
        
        public bool TryAddAt(int index, InventoryItem item)
        {
            var dst = Get(index);
            if (dst.IsEmpty)
            {
                Set(index, item);
                return true;
            }
            
            if (!dst.IsEmpty && dst.Config == item.Config && dst.Stackable)
            {
                int total = dst.Count + item.Count;
                int canPlace = Math.Min(dst.MaxStack, total);
                int remainder = total - canPlace;
                Set(index, new InventoryItem(dst.Config, canPlace));
                return remainder == 0;
            }
            return false;
        }
        
        public void Swap(int a, int b)
        {
            if (a == b) return;
            var tmp = _slots[a];
            _slots[a] = _slots[b];
            _slots[b] = tmp;
            OnSlotChanged?.Invoke(a);
            OnSlotChanged?.Invoke(b);
        }
        
        public bool TryMerge(int from, int to)
        {
            if (from == to) return false;
            var src = Get(from);
            var dst = Get(to);
            if (src.IsEmpty) return false;

            if (dst.IsEmpty)
            {
                Set(to, src);
                Set(from, InventoryItem.Empty);
                return true;
            }
            
            if (src.Config == dst.Config && src.Stackable)
            {
                int total = src.Count + dst.Count;
                int canPlace = Math.Min(dst.MaxStack, total);
                int remainder = total - canPlace;
                Set(to, new InventoryItem(dst.Config, canPlace));
                Set(from, remainder > 0 ? new InventoryItem(src.Config, remainder) : InventoryItem.Empty);
                return true;
            }
            return false;
        }

        public void TryMergeInternal(InventoryItem dragging, int targetIndex)
        {
            var dst = Get(targetIndex);
            if (dst.IsEmpty || dst.Config != dragging.Config || !dst.Stackable) return;

            int total = dragging.Count + dst.Count;
            int canPlace = Math.Min(dst.MaxStack, total);
            int remainder = total - canPlace;

            Set(targetIndex, new InventoryItem(dst.Config, canPlace));

            if (remainder > 0)
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    if (Get(i).IsEmpty)
                    {
                        Set(i, new InventoryItem(dragging.Config, remainder));
                        return;
                    }
                }
            }
        }
        
        public bool UseAt(int index)
        {
            var it = Get(index);
            if (it.IsEmpty) return false;

            if (it.Stackable)
            {
                var decreased = it.WithDecreasedCount(1);
                Set(index, decreased.Count > 0 ? decreased : InventoryItem.Empty);
            }
            else
            {
                Set(index, InventoryItem.Empty);
            }
            return true;
        }
        
        public void RemoveAt(int index) => Set(index, InventoryItem.Empty);
    }
}