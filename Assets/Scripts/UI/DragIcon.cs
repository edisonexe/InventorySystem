using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DragIcon : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _root;

        private void Awake() => gameObject.SetActive(false);

        public void Show(InventoryItem item)
        {
            gameObject.SetActive(true); 
            _root.SetActive(true);
            _icon.sprite = item.Config.Icon;
        }

        public void MoveTo(Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, 
                screenPos, _canvas.worldCamera, out var local);
            (transform as RectTransform).anchoredPosition = local;
        }

        public void Hide()
        {
            _root.SetActive(false);
            _icon.sprite = null;
        }
    }
}