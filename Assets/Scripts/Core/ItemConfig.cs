using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Configs/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private ItemType _type;
        [SerializeField] private bool _stackable;
        [SerializeField][Min(1)] private int _maxStackCount;

        public int MaxStackCount => _maxStackCount;
        public bool Stackable => _stackable;
        public ItemType Type => _type;
        public Sprite Icon => _icon;
        public string Description => _description;
        public string Name => _name;
    }
}