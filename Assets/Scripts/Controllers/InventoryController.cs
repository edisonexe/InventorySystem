using System;
using Core;
using UnityEngine;

namespace Controllers
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private InventoryGrid _gridView;
        
        [Header("Random Fill")]
        [SerializeField] private ItemConfig[] _spawnPool;
        [SerializeField] private int _randomItems = 12;
        [SerializeField] private int _maxInitialStack = 20;
        
        [SerializeField] private int _width = 5;
        [SerializeField] private int _height = 4;
        private System.Random _rng;
        private InventoryItem _draggingItem;
        public InventoryModel Model {get; private set;}
        public int draggingFrom = -1;
        public int Width => _width;
        
        private void Awake()
        {
            Model = new InventoryModel(_width, _height);
            Model.OnSlotChanged += OnSlotChanged;
            _gridView.Build(this, _width, _height);
            _rng = new System.Random(); 
            FillRandom();
        }

        private void OnDestroy() => Model.OnSlotChanged -= OnSlotChanged;
        private void OnSlotChanged(int index) => _gridView.RefreshSlot(index);
        
        public void BeginDrag(int index)
        {
            var item = Model.Get(index);
            if (item.IsEmpty) return;
            draggingFrom = index;
            _draggingItem = item;
            Model.Set(index, InventoryItem.Empty);
            _gridView.ShowDragIcon(item);
        }
        
        public void DropOn(int targetIndex)
        {
            if (draggingFrom < 0) return;
            
            if (targetIndex == draggingFrom)
            {
                Model.Set(draggingFrom, _draggingItem);
                EndDrag();
                return;
            }

            var target = Model.Get(targetIndex);

            if (target.IsEmpty)
            {
                Model.Set(targetIndex, _draggingItem);
                EndDrag();
                return;
            }
            
            if (target.Config == _draggingItem.Config && target.Stackable)
            {
                Model.TryMergeInternal(_draggingItem, targetIndex);
                EndDrag();
                return;
            }

            var tmp = target;
            Model.Set(targetIndex, _draggingItem);
            Model.Set(draggingFrom, tmp);

            EndDrag();
        }

        public void CancelDrag()
        {
            if (draggingFrom >= 0 && !_draggingItem.IsEmpty)
            {
                Model.TryAddAt(draggingFrom, _draggingItem);
            }
            EndDrag();
        }

        public void UseAt(int index)
        {
            if (Model.UseAt(index)) 
                _gridView.PlayUseFeedback(index);
        }

        public void DropAt(int index) => Model.RemoveAt(index);
        
        private void EndDrag()
        {
            _gridView.HideDragIcon();
            draggingFrom = -1;
            _draggingItem = InventoryItem.Empty;
        }
        
        private void FillRandom()
        {
            if (_spawnPool == null || _spawnPool.Length == 0) return;

            int placed = 0;
            int attempts = 0;
            int maxAttempts = _randomItems * 10;

            while (placed < _randomItems && attempts++ < maxAttempts)
            {
                int idx = _rng.Next(0, Model.SlotCount);
                if (!Model.Get(idx).IsEmpty) continue;

                var config = _spawnPool[_rng.Next(0, _spawnPool.Length)];
                int count = config.Stackable 
                    ? _rng.Next(1, Math.Min(config.MaxStackCount, _maxInitialStack) + 1) 
                    : 1;

                Model.Set(idx, new InventoryItem(config, count));
                placed++;
            }
        }
    }
}