using Controllers;
using Script.Core;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Input
{
    public class SlotInput : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        private SlotView _view;
        private InventoryGrid _grid;

        private float _lastClickTime;
        private const float _doubleClickThreshold = 0.25f;

        private InventoryController Controller => _grid.Controller;

        public void Init(InventoryGrid grid, SlotView view)
        {
            _grid = grid;
            _view = view;
        }
        
        public void OnPointerDown(PointerEventData e)
        {
            if (Time.unscaledTime - _lastClickTime < _doubleClickThreshold)
            {
                Controller.UseAt(_view.Index);
                _lastClickTime = 0f;
            }
            else _lastClickTime = Time.unscaledTime;
        }

        public void OnPointerUp(PointerEventData e) { }

        public void OnBeginDrag(PointerEventData e) => Controller.BeginDrag(_view.Index);

        public void OnDrag(PointerEventData e) => _grid.DragIcon.MoveTo(e.position);

        public void OnEndDrag(PointerEventData e)
        {
            var target = e.pointerCurrentRaycast.gameObject;
            var slot = target ? target.GetComponentInParent<SlotView>() : null;
            if (slot != null) Controller.DropOn(slot.Index);
            else Controller.CancelDrag();
        }

        public void OnPointerEnter(PointerEventData e)
        {
            var item = Controller.Model.Get(_view.Index);
            if (!item.IsEmpty && _grid.Tooltip != null)
                _grid.Tooltip.Show(item, transform as RectTransform);
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (_grid.Tooltip != null) _grid.Tooltip.Hide();
        }
    }
}