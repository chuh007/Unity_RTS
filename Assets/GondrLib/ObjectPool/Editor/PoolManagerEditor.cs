using System;
using System.Collections.Generic;
using System.IO;
using ObjectPool.RunTime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ObjectPool.Editor
{
    public class PoolManagerEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualAsset = default;
        [SerializeField] private PoolManagerSO poolManager = default;
        [SerializeField] private VisualTreeAsset itemAsset; //아이템 UI
        private string _rootFolder = "Assets/ObjectPool";
        
        private Button _createBtn;
        private ScrollView _itemView;
        
        private List<PoolItemUI> _itemList;
        private PoolItemUI _currentItem;

        private UnityEditor.Editor _cachedEditor;
        private VisualElement _inspector;

        
        [MenuItem("Tools/PoolManager")]
        public static void ShowWindow()
        {
            PoolManagerEditor wnd = GetWindow<PoolManagerEditor>();
            wnd.titleContent = new GUIContent("PoolManager");
        }
        
        private void InitializeRootFolder()
        {
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string dataPath = Application.dataPath;
            _rootFolder = Directory.GetParent(Path.GetDirectoryName(scriptPath)).FullName.Replace("\\", "/");
            if (_rootFolder.StartsWith(dataPath))
            {
                _rootFolder = "Assets" + _rootFolder.Substring(dataPath.Length);
            }
        }

        public void CreateGUI()
        {
            InitializeRootFolder();
            VisualElement root = rootVisualElement;
            visualAsset.CloneTree(root);

            InitializeItems(root);
            GeneratePoolingItemUI();
        }

        private void InitializeItems(VisualElement root)
        {
            _createBtn = root.Q<Button>("CreateBtn");
            _createBtn.clicked += HandleCreateItem;
            _itemView = root.Q<ScrollView>("ItemView");

            _itemView.Clear();
            _itemList = new List<PoolItemUI>();
            
            _inspector = root.Q<VisualElement>("InspectorView");
        }
        
        private void GeneratePoolingItemUI()
        {
            _itemView.Clear();
            _itemList.Clear();
            _inspector.Clear();

            if (poolManager == null)
            {
                string filePath = $"{_rootFolder}/PoolManager.asset";
                poolManager = AssetDatabase.LoadAssetAtPath<PoolManagerSO>(filePath);
                if(poolManager == null)
                {
                    Debug.LogWarning("pool manager so is not exist, create new one");
                    poolManager = ScriptableObject.CreateInstance<PoolManagerSO>();
                    AssetDatabase.CreateAsset(poolManager, filePath );
                }
            }
            
            foreach(PoolingItemSO item in poolManager.itemList)
            {
                TemplateContainer itemUI = itemAsset.Instantiate();
                PoolItemUI poolItem = new PoolItemUI(itemUI, item);
                _itemView.Add(itemUI); //스크롤뷰에 넣고 리스트 관리
                _itemList.Add(poolItem);

                poolItem.Name = item.name;

                poolItem.OnSelectEvent += HandleSelectionEvent;
                poolItem.OnDeleteEvent += HandleDeleteEvent;
            }
        }

        private void HandleDeleteEvent(PoolItemUI item)
        {
            poolManager.itemList.Remove(item.poolingItem);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.poolingItem));
            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();

            if(item == _currentItem){
                _currentItem = null;
                //인스펙터 클리어도 해야함. 아직 인스펙터가 없어서 여기서 끝.
            }

            GeneratePoolingItemUI(); 

        }
        
        private void HandleCreateItem()
        {
            //타입 생성
            Guid itemGUID = Guid.NewGuid();
            PoolingItemSO newItem = ScriptableObject.CreateInstance<PoolingItemSO>();
            newItem.poolingName = itemGUID.ToString();

            if (Directory.Exists($"{_rootFolder}/Items") == false)
            {
                Directory.CreateDirectory($"{_rootFolder}/Items");
            }
            
            AssetDatabase.CreateAsset(newItem, $"{_rootFolder}/Items/{newItem.poolingName}.asset");
            
            poolManager.itemList.Add(newItem);
    
            EditorUtility.SetDirty(poolManager);
            AssetDatabase.SaveAssets();

            GeneratePoolingItemUI();
        }


        private void HandleSelectionEvent(PoolItemUI selectItem)
        {
            _itemList.ForEach(item => item.IsActive = false);
            selectItem.IsActive = true; 
            _currentItem = selectItem;
            
            _inspector.Clear();
            UnityEditor.Editor.CreateCachedEditor(_currentItem.poolingItem, null, ref _cachedEditor);
            VisualElement inspectorElement = _cachedEditor.CreateInspectorGUI();
            
            SerializedObject serializedObject = new SerializedObject(_currentItem.poolingItem);
            inspectorElement.Bind(serializedObject);
            inspectorElement.TrackSerializedObjectValue(serializedObject, so =>
            {
                selectItem.Name = so.FindProperty("poolingName").stringValue;
            });
            _inspector.Add(inspectorElement);
        }

        
    }
}
