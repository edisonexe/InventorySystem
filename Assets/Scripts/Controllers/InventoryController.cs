using Core;
using UnityEngine;

namespace Controllers
{
    public class InventoryController : MonoBehaviour
    {
        [Header("GridSize")]
        [SerializeField] private int _width = 5;
        [SerializeField] private int _height = 4;

        [SerializeField] private InventoryGrid _gridView;
        public InventoryModel Model {get; private set;}
        
        public int draggingFrom = -1;
        private InventoryItem _draggingItem;
        
        private void Awake()
        {
            Model = new InventoryModel(_width, _height);
            Model.OnSlotChanged += OnSlotChanged;
            _gridView.Build(this, _width, _height);
            _gridView.DebugFillDemoItems();
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

        private void EndDrag()
        {
            _gridView.HideDragIcon();
            draggingFrom = -1;
            _draggingItem = InventoryItem.Empty;
        }

        public void UseAt(int index)
        {
            if (Model.UseAt(index)) 
                _gridView.PlayUseFeedback(index);
        }

        public void DropAt(int index) => Model.RemoveAt(index);
    }
}