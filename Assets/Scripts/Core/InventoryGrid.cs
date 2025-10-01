using Controllers;
using Core.Input;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class InventoryGrid : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private SlotView _slotPrefab;
        [SerializeField] private DragIcon _dragIcon;
        [SerializeField] private TooltipView _tooltip;

        [SerializeField] private ItemConfig _demoWeapon1;
        [SerializeField] private ItemConfig _demoWeapon2;
        [SerializeField] private ItemConfig _demoPotion1;
        [SerializeField] private ItemConfig _demoPotion2;
        
        private InventoryController _controller;
        private SlotView[] _views;
        
        public DragIcon DragIcon => _dragIcon;
        public TooltipView Tooltip => _tooltip;
        public InventoryController Controller => _controller;  
        
        public void Build(InventoryController controller, int w, int h)
        {
            _controller = controller;
            int count = w * h;
            _views = new SlotView[count];
            for (int i = 0; i < count; i++)
            {
                var view = Instantiate(_slotPrefab, _grid.transform);
                view.Bind(this, i);
                _views[i] = view;
                
                var input = view.GetComponent<SlotInput>();
                if (input != null)
                {
                    input.Init(this, view);
                }

                RefreshSlot(i);
            }
        }
        
        public void RefreshSlot(int index)
        {
            if (_views == null || index < 0 || index >= _views.Length) return;
            _views[index].Render(_controller.Model.Get(index));
        }
        
        public void ShowDragIcon(InventoryItem item) => _dragIcon.Show(item);

        public void HideDragIcon() => _dragIcon.Hide();
        
        public void PlayUseFeedback(int index) { }

        public void DebugFillDemoItems()
        {
            var m = _controller.Model;
            m.Set(0, new InventoryItem (_demoWeapon1, 1));
            m.Set(1, new InventoryItem (_demoWeapon2, 10));
            m.Set(2, new InventoryItem (_demoPotion1, 10));
            m.Set(3, new InventoryItem (_demoPotion2, 10));
            m.Set(4, new InventoryItem (_demoPotion2, 10));
            m.Set(5, new InventoryItem (_demoWeapon2, 10));
            m.Set(6, new InventoryItem (_demoPotion1, 10));
            m.Set(7, new InventoryItem (_demoPotion1, 20));
        }
    }
}