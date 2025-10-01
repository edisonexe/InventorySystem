using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _countRoot;
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Button _deleteButton;
        
        private InventoryGrid _grid;
        public int Index { get; private set; }

        public void Bind(InventoryGrid grid, int index)
        {
            _grid = grid;
            Index = index;
            Render(InventoryItem.Empty);
        }

        public void Render(InventoryItem item)
        {
            bool has = !item.IsEmpty;
            _icon.enabled = has;
            if (has)
            {
                _icon.sprite = item.Config.Icon;
                bool showCount = item.Stackable && item.Count > 1;
                _countRoot.SetActive(showCount);
                if (showCount) _countText.text = item.Count.ToString();
            }
            else
            {
                _countRoot.SetActive(false);
                _icon.sprite = null;
                
                if (_grid != null && _grid.Tooltip != null)
                    _grid.Tooltip.Hide();
            }
        }
    }
}