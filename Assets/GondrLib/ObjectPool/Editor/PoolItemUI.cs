using System;
using ObjectPool.RunTime;
using UnityEngine.UIElements;

namespace ObjectPool.Editor
{
    public class PoolItemUI
    {
        private Label _nameLabel;
        private Button _deleteBtn;
        private VisualElement _rootElement;

        public event Action<PoolItemUI> OnDeleteEvent;
        public event Action<PoolItemUI> OnSelectEvent;

        public string Name {
            get => _nameLabel.text;
            set {
                _nameLabel.text = value;
            }
        }

        public PoolingItemSO poolingItem;

        public bool IsActive {
            get => _rootElement.ClassListContains("active");
            set {
                if(value){
                    _rootElement.AddToClassList("active");
                }else{
                    _rootElement.RemoveFromClassList("active");
                }
            }
        }

        public PoolItemUI(VisualElement root, PoolingItemSO item)
        {
            poolingItem = item;
            _rootElement = root.Q("PoolItem");
            _nameLabel = _rootElement.Q<Label>("ItemName");
            _deleteBtn = _rootElement.Q<Button>("DeleteBtn");
            _deleteBtn.RegisterCallback<ClickEvent>(evt => {
                OnDeleteEvent?.Invoke(this);
                evt.StopPropagation(); //Stop event propagation to parent
            });

            _rootElement.RegisterCallback<ClickEvent>(evt =>{
                OnSelectEvent?.Invoke(this);
                evt.StopPropagation(); //Stop event propagation to parent
            });
        }

    }
}