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
        
        public InventoryItem Get(int index)
        {
            ValidateIndex(index);
            return _slots[index];
        }
        
        public void Set(int index, InventoryItem item)
        {
            ValidateIndex(index);
            _slots[index] = item;
            OnSlotChanged?.Invoke(index);
        }
        
        public void RemoveAt(int index)
        {
            ValidateIndex(index);
            Set(index, InventoryItem.Empty);
        }

        public bool TryMerge(int from, int to)
        {
            ValidateIndex(from);
            ValidateIndex(to);
            if (from == to) return false;

            var srcBefore = _slots[from];
            var dstBefore = _slots[to];

            if (srcBefore.IsEmpty) return false;

            // целевой пуст — просто перенос
            if (dstBefore.IsEmpty)
            {
                _slots[to] = srcBefore;
                _slots[from] = InventoryItem.Empty;
                OnChanged(to);
                OnChanged(from);
                return true;
            }

            // одинаковый тип и стэкабельно — докладываем в to, остаток в from
            if (srcBefore.Config == dstBefore.Config && srcBefore.Stackable)
            {
                int total = srcBefore.Count + dstBefore.Count;
                int place = Math.Min(dstBefore.MaxStack, total);
                int remainder = total - place;
                
                var newDst = new InventoryItem(dstBefore.Config, place);
                var newSrc = remainder > 0
                    ? new InventoryItem(srcBefore.Config, remainder)   // возврат остатка
                    : InventoryItem.Empty;

                _slots[to] = newDst;
                _slots[from] = newSrc;

                OnChanged(to);
                OnChanged(from);
                return true;
            }
            
            return false;
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
        
        
        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= _slots.Length)
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Index {index} is out of range [0..{_slots.Length - 1}]");
        }
        
        private void OnChanged(int index) => OnSlotChanged?.Invoke(index);
    }
}