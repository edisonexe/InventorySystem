using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TooltipView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _type;
        [SerializeField] private TMP_Text _desc;

        private void Awake() => gameObject.SetActive(false);

        public void Show(InventoryItem item, RectTransform anchor)
        {
            _title.text = item.Config.Name;
            _type.text = "Type:" + item.Config.Type.ToString();
            _desc.text = item.Config.Description;
            _root.gameObject.SetActive(true);
            
            var canvas = GetComponentInParent<Canvas>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, 
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, anchor.position), 
                canvas.worldCamera, out var local);
            _root.anchoredPosition = local + new Vector2(300, 0);
        }

        public void Hide() => _root.gameObject.SetActive(false);
    }
}